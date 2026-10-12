using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public IState currentState;     

    public Dictionary<MonsterStateType, IState> stateDic = new Dictionary<MonsterStateType, IState>();

    public void ResgisterState(MonsterStateType type, IState state)
    {
        stateDic.Add(type, state);
    }

    public void ChangeState(MonsterStateType type)
    {
        var newState = stateDic[type];

        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }

    public void Update()
    {
        currentState?.OnUpdate();
    }

    public void InitMonsterStates(EnemyBase enemyBase)
    {
        ResgisterState(MonsterStateType.Patrol, new MonsterPatrolState(enemyBase));
        ResgisterState(MonsterStateType.Hurt, new MonsterHurtState(enemyBase));
        ResgisterState(MonsterStateType.Attack, new MonsterAttackState(enemyBase));
        ResgisterState(MonsterStateType.Chase, new MonsterChaseState(enemyBase));
        ResgisterState(MonsterStateType.Dead, new MonsterDeadState(enemyBase));
    }
}
