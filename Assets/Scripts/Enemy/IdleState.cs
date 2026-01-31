using UnityEngine;

public class IdleState : IEnemyState
{
    private Vector3 startPosition;
    private bool movingRight = true;

    public void Enter(EnemyAI enemy)
    {
        startPosition = enemy.transform.position;
        movingRight = true;
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

        // Check if we've reached patrol boundaries
        if (movingRight && distanceMoved >= enemy.PatrolDistance)
        {
            movingRight = false;
            Flip(enemy);
        }
        else if (!movingRight && distanceMoved <= -enemy.PatrolDistance)
        {
            movingRight = true;
            Flip(enemy);
        }

        // Move in current direction
        float direction = movingRight ? 1f : -1f;
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
