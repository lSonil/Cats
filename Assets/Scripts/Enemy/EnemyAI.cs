using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public bool patrolOnIdle = true;
    public float patrolSpeed = 1f;
    public float patrolDistance = 2f;

    [Header("Chase Settings")]
    public float detectionDistance = 3f;
    public float chaseSpeed = 2f;

    // Public properties for states to access
    public float PatrolSpeed => patrolSpeed;
    public float PatrolDistance => patrolDistance;
    public float ChaseSpeed => chaseSpeed;
    public Rigidbody2D Rb => rb;

    private Rigidbody2D rb;
    private IEnemyState currentState;
    private Transform playerTransform;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Find the player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }

        // Initialize with Idle state
        ChangeState(new IdleState());
    }

    void Update()
    {
        // Check if player is in detection range
        if (playerTransform != null && !isChasing)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer < detectionDistance)
            {
                isChasing = true;
                ChangeState(new ChaseState());
            }
        }

        if (currentState != null)
        {
            currentState.Update(this);
        }
    }

    public void ChangeState(IEnemyState newState)
    {
        // Exit current state
        if (currentState != null)
        {
            currentState.Exit(this);
        }

        // Enter new state
        currentState = newState;
        if (currentState != null)
        {
            currentState.Enter(this);
        }
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if enemy collided with player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Trigger game over
            GameManager.Instance.GameOver();
        }
    }
}
