using UnityEngine;

public class Camera_Follower : MonoBehaviour
{
    public GameObject Camera_target;

    public float Cam_Right_Limiter;
    public float Cam_Left_Limiter;
    public float Cam_Top_Limiter;
    public float Cam_Bottom_Limiter;
    public Vector3 debug_position;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
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

            this.transform.position = new Vector3(Camera_target.transform.position.x, Camera_target.transform.position.y,-10);

             Cam_Right_Limiter = Camera_target.GetComponent<Player_Camera>().Cam_Right_Limiter;
             Cam_Left_Limiter = Camera_target.GetComponent<Player_Camera>().Cam_Left_Limiter;
             Cam_Top_Limiter = Camera_target.GetComponent<Player_Camera>().Cam_Top_Limiter;
             Cam_Bottom_Limiter = Camera_target.GetComponent<Player_Camera>().Cam_Bottom_Limiter;


            this.transform.position = Camera_target.GetComponent<Player_Camera>().true_position;
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10);
            debug_position = this.transform.position;
            if (this.transform.position.x > Cam_Right_Limiter) this.transform.position = new Vector3(Cam_Right_Limiter, this.transform.position.y, -10);
            if (this.transform.position.x < Cam_Left_Limiter) this.transform.position = new Vector3(Cam_Left_Limiter, this.transform.position.y, -10);
            if (this.transform.position.y > Cam_Top_Limiter) this.transform.position = new Vector3(this.transform.position.x, Cam_Top_Limiter, -10);
            if (this.transform.position.y < Cam_Bottom_Limiter) this.transform.position = new Vector3(this.transform.position.x, Cam_Bottom_Limiter, -10);
           



        }
    }
}
