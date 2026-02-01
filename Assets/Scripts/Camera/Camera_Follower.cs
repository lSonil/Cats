using UnityEngine;

public class Camera_Follower : MonoBehaviour
{
    public GameObject Camera_target;
    private Camera mainCamera;

    public float Cam_Right_Limiter;
    public float Cam_Left_Limiter;
    public float Cam_Top_Limiter;
    public float Cam_Bottom_Limiter;
    public Vector3 debug_position;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Camera_target == null)
        {
            try
            {
                Camera_target = GameObject.FindGameObjectWithTag("Player_Camera_Position");
            }
            catch
            {
                return;
            }
        }
        else
        {
            Cam_Right_Limiter = Camera_target.GetComponent<Player_Camera>().Cam_Right_Limiter;
            Cam_Left_Limiter = Camera_target.GetComponent<Player_Camera>().Cam_Left_Limiter;
            Cam_Top_Limiter = Camera_target.GetComponent<Player_Camera>().Cam_Top_Limiter;
            Cam_Bottom_Limiter = Camera_target.GetComponent<Player_Camera>().Cam_Bottom_Limiter;

            Vector3 targetPos = Camera_target.GetComponent<Player_Camera>().true_position;

            // Apply border limits
            targetPos.x = Mathf.Clamp(targetPos.x, Cam_Left_Limiter, Cam_Right_Limiter);
            targetPos.y = Mathf.Clamp(targetPos.y, Cam_Bottom_Limiter, Cam_Top_Limiter);
            targetPos.z = -10;

            this.transform.position = targetPos;
            debug_position = this.transform.position;
        }
    }
}
