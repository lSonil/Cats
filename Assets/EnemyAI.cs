using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Collision Settings")]
    public float damageAmount = 10f;

    private bool hasDealtDamage = false;

    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if enemy collided with player
        if (collision.CompareTag("Player") && !hasDealtDamage)
        {
            hasDealtDamage = true;

            // Trigger game over
            GameManager.Instance.GameOver();
        }
    }
}
