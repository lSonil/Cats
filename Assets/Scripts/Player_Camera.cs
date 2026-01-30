using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Player_Camera : MonoBehaviour
{
    public GameObject Camera_target;
   // public GameObject Smoothing_Input_UI;
   // private TMP_InputField Smoothing_Input_Field;
    public float smooth_time ;
    public Vector2 Camera_Velocity = new Vector2(1, 1);

    void Start()
    {
   //     Smoothing_Input_UI = GameObject.FindGameObjectWithTag("UI_Cam_Smooth");
   //     Smoothing_Input_Field = Smoothing_Input_UI.GetComponent<TMP_InputField>();
    }
    void Update()
    { if (Camera_target == null)
        { try
            {
                Camera_target = GameObject.FindGameObjectWithTag("Camera_Target");
                
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

            transform.position = Vector2.SmoothDamp(transform.position, Camera_target.transform.position, ref Camera_Velocity, smooth_time);
        }
    }
}