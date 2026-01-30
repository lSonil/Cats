using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    PlayerInventory pi;
    Animator ar;
    Item usedItem;
    private void Start()
    {
        ar = GetComponent<Animator>();
        pi = GetComponent<PlayerInventory>();
        usedItem = pi.currentHeldItem;
    }

    private void Update()
    {
        if(usedItem != pi.currentHeldItem)
        {
            usedItem = pi.currentHeldItem;
            print("   "+usedItem);

            ar.SetFloat("ItemId", usedItem == null?0:usedItem.itemId);
        }
    }
}
