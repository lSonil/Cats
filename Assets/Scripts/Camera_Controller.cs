using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public float Cam_X_Multiplier;
    public float Cam_Y_Multiplier;
    public GameObject Camera_Target;

    void Start()
    {

    }


    void Update()
    {
           float xInput = Input.GetAxisRaw("Horizontal");
           float yInput = Input.GetAxisRaw("Vertical");
           Vector2 Camerainput = new Vector2(xInput, yInput);
           Vector2 direction = Camerainput.sqrMagnitude > 0.01f ? Camerainput.normalized : Vector2.zero;

          Camera_Target.transform.position = this.transform.position + new Vector3(direction.x * Cam_X_Multiplier, direction.y * Cam_Y_Multiplier, 0f);

    }
}

