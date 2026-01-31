using Unity.VisualScripting;
using UnityEngine;

public class Lock : MonoBehaviour
{
    public GameObject objectToShow;
    public int ItemID;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Item item = collision.GetComponentInParent<PlayerInventory>().currentHeldItem;
            if (item != null)
            {
                if (ItemID == item.itemId) { 
                    if (objectToShow != null)
                        objectToShow .SetActive(true);
                    Destroy(this.gameObject);
                }
            }
        }
    }

    private void Start()
    {
        if (gameObject != null)
            gameObject.SetActive(false);
    }
}
