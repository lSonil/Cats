using UnityEngine;
public class Item : MonoBehaviour
{
    public GameObject grabPrompt;
    public int itemId;

    [Header("Timer Settings")]
    public bool isTimed;
    public float timeRemaining = 10f;

    void Awake()
    {
        grabPrompt.SetActive(false);
    }

    public void PickUp(Transform holdRef)
    {
        transform.SetParent(holdRef);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        gameObject.SetActive(false);
    }

    public void Drop()
    {
        gameObject.SetActive(true);
        transform.SetParent(null);
    }

    public void SetGrab(bool value)
    {
        grabPrompt.SetActive(value);
    }
}