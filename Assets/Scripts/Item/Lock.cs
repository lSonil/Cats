using UnityEngine;

public class Lock : MonoBehaviour
{
    public int ItemID; // ID-ul necesar
    public GameObject objToAppear; // ID-ul necesar

    private void Start()
    {
        objToAppear.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory inv = collision.gameObject.GetComponentInChildren<PlayerInventory>();
            if (inv == null) inv = collision.gameObject.GetComponentInParent<PlayerInventory>();

            if (inv != null && inv.currentHeldItem != null)
            {
                if (inv.currentHeldItem.itemId == ItemID)
                {
                    if(objToAppear!=null)
                        objToAppear.SetActive(false);
                    Destroy(gameObject);
                }
            }
        }
    }
}