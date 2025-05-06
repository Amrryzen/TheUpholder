using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatePatrol : EnemyState
{
    private float waitTimer = 0f;

    public EnemyStatePatrol(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void Enter()
    {
        enemy.isWaiting = false;
        Debug.Log("Entered Patrol State");

        // Set animation if needed
        if (enemy.animator != null)
        {
            enemy.animator.SetBool("IsWalking", true);
            enemy.animator.SetBool("IsChasing", false);
            enemy.animator.SetBool("IsAttacking", false);
        }
    }

    public override void Update()
    {
        // Check if player is in detection range
        if (enemy.IsPlayerInRange(enemy.detectionRange))
        {
            stateMachine.ChangeState(new EnemyStateChase(enemy, stateMachine));
            return;
        }

        // Handle waiting at patrol points
        if (enemy.isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= enemy.patrolWaitTime)
            {
                enemy.isWaiting = false;
                waitTimer = 0f;

                // Move to next patrol point
                enemy.currentPatrolIndex = (enemy.currentPatrolIndex + 1) % enemy.patrolPoints.Length;
            }
        }
    }

    public override void FixedUpdate()
    {
        if (!enemy.isWaiting && enemy.patrolPoints.Length > 0)
        {
            // Get current patrol point
            Transform target = enemy.patrolPoints[enemy.currentPatrolIndex];

            // Move towards the patrol point
            Vector2 direction = (target.position - enemy.transform.position).normalized;
            enemy.transform.position += (Vector3)direction * enemy.moveSpeed * Time.fixedDeltaTime;

            // Flip sprite based on movement direction
            enemy.FlipSpriteBasedOnDirection(direction);

            // Check if we've reached the patrol point
            if (Vector2.Distance(enemy.transform.position, target.position) < 0.1f)
            {
                enemy.isWaiting = true;

                // Update animation if needed
                if (enemy.animator != null)
                {
                    enemy.animator.SetBool("IsWalking", false);
                }
            }
        }
    }

    public override void Exit()
    {
        waitTimer = 0f;
    }
}
