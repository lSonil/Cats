using UnityEngine;

public class Lock : MonoBehaviour
{
    public int ItemID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player touched the door.");

            PlayerInventory inv = collision.gameObject.GetComponentInChildren<PlayerInventory>();
            if (inv == null) inv = collision.gameObject.GetComponentInParent<PlayerInventory>();

            if (inv != null && inv.currentHeldItem != null)
            {
                if (inv.currentHeldItem.itemId == ItemID)
                {
                    Debug.Log("Correct ID. Victory!");
                    GameManager.Instance.Victory();
                }
                else
                {
                    Debug.Log("Wrong ID! You have: " + inv.currentHeldItem.itemId + " but we need: " + ItemID);
                }
            }
            else
            {
                Debug.Log("Inventory not found!");
            }
        }
    }
}