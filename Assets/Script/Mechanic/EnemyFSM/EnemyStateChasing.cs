using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateChase : EnemyState
{
    public EnemyStateChase(Enemy enemy, EnemyStateMachine stateMachine) 
        : base(enemy, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entered Chase State");
        if (enemy.animator != null)
        {
            enemy.animator.SetBool("IsWalking", false);
            enemy.animator.SetBool("IsChasing", true);
            enemy.animator.SetBool("IsAttacking", false);
        }
    }

    public override void Update()
    {
        if (!enemy.IsPlayerInRange(enemy.detectionRange * 1.2f))
        {
            stateMachine.ChangeState(new EnemyStatePatrol(enemy, stateMachine));
            return;
        }
        if (enemy.IsPlayerInRange(enemy.attackRange))
        {
            stateMachine.ChangeState(new EnemyStateAttack(enemy, stateMachine));
            return;
        }
    }

    public override void FixedUpdate()
    {
        if (enemy.player != null)
        {
            Vector2 direction = (enemy.player.position - enemy.transform.position).normalized;
            enemy.transform.position += (Vector3)direction * enemy.chaseSpeed * Time.fixedDeltaTime;

            // **Flip** sprite di sini
            enemy.FlipSpriteBasedOnDirection(direction);
        }
    }
}

