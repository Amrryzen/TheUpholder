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

    [HideInInspector] public Transform player;
    [HideInInspector] public int currentPatrolIndex = 0;
    [HideInInspector] public bool isWaiting = false;

    private EnemyStateMachine stateMachine;

    /// <summary>
    /// Pada Awake, kita pastikan referensi animator & spriteRenderer tidak null.
    /// Jika belum di‐assign dari Inspector, kita ambil via GetComponent&lt;T&gt;().
    /// </summary>
    private void Awake()
    {
        // Cari Player berdasarkan tag
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Pastikan ada Animator
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
                Debug.LogWarning($"{name}: Animator belum di‐assign dan tidak ditemukan di GameObject.");
        }

        // Pastikan ada SpriteRenderer
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                Debug.LogWarning($"{name}: SpriteRenderer belum di‐assign dan tidak ditemukan di GameObject.");
        }

        stateMachine = new EnemyStateMachine();
    }

    private void Start()
    {
        // Inisialisasi state machine dengan Patrol
        stateMachine.Initialize(new EnemyStatePatrol(this, stateMachine));
    }

    private void Update()
    {
        stateMachine?.CurrentState?.Update();
    }

    private void FixedUpdate()
    {
        stateMachine?.CurrentState?.FixedUpdate();
    }

    /// <summary>
    /// Cek jarak player
    /// </summary>
    public bool IsPlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.position) < range;
    }

    /// <summary>
    /// Metode untuk flip sprite berdasarkan vektor arah (x negatif => flip:left, x positif => flip:right).
    /// </summary>
    public void FlipSpriteBasedOnDirection(Vector2 direction)
    {
        if (spriteRenderer == null) return;
        if (direction.x < 0)
            spriteRenderer.flipX = true;
        else if (direction.x > 0)
            spriteRenderer.flipX = false;
        // Jika direction.x == 0, tidak di‐ubah (tetap arah sebelumnya).
    }

    private void OnDrawGizmos()
    {
        if (showDetectionRange)
        {
            Gizmos.color = detectionRangeColor;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            Gizmos.color = new Color(detectionRangeColor.r, detectionRangeColor.g, detectionRangeColor.b, 0.1f);
            Gizmos.DrawSphere(transform.position, detectionRange);
        }

        if (showAttackRange)
        {
            Gizmos.color = attackRangeColor;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = new Color(attackRangeColor.r, attackRangeColor.g, attackRangeColor.b, 0.2f);
            Gizmos.DrawSphere(transform.position, attackRange);
        }

        if (showPatrolPath && patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = patrolPathColor;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.2f);
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    else if (i == patrolPoints.Length - 1 && patrolPoints[0] != null)
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
                }
            }

            if (Application.isPlaying &&
                currentPatrolIndex < patrolPoints.Length &&
                patrolPoints[currentPatrolIndex] != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(patrolPoints[currentPatrolIndex].position, 0.3f);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying && stateMachine != null && stateMachine.CurrentState != null)
        {
            UnityEditor.Handles.BeginGUI();
            Vector3 screenPos = UnityEditor.HandleUtility
                .WorldToGUIPoint(transform.position + Vector3.up * 1.5f);
            string stateName = stateMachine.CurrentState.GetType()
                .Name.Replace("EnemyState", "");
            UnityEditor.Handles.Label(screenPos, stateName);
            UnityEditor.Handles.EndGUI();
        }
    }
#endif
}
