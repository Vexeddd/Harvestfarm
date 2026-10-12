
using UnityEngine;

public class MonsterPatrolState : IState
{
    public EnemyBase enemy;

    public Vector3 originPos;
    public Vector3 patrolTarget;

    public float patrolDistance = 3f;
    public float patrolSpeed = 1.5f;


    public MonsterPatrolState(EnemyBase enemy)
    {
        this.enemy = enemy;
    }

    public void OnEnter()
    {
        originPos = enemy.transform.position;

        SetRandomPatrolPoint();
        enemy._animator.SetFloat("Run", 0.5f);
    }

    public void OnUpdate()
    {
        if (enemy.CanSeePlayer())
        {
            enemy.stateMachine.ChangeState(MonsterStateType.Chase);
        }

        MoveToPatrolPoint();

        if (Vector3.Distance(enemy.transform.position, patrolTarget) <= 0.4)
        {
            SetRandomPatrolPoint();
        }
    }

    public void OnExit()
    {
        
    }

    private void SetRandomPatrolPoint()
    {
        patrolTarget = originPos + Random.insideUnitSphere * patrolDistance;
        patrolTarget.y = originPos.y;
    }

    private void MoveToPatrolPoint()
    {
        //×ªÏò
        Vector3 dir = patrolTarget - enemy.transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation, targetRot, 10f * Time.deltaTime);
        }

        //ÒÆ¶¯
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, patrolTarget, patrolSpeed * Time.deltaTime);
    }
}