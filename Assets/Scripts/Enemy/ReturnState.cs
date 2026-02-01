using UnityEngine;

public class ReturnState : IEnemyState
{

    public void Enter(EnemyAI enemy)
    {
        Debug.Log($"ReturnState: Entered. Target start position: {enemy.GetStartPosition()}");
    }

    public void Update(EnemyAI enemy)
    {
        // If the player becomes a threat again, switch back to chase
        if (enemy.ShouldChasePlayer())
        {
            // Debug.Log("ReturnState: Player became a threat again, switching to Chase");
            enemy.ChangeState(new ChaseState());
            return;
        }

        Vector3 target = enemy.GetStartPosition();
        Vector3 current = enemy.transform.position;

        // Calculate arrived threshold based on enemy's collider width for consistency
        float arrivedThreshold = 0.1f; // Default fallback
        Collider2D collider = enemy.GetComponent<Collider2D>();
        if (collider != null)
        {
            arrivedThreshold = collider.bounds.extents.x; // Use half the width as threshold
        }

        float distance = Vector2.Distance(current, target);

        if (distance <= arrivedThreshold)
        {
            // Debug.Log("ReturnState: Arrived at start position, switching to Idle");
            enemy.Rb.linearVelocity = new Vector2(0f, enemy.Rb.linearVelocity.y);
            enemy.ChangeState(new IdleState());
            return;
        }

        float direction = Mathf.Sign(target.x - current.x);

        // Face the direction of travel
        if ((direction > 0 && enemy.transform.localScale.x < 0) ||
            (direction < 0 && enemy.transform.localScale.x > 0))
        {
            Flip(enemy);
        }

        enemy.Rb.linearVelocity = new Vector2(direction * enemy.ReturnSpeed, enemy.Rb.linearVelocity.y);
    }

    public void Exit(EnemyAI enemy)
    {
        // Cleanup if needed when leaving this state
    }

    private void Flip(EnemyAI enemy)
    {
        Vector3 scale = enemy.transform.localScale;
        scale.x *= -1;
        enemy.transform.localScale = scale;
    }
}
