using UnityEngine;

public class ChaseState : IEnemyState
{
    private float lastDirection = 1f; // Remember the last facing direction

    public void Enter(EnemyAI enemy)
    {
        // Any setup needed when entering chase state
        lastDirection = enemy.transform.localScale.x > 0 ? 1f : -1f;
    }

    public void Update(EnemyAI enemy)
    {
        Chase(enemy);
    }

    public void Exit(EnemyAI enemy)
    {
        // Cleanup if needed when leaving this state
    }

    private void Chase(EnemyAI enemy)
    {
        Transform playerTransform = enemy.GetPlayerTransform();

        if (playerTransform == null)
        {
            return;
        }

        // Calculate dead zone based on enemy's collider width
        float deadZone = 0.1f; // Default fallback
        Collider2D collider = enemy.GetComponent<Collider2D>();
        if (collider != null)
        {
            deadZone = collider.bounds.extents.x; // Use half the width
        }

        // Calculate horizontal distance to player
        float horizontalDistance = playerTransform.position.x - enemy.transform.position.x;

        // Only update direction if player is not directly above/below (dead zone prevents oscillation)
        float directionToPlayer = lastDirection; // Default to last direction
        if (Mathf.Abs(horizontalDistance) > deadZone)
        {
            directionToPlayer = Mathf.Sign(horizontalDistance);
            lastDirection = directionToPlayer;
        }

        // Face the player
        if ((directionToPlayer > 0 && enemy.transform.localScale.x < 0) ||
            (directionToPlayer < 0 && enemy.transform.localScale.x > 0))
        {
            Flip(enemy);
        }

        // Move towards player
        enemy.Rb.linearVelocity = new Vector2(directionToPlayer * enemy.ChaseSpeed, enemy.Rb.linearVelocity.y);
    }

    private void Flip(EnemyAI enemy)
    {
        Vector3 scale = enemy.transform.localScale;
        scale.x *= -1;
        enemy.transform.localScale = scale;
    }
}
