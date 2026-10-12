using System.Collections;
using UnityEngine;

public class MonsterDeadState : IState
{
    public EnemyBase enemy;

    //public float animationTimer = 0.3f;

    public MonsterDeadState(EnemyBase enemyBase)
    {
        this.enemy = enemyBase;
    }

    public void OnEnter()
    {
        enemy._animator.SetTrigger("Dead");

        enemy.StartCoroutine(RemoveBody());
    }

    public void OnUpdate()
    {
        //animationTimer -= Time.deltaTime;
        //if (animationTimer <= 0)
        //{
        //}
    }

    public void OnExit()
    {
        
    }


    public IEnumerator RemoveBody()
    {
        //ÒÆ³ýÅö×²Æ÷
        yield return new WaitForSeconds(3f);
        GameObject.Destroy(enemy.GetComponent<Collider>());

        //³¹µ×ÒÆ³ý
        yield return new WaitForSeconds(3f);
        GameObject.Destroy(enemy.gameObject);
    }
}
