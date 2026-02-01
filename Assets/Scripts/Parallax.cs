using UnityEngine;
using UnityEngine.InputSystem;

public class Parallax : MonoBehaviour
{
    [SerializeField] private InputActionReference moveAction; // Drag your 'Move' action here
    [SerializeField] private float intensity = 0.5f;
    [SerializeField] private float smoothTime = 0.2f;

    private Vector3 startPosition;
    private Vector3 currentVelocity;

    private void Start() => startPosition = transform.position;

    private void Update()
    {
        // Directly read the input value every frame
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        // Calculate target (Inverse it with - for a natural parallax feel)
        Vector3 targetPos = startPosition + new Vector3(input.x * -intensity, 0, 0);

        // Move smoothly
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, smoothTime);
    }
}