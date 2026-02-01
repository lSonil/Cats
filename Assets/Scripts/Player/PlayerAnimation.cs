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
        if (usedItem != pi.currentHeldItem)
        {
            usedItem = pi.currentHeldItem;
            print("   " + usedItem);

            ar.SetFloat("itemId", usedItem == null ? 0 : usedItem.itemId);
        }
        ar.SetBool("isMove", pm.Moving());
        ar.SetBool("prepJump", pm.HoldingJump());
        ar.SetFloat("jumpCharge", pm.GetJumpChargeProgress());
        ar.SetBool("inAir", pm.ShouldShowInAir());
        ar.SetBool("isIdle", pm.Idle());
        ar.SetBool("isFalling", pm.Falling());

        // Dynamically control Jump animation time based on charge progress
        if (pm.HoldingJump())
        {
            AnimatorStateInfo stateInfo = ar.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Jump"))
            {
                ar.Play("Jump", 0, pm.GetJumpChargeProgress());
            }
        }
    }
}
