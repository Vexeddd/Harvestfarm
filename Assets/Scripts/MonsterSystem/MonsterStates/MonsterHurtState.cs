using UnityEngine;

public class MonsterHurtState : IState
{
    public EnemyBase enemy;

    public float timer = 0.1f; //½©Ö±

    public MonsterHurtState(EnemyBase enemyBase)
    {
        this.enemy = enemyBase;
    }

    public void OnEnter()
    {
        enemy._animator.SetFloat("Run", 0f);

        timer = 1f;
    }

    public void OnUpdate()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            enemy.stateMachine.ChangeState(MonsterStateType.Chase);
        }
    }

    public void OnExit()
    {

    }
}