using UnityEngine;

public class Lock : MonoBehaviour
{
    public int ItemID; // ID-ul necesar

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Juc?torul a atins u?a!");

            // C?ut?m inventarul pe orice parte a juc?torului
            PlayerInventory inv = collision.gameObject.GetComponentInChildren<PlayerInventory>();
            if (inv == null) inv = collision.gameObject.GetComponentInParent<PlayerInventory>();

            if (inv != null && inv.currentHeldItem != null)
            {
                if (inv.currentHeldItem.itemId == ItemID)
                {
                    Debug.Log("ID Corect! Declan??m Victory.");
                    GameManager.Instance.Victory();
                }
                else
                {
                    Debug.Log("ID Gresit! Ai: " + inv.currentHeldItem.itemId + " dar trebuie: " + ItemID);
                }
            }
            else
            {
                Debug.Log("Inventar neg?sit sau mâna e goal?!");
            }
        }
    }
}