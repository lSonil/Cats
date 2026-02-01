using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Player_Camera : MonoBehaviour
{
    public GameObject Camera_Target;
    // public GameObject Smoothing_Input_UI;
    // private TMP_InputField Smoothing_Input_Field;
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
        //     Smoothing_Input_UI = GameObject.FindGameObjectWithTag("UI_Cam_Smooth");
        //     Smoothing_Input_Field = Smoothing_Input_UI.GetComponent<TMP_InputField>();
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
        else
        {
            //  transform.position = Vector2.SmoothDamp(transform.position, Camera_target.transform.position, ref Camera_Velocity, smooth_time);

            try
            {
                //     smooth_time = float.Parse(Smoothing_Input_Field.text);
            }
            catch
            {
                smooth_time = 0.25f;
            }

            //  smooth_time = float.Parse(Smoothing_Input_Field.text);
            target_position_Original = Camera_Target.transform.position;
            target_position = Camera_Target.transform.position;

            transform.position = Vector2.SmoothDamp(transform.position, target_position, ref Camera_Velocity, smooth_time);

            true_position = this.transform.position;

     




        }

       
    }

}
