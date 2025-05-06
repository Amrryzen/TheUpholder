using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateChase : EnemyState
{
    public EnemyStateChase(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entered Chase State");

        // Set animation if needed
        if (enemy.animator != null)
        {
            enemy.animator.SetBool("IsWalking", false);
            enemy.animator.SetBool("IsChasing", true);
            enemy.animator.SetBool("IsAttacking", false);
        }
    }

    public override void Update()
    {
        // If player is out of detection range, go back to patrol
        if (!enemy.IsPlayerInRange(enemy.detectionRange * 1.2f)) // Give a bit of extra range before giving up
        {
            stateMachine.ChangeState(new EnemyStatePatrol(enemy, stateMachine));
            return;
        }

        // If player is in attack range, switch to attack state
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
            // Move towards the player
            Vector2 direction = (enemy.player.position - enemy.transform.position).normalized;
            enemy.transform.position += (Vector3)direction * enemy.chaseSpeed * Time.fixedDeltaTime;

            // Flip sprite based on movement direction
            enemy.FlipSpriteBasedOnDirection(direction);
        }
    }
}
