using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public bool patrolOnIdle = true;
    public float patrolSpeed = 1f;
    public float patrolDistance = 2f;

    [Header("Chase Settings")]
    public float detectionDistance = 3f;
    public float chaseSpeed = 2f;

    [Header("Return Settings")]
    public float returnSpeed = 1.5f;
    public float arrivedThreshold = 0.2f;

    [Header("ID Settings")]
    public int Dog_Id;

    // Public properties for states to access
    public float PatrolSpeed => patrolSpeed;
    public float PatrolDistance => patrolDistance;
    public float ChaseSpeed => chaseSpeed;
    public float ReturnSpeed => returnSpeed;
    public float ArrivedThreshold => arrivedThreshold;
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
        SetGo();
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
                SetGo();
                ChangeState(new ReturnState());
            }
        }
        else
        {
            if (ShouldChasePlayer())
            {
                SetGo();
                ChangeState(new ChaseState());
            }
        }

        if (currentState != null)
        {
            currentState.Update(this);
        }

        IsMoving = currentState is ChaseState ||
                   currentState is ReturnState ||
                   (currentState is IdleState && patrolOnIdle);
    }
    public void SetSit()
    {
        ar.SetBool("Moving", false);
    }
    public void SetGo()
    {
        ar.SetBool("Moving", true);
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

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
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

    private void OnHeldItemChanged(Item newItem)
    {
        if (currentState is ChaseState && newItem != null && newItem.itemId == Dog_Id)
        {
            ChangeState(new ReturnState());
            SetGo();
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
}
