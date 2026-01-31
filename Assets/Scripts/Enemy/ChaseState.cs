using UnityEngine;

public class ChaseState : IEnemyState
{
    public void Enter(EnemyAI enemy)
    {
        // Any setup needed when entering chase state
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

        // Calculate direction to player
        float directionToPlayer = Mathf.Sign(playerTransform.position.x - enemy.transform.position.x);

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
