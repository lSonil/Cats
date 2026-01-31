using UnityEngine;

public class Item_Text_Positioner : MonoBehaviour
{
    
    public GameObject Target_Position;
   

    void LateUpdate()
    {
        this.transform.position = new Vector3(Target_Position.transform.position.x, Target_Position.transform.position.y+1.5f,10);
        this.transform.rotation = new Quaternion(0, 0, 0, 1);
    }
}
