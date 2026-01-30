using UnityEngine;

public class Camera_Follower : MonoBehaviour
{
    public GameObject Camera_target;
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


        }
    }
}
