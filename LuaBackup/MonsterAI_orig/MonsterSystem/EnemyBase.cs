
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.UI.Image;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public StateMachine stateMachine;

    public int monsterId = 1;
    //MonsterData ....

    public float HP = 100;
    public float Damage = 15;
    public float Crit_rate = 0.2f;
    public static float attackCooldown = 2f; //攻击间隔
    public float laskAttackTime = -attackCooldown; //最后一次攻击的时间
    public float attackRange = 2f;

    public bool isDead = false;

    public Animator _animator;
    //public string runHash;
    //public string idleHash;
    //public string attackHash;

    private void Awake()
    {
        _animator = this.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _animator.SetFloat("Run", 0);

        stateMachine = new StateMachine();

        stateMachine.InitMonsterStates(this);
        stateMachine.ChangeState(MonsterStateType.Patrol);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }
        if (stateMachine != null)
        {
            stateMachine.currentState.OnUpdate();
        }
    }

    public void ApplyDamage(float damage)
    {
        print("被击中了");
        HP = Mathf.Max(HP - damage, 0);

        //播放音效
        MusicManager.Instance.PlayOneShot(MusicType.MonsterHurt);


        stateMachine.ChangeState(MonsterStateType.Hurt);

        if (HP <= 0 && !isDead)
        {
            Dead();
        }
    }

    public void Dead()
    {
        //Debug.Log("死了");
        isDead = true;
        stateMachine.ChangeState(MonsterStateType.Dead);

        QuestManager.Instance.ReportAction(QuestType.KillMonster, monsterId.ToString());
    }

    public bool CanSeePlayer()
    {
        Vector3 toPlayer = GetPlayerDir();
        //三米以内听到
        if (toPlayer.magnitude <= 3f)
        {
            return true;
        }

        //三米到五米看到
        if (toPlayer.magnitude <= 8f)
        {
            Vector3 flatDir = toPlayer;
            flatDir.y = 0;

            float dot = Vector3.Dot(transform.forward, flatDir.normalized);
            float cosHalf = Mathf.Cos(90f * 0.5f * Mathf.Deg2Rad);
            if (dot < cosHalf) return false;

            // 3. 视线遮挡检测
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

}
