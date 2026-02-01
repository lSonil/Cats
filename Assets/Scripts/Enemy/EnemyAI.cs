using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public bool patrolOnIdle = true;
    public float patrolSpeed = 1f;
    public float patrolDistance = 2f;

    [Header("Chase Settings")]
    public float detectionDistance = 3f;
    public float detectionHeight = 2f;
    public float chaseSpeed = 2f;

    [Header("Return Settings")]
    public float returnSpeed = 1.5f;

    [Header("Wall Detection Settings")]
    public LayerMask wallLayers;

    [Header("ID Settings")]
    public int Dog_Id;

    // Public properties for states to access
    public float PatrolSpeed => patrolSpeed;
    public float PatrolDistance => patrolDistance;
    public float ChaseSpeed => chaseSpeed;
    public float ReturnSpeed => returnSpeed;
    public Rigidbody2D Rb => rb;
    public bool IsMoving { get; private set; }

    private Rigidbody2D rb;
    private IEnemyState currentState;
    private Transform playerTransform;
    private PlayerInventory playerInventory;
    private Vector3 startPosition;
    Animator ar;


    void Start()
    {
        ar = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        // Find the player
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerInventory = playerObject.GetComponent<PlayerInventory>();
            if (playerInventory == null)
            {
                playerInventory = playerObject.GetComponentInParent<PlayerInventory>();
            }

            if (playerInventory != null)
            {
                playerInventory.HeldItemChanged += OnHeldItemChanged;
            }
        }

        // Initialize with Idle state
        ChangeState(new IdleState());
    }

    void OnDisable()
    {
        if (playerInventory != null)
        {
            playerInventory.HeldItemChanged -= OnHeldItemChanged;
        }
    }

    void Update()
    {
        if (currentState is ChaseState)
        {
            if (IsPlayerHoldingCorrectItem())
            {
                ChangeState(new ReturnState());
            }
        }
        else
        {
            if (ShouldChasePlayer())
            {
                ChangeState(new ChaseState());
            }
        }

        if (currentState != null)
        {
            currentState.Update(this);
        }

        // Check if actually moving based on velocity
        IsMoving = Mathf.Abs(rb.linearVelocity.x) > 0.01f;
        ar.SetBool("Moving", IsMoving);
    }
    public void ChangeState(IEnemyState newState)
    {
        // Exit current state
        if (currentState != null)
        {
            Debug.Log($"EnemyAI: Exiting state {currentState.GetType().Name}");
            currentState.Exit(this);
        }

        // Enter new state
        currentState = newState;
        if (currentState != null)
        {
            Debug.Log($"EnemyAI: Entering state {currentState.GetType().Name}");
            currentState.Enter(this);
        }
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public Vector3 GetStartPosition()
    {
        return startPosition;
    }

    public bool ShouldChasePlayer()
    {
        if (playerTransform == null)
        {
            return false;
        }

        // Get the center of the enemy (accounting for collider bounds)
        Vector3 detectionCenter = transform.position;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            detectionCenter = col.bounds.center;
        }

        Vector2 offset = playerTransform.position - detectionCenter;

        // Check vertical cap first
        if (Mathf.Abs(offset.y) >= detectionHeight)
        {
            return false;
        }

        // Check if within circular detection range
        float distanceToPlayer = offset.magnitude;
        if (distanceToPlayer >= detectionDistance)
        {
            return false;
        }

        return !IsPlayerHoldingCorrectItem();
    }

    private bool IsPlayerHoldingCorrectItem()
    {
        if (playerInventory == null)
        {
            return false;
        }

        Item heldItem = playerInventory.currentHeldItem;
        return heldItem != null && heldItem.itemId == Dog_Id;
    }

    public bool IsWallInDirection(float direction)
    {
        // Get the collider to determine raycast origin and size
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            return false;
        }

        // Cast from the center of the collider in the movement direction
        Vector2 rayOrigin = col.bounds.center;
        Vector2 rayDirection = direction > 0 ? Vector2.right : Vector2.left;

        // Raycast slightly beyond the collider edge (adaptive based on collider size)
        float wallDetectionDistance = col.bounds.extents.x * 0.9f; // Use 90% of half-width as detection distance
        float rayDistance = col.bounds.extents.x + wallDetectionDistance;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, rayDistance, wallLayers);

        return hit.collider != null;
    }

    private void OnHeldItemChanged(Item newItem)
    {
        if (currentState is ChaseState && newItem != null && newItem.itemId == Dog_Id)
        {
            ChangeState(new ReturnState());
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if enemy collided with player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory inventory = playerInventory;
            if (inventory == null)
            {
                inventory = collision.gameObject.GetComponentInParent<PlayerInventory>();
            }

            if (inventory == null || inventory.currentHeldItem == null)
            {
                GameManager.Instance.GameOver();
                return;
            }
            if (inventory.currentHeldItem.itemId != Dog_Id)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void OnDrawGizmos()
    {
        // --- PATROL BOUNDARIES ---
        Vector3 startPos = Application.isPlaying ? GetStartPosition() : transform.position;

        float halfWidth = 0f;
        Vector3 enemyPos = transform.position;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            halfWidth = col.bounds.extents.x;
            enemyPos = col.bounds.center;
        }

        // Calculate the "Hard Stop" points (center distance + body width)
        Vector3 leftBound = startPos + Vector3.left * (PatrolDistance + halfWidth);
        Vector3 rightBound = startPos + Vector3.right * (PatrolDistance + halfWidth);

        // Draw Vertical Boundary Lines
        Gizmos.color = Color.cyan;
        float h = 1.5f; // Height of the line
        Gizmos.DrawLine(leftBound + Vector3.down * h, leftBound + Vector3.up * h);
        Gizmos.DrawLine(rightBound + Vector3.down * h, rightBound + Vector3.up * h);

        // Draw connecting path line
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.DrawLine(leftBound, rightBound);


        // --- DETECTION RANGE ---
        // Draw capped circle (circle with horizontal cuts at top/bottom)
        Gizmos.color = Color.red;

        float topY = enemyPos.y + detectionHeight;
        float bottomY = enemyPos.y - detectionHeight;

        // Calculate the x-width at the cap height using circle equation: x = sqrt(r^2 - y^2)
        float capWidth = 0f;
        if (detectionHeight < detectionDistance)
        {
            capWidth = Mathf.Sqrt(detectionDistance * detectionDistance - detectionHeight * detectionHeight);
        }

        // Draw top and bottom cap lines
        Gizmos.DrawLine(new Vector3(enemyPos.x - capWidth, topY, 0), new Vector3(enemyPos.x + capWidth, topY, 0));
        Gizmos.DrawLine(new Vector3(enemyPos.x - capWidth, bottomY, 0), new Vector3(enemyPos.x + capWidth, bottomY, 0));

        // Draw the circular arcs (left and right curves)
        int segments = 64;

        // Calculate the angle at which the circle reaches the cap height
        // When y = detectionHeight: sin(angle) = detectionHeight / detectionDistance
        float capAngle = Mathf.Asin(detectionHeight / detectionDistance);

        // Right arc: from bottom-right cap to top-right cap
        Vector3 prevRightPoint = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float angle = -capAngle + (2 * capAngle) * i / segments; // From -capAngle to +capAngle
            float x = Mathf.Cos(angle) * detectionDistance;
            float y = Mathf.Sin(angle) * detectionDistance;

            Vector3 point = new Vector3(x, y, 0);
            if (i > 0)
            {
                Gizmos.DrawLine(enemyPos + prevRightPoint, enemyPos + point);
            }
            prevRightPoint = point;
        }

        // Left arc: from top-left cap to bottom-left cap
        Vector3 prevLeftPoint = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.PI - capAngle + (2 * capAngle) * i / segments; // From (PI - capAngle) to (PI + capAngle)
            float x = Mathf.Cos(angle) * detectionDistance;
            float y = Mathf.Sin(angle) * detectionDistance;

            Vector3 point = new Vector3(x, y, 0);
            if (i > 0)
            {
                Gizmos.DrawLine(enemyPos + prevLeftPoint, enemyPos + point);
            }
            prevLeftPoint = point;
        }
    }
}
