using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterChaseState : IState
{
    public EnemyBase enemy;

    public float chaseSpeed = 3f;

    public MonsterChaseState(EnemyBase enemyBase)
    {
        this.enemy = enemyBase;
    }

    public void OnEnter()
    {
        enemy._animator.SetFloat("Run", 1f);
    }

    public void OnUpdate()
    {
        if (!enemy.CanSeePlayer())
        {
            enemy.stateMachine.ChangeState(MonsterStateType.Patrol);
            return;
        }

        //攻击玩家
        if (enemy.CanAttack())
        {
            enemy.stateMachine.ChangeState(MonsterStateType.Attack);
        }

        MoveToPlayer();
    }


    public void OnExit()
    {
        
    }


    private void MoveToPlayer()
    {
        Vector3 toPlayer = enemy.GetPlayerDir();
        //看向玩家
        Quaternion toPlayerR = Quaternion.LookRotation(toPlayer);
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, toPlayerR, 10f * Time.deltaTime);

        //跑向玩家
        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, Player.Instance.transform.position, chaseSpeed * Time.deltaTime);
    }
}
