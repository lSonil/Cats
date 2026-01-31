using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public List<Item> nearbyItems = new List<Item>();
    public Item currentHeldItem = null;
    private Item closest = null;

    [Header("Throw Settings")]
    public float throwForce = 5f;
    public float holdThreshold = 0.3f;

    private float interactPressTime = -1f;
    private bool interactWasPressed = false;
    private InputAction interactAction;

    void Start()
    {
        // Get reference to the Interact action
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            interactAction = playerInput.actions.FindAction("Interact");
        }
    }

    void Update()
    {
        UpdateClosest();
        CheckInteractRelease();
    }

    void CheckInteractRelease()
    {
        if (interactAction == null)
            return;

        bool isCurrentlyPressed = interactAction.IsPressed();

        // Detect button press start
        if (isCurrentlyPressed && !interactWasPressed)
        {
            interactPressTime = Time.time;
            interactWasPressed = true;
        }
        // Detect button release
        else if (!isCurrentlyPressed && interactWasPressed)
        {
            interactWasPressed = false;
            float pressDuration = Time.time - interactPressTime;
            bool isHold = pressDuration >= holdThreshold;

            HandleInteraction(isHold);
        }
    }

    private void HandleInteraction(bool isHold)
    {
        // Case 1: No item held + near pickable item -> Pick up
        if (currentHeldItem == null && closest != null)
        {
            PickUpClosest();
        }
        // Case 2: Holding item + not near pickable item -> Drop (short) or Throw (hold)
        else if (currentHeldItem != null && closest == null)
        {
            if (isHold)
            {
                ThrowItem();
            }
            else
            {
                DropItem();
            }
        }
        // Case 3: Holding item + near pickable item -> Swap (short) or Throw (hold)
        else if (currentHeldItem != null && closest != null)
        {
            if (isHold)
            {
                ThrowItem();
            }
            else
            {
                SwapItems();
            }
        }
        
        if (currentHeldItem != null && currentHeldItem.isTimed)
        {
            currentHeldItem.timeRemaining -= Time.deltaTime;

            if (currentHeldItem.timeRemaining <= 0)
            {
                DropAndDestroyItem();
            }
        }
    }

    void UpdateClosest()
    {
        nearbyItems.RemoveAll(i => i == null);

        if (nearbyItems.Count > 0)
        {
            nearbyItems = nearbyItems
                .OrderBy(i => Vector2.SqrMagnitude(transform.position - i.transform.position))
                .ToList();

            Item newClosest = nearbyItems[0];

            if (newClosest != closest)
            {
                if (closest != null) closest.SetGrab(false);
                closest = newClosest;
                closest.SetGrab(true);
            }
        }
        else
        {
            if (closest != null) closest.SetGrab(false);
            closest = null;
        }
    }

    void PickUpClosest()
    {
        Item toRememberClosest = closest;
        if (currentHeldItem != null)
        {
            DropItem();
        }
        currentHeldItem = toRememberClosest;
        currentHeldItem.PickUp(transform);

        nearbyItems.Remove(currentHeldItem);
        currentHeldItem.SetGrab(false);
        closest = null;
    }

    void SwapItems()
    {
        // Drop current item and pick up the new one
        Item itemToPickUp = closest;
        DropItem();
        currentHeldItem = itemToPickUp;
        currentHeldItem.PickUp(transform);

        nearbyItems.Remove(currentHeldItem);
        currentHeldItem.SetGrab(false);
        closest = null;
    }

    void DropItem()
    {
        currentHeldItem.Drop();
        currentHeldItem = null;
    }
    void DropAndDestroyItem()
    {
        currentHeldItem.Drop();
        GameObject gj = currentHeldItem.gameObject;
        currentHeldItem = null;
        Destroy(gj);
    }

    void ThrowItem()
    {
        if (currentHeldItem == null)
        {
            return;
        }

        Item itemToThrow = currentHeldItem;
        itemToThrow.gameObject.SetActive(true);
        itemToThrow.transform.position = transform.position;
        currentHeldItem = null;

        Debug.Log("Throw Triggered");

        Rigidbody2D rb = itemToThrow.GetComponent<Rigidbody2D>();
        Vector2 throwDirection = new Vector2(1f, 0.7f).normalized;
        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if (item != null && !nearbyItems.Contains(item))
            {
                nearbyItems.Add(item);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                item.SetGrab(false);
                nearbyItems.Remove(item);
            }
        }
    }
}