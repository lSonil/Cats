using UnityEngine;

public class ChaseState : IEnemyState
{
    private float lastDirection = 1f; // Remember the last facing direction
    private float timeLostPlayer = 0f; // Track how long the player has been out of detection range

    public void Enter(EnemyAI enemy)
    {
        // Any setup needed when entering chase state
        lastDirection = enemy.transform.localScale.x > 0 ? 1f : -1f;
        timeLostPlayer = 0f; // Reset loss timer
    }

    public void Update(EnemyAI enemy)
    {
        Chase(enemy);
    }

    public void Exit(EnemyAI enemy)
    {
        // Clear wall blocked flag when exiting chase state
        Animator animator = enemy.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("WallBlocked", false);
        }
    }

    private void Chase(EnemyAI enemy)
    {
        Transform playerTransform = enemy.GetPlayerTransform();

        if (playerTransform == null)
        {
            return;
        }

        // Check if player is still in detection range
        if (enemy.IsPlayerInDetectionRange())
        {
            // Player is in range, reset loss timer
            timeLostPlayer = 0f;
        }
        else
        {
            // Player is out of range, increment loss timer
            timeLostPlayer += Time.deltaTime * 1000f; // Convert to milliseconds

            // Check if timeout exceeded
            if (timeLostPlayer >= enemy.GetComponent<EnemyAI>().chaseLossTimeout)
            {
                // Lost target for too long, return to start position
                enemy.ChangeState(new ReturnState());
                return;
            }
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

        // Check for wall in movement direction
        if (enemy.IsWallInDirection(directionToPlayer))
        {
            // Hit a wall, stop movement but stay in chase state facing the wall
            enemy.Rb.linearVelocity = new Vector2(0f, enemy.Rb.linearVelocity.y);
            enemy.GetComponent<Animator>().SetBool("WallBlocked", true);
        }
        else
        {
            // Clear wall blocked flag if wall is no longer in the way
            enemy.GetComponent<Animator>().SetBool("WallBlocked", false);
            // Move towards player
            enemy.Rb.linearVelocity = new Vector2(directionToPlayer * enemy.ChaseSpeed, enemy.Rb.linearVelocity.y);
        }
    }

    private void Flip(EnemyAI enemy)
    {
        Vector3 scale = enemy.transform.localScale;
        scale.x *= -1;
        enemy.transform.localScale = scale;
    }
}
