using System.Collections;
using UnityEngine;
using XLua;

public class EnemyBase : MonoBehaviour, IDamageable
{
    private LuaTable _ai;   // Lua 侧怪物 AI 状态机控制器

    public int monsterId = 1;
    //MonsterData ....

    public float HP = 100;
    public float Damage = 15;
    public float Crit_rate = 0.2f;
    public static float attackCooldown = 2f; //攻击冷却
    public float laskAttackTime = -attackCooldown; //最后一次攻击的时间
    public float attackRange = 2f;

    public bool isDead = false;

    public Animator _animator;

    public StateMachine stateMachine;

    private void Awake()
    {
        _animator = this.GetComponent<Animator>();
    }

    void Start()
    {
        _animator.SetFloat("Run", 0);

        // 状态机已 Lua 化：Lua 侧负责状态切换 / 条件 / 计时，这里只拿到控制器句柄
        _ai = LuaMgr.Instance.CreateMonsterAI(this);
    }

    void Update()
    {
        if (isDead) return;

        // 每帧驱动 Lua 状态机（逻辑在 monster_ai.lua）
        LuaMgr.Instance.TickMonsterAI(_ai, Time.deltaTime);
    }

    public void ApplyDamage(float damage)
    {
        print("受到伤害");
        HP = Mathf.Max(HP - damage, 0);

        //受击音效
        MusicManager.Instance.PlayOneShot(MusicType.MonsterHurt);

        LuaMgr.Instance.MonsterOnHurt(_ai, damage);

        if (HP <= 0 && !isDead)
        {
            Dead();
        }
    }

    public void Dead()
    {
        isDead = true;

        LuaMgr.Instance.MonsterOnDead(_ai);

        QuestManager.Instance.ReportAction(QuestType.KillMonster, monsterId.ToString());
    }

    public bool CanSeePlayer()
    {
        Vector3 toPlayer = GetPlayerDir();
        //距离很近直接看到
        if (toPlayer.magnitude <= 3f)
        {
            return true;
        }

        //距离中等则看视野角
        if (toPlayer.magnitude <= 8f)
        {
            Vector3 flatDir = toPlayer;
            flatDir.y = 0;

            float dot = Vector3.Dot(transform.forward, flatDir.normalized);
            float cosHalf = Mathf.Cos(90f * 0.5f * Mathf.Deg2Rad);
            if (dot < cosHalf) return false;

            // 3. 是否有物体遮挡
            if (Physics.Raycast(transform.position + Vector3.up * 1.5f, toPlayer.normalized, out RaycastHit hit, toPlayer.magnitude))
            {
                if (!hit.collider.CompareTag("Player")) return false;
            }

            return true;
        }

        return false;
    }

    public Vector3 GetPlayerDir()
    {
        Vector3 origin = transform.position + Vector3.up * 1.5f;        // 怪物眼睛高度
        Vector3 target = Player.Instance.transform.position + Vector3.up * 1.0f;     // 玩家胸口
        Vector3 toPlayer = target - origin;

        return toPlayer;
    }

    public bool CanAttack()
    {
        if (GetPlayerDir().magnitude >= 2f)
        {
            return false;
        }
        if (Time.time - laskAttackTime < attackCooldown)
        {
            return false;
        }

        return true;
    }

    public void OnAttackHit()
    {
        Vector3 center = transform.position + transform.forward * attackRange * 0.5f;
        Collider[] hits = Physics.OverlapSphere(center, attackRange * 0.5f, LayerMask.GetMask("Player"));

        foreach (var col in hits)
        {
            if (col.TryGetComponent<IDamageable>(out var target))
            {
                target.ApplyDamage(CalDamage());
            }
        }
    }

    public float CalDamage()
    {
        float dmg = Damage;

        if (Random.value <= Crit_rate)
        {
            dmg *= 2f;
        }

        return dmg;
    }

    // 死亡后延时移除尸体（原 MonsterDeadState.RemoveBody 移到这里，供 Lua 调用）
    public IEnumerator RemoveBody()
    {
        //先移除碰撞体
        yield return new WaitForSeconds(3f);
        GameObject.Destroy(GetComponent<Collider>());

        //再把尸体移除
        yield return new WaitForSeconds(3f);
        GameObject.Destroy(gameObject);
    }
}
