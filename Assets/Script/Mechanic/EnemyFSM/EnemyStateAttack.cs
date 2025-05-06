using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateAttack : EnemyState
{
    private float attackCooldown = 1.5f;
    private float attackTimer = 0f;
    private bool canAttack = true;

    public EnemyStateAttack(Enemy enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entered Attack State");
        attackTimer = 0f;
        canAttack = true;

        // Set animation if needed
        if (enemy.animator != null)
        {
            enemy.animator.SetBool("IsWalking", false);
            enemy.animator.SetBool("IsChasing", false);
            enemy.animator.SetBool("IsAttacking", true);
        }
    }

    public override void Update()
    {
        // If player is out of attack range, go back to chase
        if (!enemy.IsPlayerInRange(enemy.attackRange))
        {
            stateMachine.ChangeState(new EnemyStateChase(enemy, stateMachine));
            return;
        }

        // Attack logic with cooldown
        if (canAttack)
        {
            Attack();
            canAttack = false;
        }
        else
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0f;
            }
        }
    }

    private void Attack()
    {
        Debug.Log("Enemy is attacking!");

        // Trigger attack animation
        if (enemy.animator != null)
        {
            enemy.animator.SetTrigger("Attack");
        }

        // You can implement actual damage logic here
        // For example:
        // if (enemy.IsPlayerInRange(enemy.attackRange))
        // {
        //     enemy.player.GetComponent<PlayerHealth>().TakeDamage(10);
        // }
    }
}

