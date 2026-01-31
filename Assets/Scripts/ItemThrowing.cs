using UnityEngine;

public class ItemThrowing : MonoBehaviour
{
    [Header("Throw Settings")]
    public float throwForce = 5f;


    [SerializeField]private Item Item_to_Throw ;
   // [SerializeField]private Item projectile_Instance;

    void Update()
    {
        Item_to_Throw = this.GetComponent<PlayerInventory>().currentHeldItem;

        if (Item_to_Throw!=null && Input.GetMouseButtonDown(0))
        {
            Item_to_Throw.gameObject.SetActive(true);
            Item_to_Throw.transform.position = this.transform.position;
            this.GetComponent<PlayerInventory>().currentHeldItem = null;


            Debug.Log("Throw Triggered");

            // if held down
            {
                Rigidbody2D rb = Item_to_Throw.GetComponent<Rigidbody2D>();
                Vector2 throwDirection = new Vector2(1f, 0.7f).normalized;
                rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
            }
        }
    }



}
