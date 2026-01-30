using UnityEngine;
using UnityEngine.InputSystem; // Adăugăm această bibliotecă

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 8f;
    public float jumpForce = 14f;

    [Header("Detection Settings")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Vector2 moveInput; // Schimbat din float în Vector2 pentru noul sistem
    private bool isGrounded;
    private PlayerInput playerInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Fix pentru warning-ul despre project-wide actions asset
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.actions.Disable();
            playerInput.actions.FindActionMap("Player").Enable();
        }
    }

    // Această funcție va fi apelată automat de componenta Player Input
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Această funcție va fi apelată automat când apeși oricare tastă de Jump (Space/W/Up)
    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }

    void FixedUpdate()
    {
        // Aplicăm mișcarea pe axa X folosind moveInput.x
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }
}