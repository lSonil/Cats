using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public float Cam_X_Multiplier = 0.25f;
    public float Cam_Y_Multiplier = 0.25f;
    public GameObject Camera_Target;
    public Rigidbody2D playerRigidbody;
    [Range(0.1f, 0.9f)]
    public float VerticalScreenAnchorFromBottom = 0.6666667f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
        }

        if (Camera_Target == null)
        {
            Transform child = transform.Find("Camera_Target");
            if (child != null)
            {
                Camera_Target = child.gameObject;
            }
            else
            {
                Debug.LogError("Camera_Controller: No child named 'Camera_Target' found. Please add one or assign Camera_Target manually.");
            }
        }
    }


    void Update()
    {
        if (playerRigidbody == null)
        {
            //Debug.LogWarning("Camera_Controller: playerRigidbody is not assigned!");
            return;
        }

        if (Camera_Target == null)
        {
            //Debug.LogWarning("Camera_Controller: Camera_Target is not assigned!");
            return;
        }

        Vector2 velocity = playerRigidbody.linearVelocity;
        Vector2 lookAhead = velocity.sqrMagnitude > 0.01f ? velocity : Vector2.zero;

        float verticalOffset = 0f;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null && mainCamera.orthographic)
        {
            float viewHeight = mainCamera.orthographicSize * 2f;
            float offsetFactor = 0.5f - VerticalScreenAnchorFromBottom;
            verticalOffset = viewHeight * offsetFactor;
        }

        // Debug.Log($"Velocity: {velocity}, LookAhead: {lookAhead}, SqrMag: {velocity.sqrMagnitude}");

        Camera_Target.transform.position = playerRigidbody.transform.position + new Vector3(
            lookAhead.x * Cam_X_Multiplier,
            lookAhead.y * Cam_Y_Multiplier - verticalOffset,
            0f);

    }
}

