using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackState : IState
{
    public EnemyBase enemy;

    public float attackTimer = 1.65f;

    public MonsterAttackState(EnemyBase enemyBase)
    {
        this.enemy = enemyBase;
    }

    public void OnEnter()
    {
        enemy._animator.SetFloat("Run", 0);
        enemy._animator.SetTrigger("Attack");

        attackTimer = 1.65f;
        enemy.laskAttackTime = Time.time;
    }

    public void OnUpdate()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            enemy.stateMachine.ChangeState(MonsterStateType.Chase);
        }
    }

    public void OnExit()
    {
        
    }

}
