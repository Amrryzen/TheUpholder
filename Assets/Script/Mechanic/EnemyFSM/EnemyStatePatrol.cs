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

        if (enemy.animator != null)
        {
            enemy.animator.SetBool("IsWalking", true);
            enemy.animator.SetBool("IsChasing", false);
            enemy.animator.SetBool("IsAttacking", false);
        }
    }

    public override void Update()
    {
        if (enemy.IsPlayerInRange(enemy.detectionRange))
        {
            stateMachine.ChangeState(new EnemyStateChase(enemy, stateMachine));
            return;
        }

        if (enemy.isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= enemy.patrolWaitTime)
            {
                enemy.isWaiting = false;
                waitTimer = 0f;
                enemy.currentPatrolIndex = (enemy.currentPatrolIndex + 1) % enemy.patrolPoints.Length;
                if (enemy.animator != null)
                    enemy.animator.SetBool("IsWalking", true);
            }
        }
    }

    public override void FixedUpdate()
    {
        if (!enemy.isWaiting && enemy.patrolPoints.Length > 0)
        {
            Transform target = enemy.patrolPoints[enemy.currentPatrolIndex];
            Vector2 direction = (target.position - enemy.transform.position).normalized;
            enemy.transform.position += (Vector3)direction * enemy.moveSpeed * Time.fixedDeltaTime;

            // **Flip** sprite di sini
            enemy.FlipSpriteBasedOnDirection(direction);

            if (Vector2.Distance(enemy.transform.position, target.position) < 0.1f)
            {
                enemy.isWaiting = true;
                if (enemy.animator != null)
                    enemy.animator.SetBool("IsWalking", false);
            }
        }
    }

    public override void Exit()
    {
        waitTimer = 0f;
    }
}

