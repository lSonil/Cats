using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<Item> nearbyItems = new List<Item>();
    public Item currentHeldItem = null;
    private Item closest = null;

    void Update()
    {
        UpdateClosest();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (closest != null)
            {
                PickUpClosest();
            }
            else if (currentHeldItem != null)
            {
                DropItem();
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

    void DropItem()
    {
        currentHeldItem.Drop();
        currentHeldItem = null;
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