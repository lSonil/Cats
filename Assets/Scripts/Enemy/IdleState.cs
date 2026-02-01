using UnityEngine;

public class IdleState : IEnemyState
{
    private Vector3 startPosition;
    private bool movingRight = true;
    private int wallDetectionCooldown = 0; // Prevent wall detection immediately after entering from Return
    private bool skipWallDetectionNextFrame = false; // Skip wall checks for one frame after wall hit

    public void Enter(EnemyAI enemy)
    {
        startPosition = enemy.GetStartPosition();

        // Determine initial direction based on SpriteRenderer flipX setting
        // Default: facing left (movingRight = false), unless flipX is checked (then facing right)
        SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.flipX)
        {
            movingRight = true; // Flip X checked = facing right
        }
        else
        {
            movingRight = false; // Default = facing left
        }

        // When entering IdleState, disable wall detection for one frame if we came from a wall collision
        wallDetectionCooldown = 2; // Skip for 2 frames to ensure we're stable
    }

    public void Update(EnemyAI enemy)
    {
        // Decrement cooldown
        if (wallDetectionCooldown > 0)
        {
            wallDetectionCooldown--;
            return; // Skip patrol update on the frame we entered
        }

        if (enemy.patrolOnIdle)
        {
            Patrol(enemy);
        }
    }

    public void Exit(EnemyAI enemy)
    {
        // Cleanup if needed when leaving this state
    }

    private void Patrol(EnemyAI enemy)
    {
        // Calculate how far we've moved from start position
        float distanceMoved = enemy.transform.position.x - startPosition.x;
        float direction = movingRight ? 1f : -1f;

        // Debug.Log($"IdleState.Patrol: distanceMoved={distanceMoved:F2}, patrolDistance={enemy.PatrolDistance:F2}, direction={direction}, moving={movingRight}");

        // Check for wall in movement direction (but not immediately after entering from a wall collision)
        if (wallDetectionCooldown <= 0 && enemy.IsWallInDirection(direction))
        {
            // Hit a wall, turn around immediately
            movingRight = !movingRight;
            Flip(enemy);
            direction = -direction; // Update direction for movement this frame
        }
        // Check if we've reached patrol boundaries
        else if (movingRight && distanceMoved >= enemy.PatrolDistance)
        {
            movingRight = false;
            // Debug.Log("IdleState.Patrol: Reached right boundary, flipping");
            Flip(enemy);
        }
        else if (!movingRight && distanceMoved <= -enemy.PatrolDistance)
        {
            movingRight = true;
            // Debug.Log("IdleState.Patrol: Reached left boundary, flipping");
            Flip(enemy);
        }

        // Move in current direction
        enemy.Rb.linearVelocity = new Vector2(direction * enemy.PatrolSpeed, enemy.Rb.linearVelocity.y);
    }

    private void Flip(EnemyAI enemy)
    {
        // Flip the enemy sprite to face movement direction
        Vector3 scale = enemy.transform.localScale;
        scale.x *= -1;
        enemy.transform.localScale = scale;
    }

}
