using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateChase : EnemyState
{
    public EnemyStateChase(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entered Chase State");

        // animasi
        if (enemy.animator != null)
        {
            enemy.animator.SetBool("IsWalking", false);
            enemy.animator.SetBool("IsChasing", true);
            enemy.animator.SetBool("IsAttacking", false);
        }
    }

    public override void Update()
    {
        // out of detection range, kembali patrol
        if (!enemy.IsPlayerInRange(enemy.detectionRange * 1.2f)) // Give a bit of extra range before giving up
        {
            stateMachine.ChangeState(new EnemyStatePatrol(enemy, stateMachine));
            return;
        }

        // attact range, akan melakukan state attack
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
            // pergi ke arah player
            Vector2 direction = (enemy.player.position - enemy.transform.position).normalized;
            enemy.transform.position += (Vector3)direction * enemy.chaseSpeed * Time.fixedDeltaTime;

            // flip enemy
            enemy.FlipSpriteBasedOnDirection(direction);
        }
    }
}
