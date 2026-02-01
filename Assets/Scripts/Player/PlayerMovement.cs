using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    [Header("Jump Charge Settings")]
    public float minJump = 10f; // Set higher so tap jump feels real
    public float maxJump = 25f;
    public float chargeStartThreshold = 0.15f; // Time before jump charging begins
    public float maxChargeTime = 1.0f; // Time to reach full charge (can be modified by masks)

    [Header("Detection Settings")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask itemFilterLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private float jumpChargeTimer;
    private bool isCharging;
    private bool isHoldingJump;
    private float jumpHoldTimer;

    [Header("Idler animation")]
    public float idleThreshold = 5.0f; // Time in seconds before becoming "bored"
    private float idleTimer = 0f;
    public bool isIdleLongTime = false; // The flag

    void Start() => rb = GetComponent<Rigidbody2D>();
    public bool Grounded() => isGrounded;
    public bool Charging() => isCharging;
    public bool HoldingJump() => isHoldingJump;
    public bool ShouldShowInAir() => !isGrounded && !isHoldingJump; // Only show InAir when actually airborne and not charging
    public bool Idle() => isIdleLongTime;
    public bool Falling() => rb.linearVelocity.y < 0;
    public bool Moving() => rb.linearVelocity.x != 0;
    public float GetJumpChargeProgress() => isHoldingJump ? Mathf.Clamp01(jumpHoldTimer / maxChargeTime) : 0f;
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();



    // This function runs on BOTH press and release if Action Type is "Pass Through"
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (isGrounded)
            {
                isHoldingJump = true;
                jumpHoldTimer = 0f;
            }
        }
        else // This is the "Key Unpress"
        {
            if (isHoldingJump)
            {
                PerformJump();
                isHoldingJump = false;
                isCharging = false;
                jumpHoldTimer = 0f;
            }
        }
    }

    private void PerformJump()
    {
        // Calculate charge percentage based on maxChargeTime
        float chargePercent = isCharging ? Mathf.Clamp01(jumpChargeTimer / maxChargeTime) : 0f;
        float finalJumpForce = Mathf.Lerp(minJump, maxJump, chargePercent);

        // 1. Determine direction based on where the sprite is looking
        // If flipX is true, we are looking Left (-1). If false, Right (1).
        float jumpDirection = GetComponent<SpriteRenderer>().flipX ? -1f : 1f;

        // 2. Apply both X and Y forces
        rb.linearVelocity = new Vector2(jumpDirection * finalJumpForce, finalJumpForce);
        isGrounded = false;
        isCharging = false;
        jumpChargeTimer = 0f;

        // Force animator to InAir state
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("InAir", 0, 0f);
        }
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer) ||
                     Physics2D.OverlapCircle(groundCheck.position, checkRadius, itemFilterLayer);

        // Handle jump hold timing and charging
        if (isHoldingJump)
        {
            jumpHoldTimer += Time.deltaTime;

            // Start charging only after threshold is exceeded
            if (!isCharging && jumpHoldTimer >= chargeStartThreshold)
            {
                isCharging = true;
                jumpChargeTimer = jumpHoldTimer; // Carry over the elapsed time
            }

            // Continue charging if already started
            if (isCharging)
            {
                jumpChargeTimer += Time.deltaTime;
            }
        }
    }

    void FixedUpdate()
    {
        // If charging, we don't move horizontally via keys
        if (isCharging)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            // Standard movement
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }

        // Handle Sprite Flipping
        if (moveInput.x != 0)
        {
            GetComponent<SpriteRenderer>().flipX = (moveInput.x < 0);
        }

        // Handle Idle Timer
        if (moveInput.x == 0 && isGrounded) // Only idle if standing still on ground
        {
            idleTimer += Time.fixedDeltaTime;
            if (idleTimer >= idleThreshold) isIdleLongTime = true;
        }
        else
        {
            idleTimer = 0f;
            isIdleLongTime = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}