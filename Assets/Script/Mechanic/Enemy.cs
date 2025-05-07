using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 3f;
    public float patrolWaitTime = 2f;
    public Transform[] patrolPoints;

    [Header("Detection Settings")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;
    public LayerMask playerLayer;

    [Header("Gizmo Settings")]
    public bool showDetectionRange = true;
    public bool showAttackRange = true;
    public bool showPatrolPath = true;
    public Color detectionRangeColor = new Color(0.8f, 0.8f, 0.2f, 0.3f);
    public Color attackRangeColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);
    public Color patrolPathColor = new Color(0.2f, 0.8f, 0.2f, 1f);

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    // variabel Internal
    [HideInInspector] public Transform player;
    [HideInInspector] public int currentPatrolIndex = 0;
    [HideInInspector] public bool isWaiting = false;

    // SM
    private EnemyStateMachine stateMachine;

    private void Awake()
    {
        // cari player
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // inisiasi SM, membuat EnemySM
        stateMachine = new EnemyStateMachine();
    }

    private void Start()
    {
        // mulai PatrolState
        stateMachine.Initialize(new EnemyStatePatrol(this, stateMachine));
    }

    private void Update()
    {
        if (stateMachine.CurrentState != null)
            stateMachine.CurrentState.Update();
    }

    private void FixedUpdate()
    {
        if (stateMachine.CurrentState != null)
            stateMachine.CurrentState.FixedUpdate();
    }

    // methode bantu untuk mengetahui range player
    public bool IsPlayerInRange(float range)
    {
        if (player == null) return false;

        return Vector2.Distance(transform.position, player.position) < range;
    }

    // methode bantu untuk flipping
    public void FlipSpriteBasedOnDirection(Vector2 direction)
    {
        if (direction.x != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }

    // Debug Visual
    private void OnDrawGizmos()
    {
        // Mengetahui detection range
        if (showDetectionRange)
        {
            Gizmos.color = detectionRangeColor;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            // semi transparan
            Gizmos.color = new Color(detectionRangeColor.r, detectionRangeColor.g, detectionRangeColor.b, 0.1f);
            Gizmos.DrawSphere(transform.position, detectionRange);
        }

        // Mengetahui attack range
        if (showAttackRange)
        {
            Gizmos.color = attackRangeColor;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            // semi transparan
            Gizmos.color = new Color(attackRangeColor.r, attackRangeColor.g, attackRangeColor.b, 0.2f);
            Gizmos.DrawSphere(transform.position, attackRange);
        }

        // patrol path
        if (showPatrolPath && patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = patrolPathColor;

            // gambar arah patrol
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    // gambar range pada patrol point
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);

                    // gambar range pada patrol point berikutnya
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                    // sambungkan keduanya
                    else if (i == patrolPoints.Length - 1 && patrolPoints[0] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
                    }
                }
            }

            // play mode, highlight patrol point
            if (Application.isPlaying && currentPatrolIndex < patrolPoints.Length && patrolPoints[currentPatrolIndex] != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(patrolPoints[currentPatrolIndex].position, 0.3f);
            }
        }
    }

    // selected show
    private void OnDrawGizmosSelected()
    {
        // Show current state name
        if (Application.isPlaying && stateMachine != null && stateMachine.CurrentState != null)
        {
            // Draw state name above the enemy
            UnityEditor.Handles.BeginGUI();
            Vector3 screenPos = UnityEditor.HandleUtility.WorldToGUIPoint(transform.position + Vector3.up * 1.5f);
            string stateName = stateMachine.CurrentState.GetType().Name.Replace("EnemyState", "");
            UnityEditor.Handles.Label(screenPos, stateName);
            UnityEditor.Handles.EndGUI();
        }
    }
}
