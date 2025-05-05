
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float stoppingDistance = 0.5f;

    private NavMeshAgent agent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastPosition;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Configure agent for 2D
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.stoppingDistance = stoppingDistance;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath();
        }

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        Vector2 currentPosition = transform.position;
        Vector2 movementDirection = (currentPosition - lastPosition).normalized;

        if (animator != null)
        {
            animator.SetBool("IsMoving", agent.velocity.magnitude > 0.1f);

            if (Mathf.Abs(movementDirection.x) > 0.1f)
            {
                spriteRenderer.flipX = movementDirection.x < 0;
            }
        }

        lastPosition = currentPosition;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}