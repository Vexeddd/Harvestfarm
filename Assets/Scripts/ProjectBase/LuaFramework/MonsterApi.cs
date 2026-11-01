using UnityEngine;
using XLua;

/// <summary>
/// 暴露给 Lua 的怪物「引擎原语」层（稳定、性能敏感、需要强类型的 C# 能力）。
/// Lua 侧的状态机只写逻辑（状态切换 / 条件 / 计时），
/// 具体的移动、旋转、动画、感知、碰撞等能力通过这里调用，不写进 Lua。
/// </summary>
[LuaCallCSharp]
public static class MonsterApi
{
    // —— 感知 ——
    public static bool CanSeePlayer(EnemyBase e) => e.CanSeePlayer();
    public static bool CanAttack(EnemyBase e) => e.CanAttack();

    // —— 位置 ——
    public static Vector3 GetPosition(EnemyBase e) => e.transform.position;
    public static Vector3 GetPlayerPosition(EnemyBase e) => Player.Instance.transform.position;
    public static float DistanceTo(EnemyBase e, Vector3 target) => Vector3.Distance(e.transform.position, target);

    // —— 动画 ——
    public static void SetAnimRun(EnemyBase e, float value) => e._animator.SetFloat("Run", value);
    public static void SetAnimTrigger(EnemyBase e, string name) => e._animator.SetTrigger(name);

    // —— 移动 / 转向 ——
    public static void MoveTowards(EnemyBase e, Vector3 target, float speed)
    {
        e.transform.position = Vector3.MoveTowards(e.transform.position, target, speed * Time.deltaTime);
    }

    public static void RotateTowards(EnemyBase e, Vector3 target)
    {
        Vector3 dir = target - e.transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            e.transform.rotation = Quaternion.Slerp(e.transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    // —— 巡逻点 ——
    public static Vector3 GetRandomPatrolPoint(EnemyBase e, Vector3 origin, float distance)
    {
        Vector3 p = origin + Random.insideUnitSphere * distance;
        p.y = origin.y;
        return p;
    }

    // —— 攻击计时 ——
    public static void MarkAttackTime(EnemyBase e) => e.laskAttackTime = Time.time;

    // —— 死亡收尾 ——
    public static void StartRemoveBody(EnemyBase e) => e.StartCoroutine(e.RemoveBody());
}
