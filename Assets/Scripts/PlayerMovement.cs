using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    [Header("Jump Charge Settings")]
    public float minJump = 10f; // Set higher so tap jump feels real
    public float maxJump = 25f;

    [Header("Detection Settings")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isGrounded;
    private float jumpChargeTimer;
    private bool isCharging;

    [Header("Idler animation")]
    public float idleThreshold = 5.0f; // Time in seconds before becoming "bored"
    private float idleTimer = 0f;
    public bool isIdleLongTime = false; // The flag

    void Start() => rb = GetComponent<Rigidbody2D>();
    public bool Grounded() => isGrounded;
    public bool Gharging() => isCharging;
    public bool Idle() => isIdleLongTime;
    public bool Moving() => moveInput!=Vector2.zero;
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    // This function runs on BOTH press and release if Action Type is "Pass Through"
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (isGrounded)
            {
                isCharging = true;
                jumpChargeTimer = 0f;
            }
        }
        else // This is the "Key Unpress"
        {
            if (isCharging)
            {
                PerformJump();
            }
        }
    }

    private void PerformJump()
    {
        float chargePercent = Mathf.Clamp01(jumpChargeTimer);
        float finalJumpForce = Mathf.Lerp(minJump, maxJump, chargePercent);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, finalJumpForce);

        isCharging = false;
        jumpChargeTimer = 0f;
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        if (isCharging)
        {
            jumpChargeTimer += Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        if (moveInput.x != 0)
        {
            GetComponent<SpriteRenderer>().flipX = (moveInput.x < 0);
        }

        if (moveInput.x == 0)
        {
            idleTimer += Time.fixedDeltaTime;

            if (idleTimer >= idleThreshold)
            {
                isIdleLongTime = true;
            }
        }
        else
        {
            idleTimer = 0f;
            isIdleLongTime = false;
        }
    }
}