using UnityEngine;

public class IdleState : IEnemyState
{
    private Vector3 startPosition;
    private bool movingRight = true;

    public void Enter(EnemyAI enemy)
    {
        startPosition = enemy.GetStartPosition();
        movingRight = true;
        Debug.Log($"IdleState: Entered. Start position: {startPosition}, patrolOnIdle: {enemy.patrolOnIdle}");
    }

    public void Update(EnemyAI enemy)
    {
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

        Debug.Log($"IdleState.Patrol: distanceMoved={distanceMoved:F2}, patrolDistance={enemy.PatrolDistance:F2}, direction={direction}, moving={movingRight}");

        // Check if we've reached patrol boundaries
        if (movingRight && distanceMoved >= enemy.PatrolDistance)
        {
            movingRight = false;
            Debug.Log("IdleState.Patrol: Reached right boundary, flipping");
            Flip(enemy);
        }
        else if (!movingRight && distanceMoved <= -enemy.PatrolDistance)
        {
            movingRight = true;
            Debug.Log("IdleState.Patrol: Reached left boundary, flipping");
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
