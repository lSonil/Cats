using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Player_Camera : MonoBehaviour
{
    public GameObject Camera_Target;
    public Camera MainCamera;
    public float smooth_time = 0.25f;
    public Vector2 Camera_Velocity = new Vector2(1, 1);

    public float Cam_Right_Limiter;
    public float Cam_Left_Limiter;
    public float Cam_Top_Limiter;
    public float Cam_Bottom_Limiter;

    [SerializeField] Vector3 target_position;
    [SerializeField] Vector3 target_position_Original;
    public Vector3 true_position;


    void Start()
    {
        if (MainCamera == null)
        {
            MainCamera = Camera.main;
            if (MainCamera == null)
            {
                Debug.LogError("Player_Camera: MainCamera not found!");
            }
        }
    }
    void Update()
    {
        if (Camera_Target == null)
        {
            try
            {
                Transform parent = transform.parent;
                if (parent != null)
                {
                    Transform siblingTarget = parent.Find("Camera_Target");
                    if (siblingTarget != null)
                    {
                        Camera_Target = siblingTarget.gameObject;
                    }
                    else
                    {
                        Debug.LogError("Player_Camera: No sibling named 'Camera_Target' found under the same parent.");
                    }
                }
                else
                {
                    Debug.LogError("Player_Camera: No parent found to search for 'Camera_Target' sibling.");
                }
            }
            catch
            {
                return;
            }
        }

        if (Camera_Target != null)
        {
            target_position_Original = Camera_Target.transform.position;
            target_position = Camera_Target.transform.position;

            // Apply border limits to target position
            target_position.x = Mathf.Clamp(target_position.x, Cam_Left_Limiter, Cam_Right_Limiter);
            target_position.y = Mathf.Clamp(target_position.y, Cam_Bottom_Limiter, Cam_Top_Limiter);

            // Smooth damp towards the limited target position
            this.transform.position = Vector2.SmoothDamp(this.transform.position, target_position, ref Camera_Velocity, smooth_time);
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);

            true_position = this.transform.position;
        }
    }
}
