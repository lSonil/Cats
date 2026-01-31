using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    PlayerInventory pi;
    PlayerMovement pm;
    Animator ar;
    Item usedItem;
    private void Start()
    {
        ar = GetComponent<Animator>();
        pi = GetComponent<PlayerInventory>();
        pm = GetComponent<PlayerMovement>();
        usedItem = pi.currentHeldItem;
    }

    private void Update()
    {
        if(usedItem != pi.currentHeldItem)
        {
            usedItem = pi.currentHeldItem;
            print("   "+usedItem);

            ar.SetFloat("itemId", usedItem == null?0:usedItem.itemId);
        }
        ar.SetBool("isMove",pm.Moving());
        ar.SetBool("prepJump",pm.Gharging());
        ar.SetBool("inAir",!pm.Grounded());
        ar.SetBool("isIdle", pm.Idle());
        ar.SetBool("isFalling", pm.Falling());
    }
}
