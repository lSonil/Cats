using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolSpeed = 2f;
    public float patrolDistance = 5f;

    private Vector3 startPosition;
    private bool movingRight = true;
    private Rigidbody2D rb;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        // Calculate how far we've moved from start position
        float distanceMoved = transform.position.x - startPosition.x;

        // Check if we've reached patrol boundaries
        if (movingRight && distanceMoved >= patrolDistance)
        {
            movingRight = false;
            Flip();
        }
        else if (!movingRight && distanceMoved <= -patrolDistance)
        {
            movingRight = true;
            Flip();
        }

        // Move in current direction
        float direction = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * patrolSpeed, rb.linearVelocity.y);
    }

    void Flip()
    {
        // Flip the enemy sprite to face movement direction
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
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
