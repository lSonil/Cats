using UnityEngine;
using System.Collections;

public class ChihuahuaScare : MonoBehaviour
{
    [Header("Detection Settings")]
    public float triggerDistance = 2f;    // How close the player needs to be
    public float scareCooldown = 3f;      // Time between scares

    [Header("Knockback Settings")]
    public float knockbackStrength = 15f; // Power of the push

    [Header("Visual & Audio Effects")]
    public GameObject jumpscareUI;        // Fullscreen image of the angry chihuahua
    public AudioSource barkSound;         // Audio component with the bark clip
    public float uiDisplayTime = 0.5f;    // How long the image stays on screen

    private Transform playerTransform;
    private bool canScare = true;

    void Start()
    {
        // Find the player using the "Player" tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        // Ensure the jumpscare image is hidden at the start
        if (jumpscareUI != null) jumpscareUI.SetActive(false);
    }

    void Update()
    {
        // Don't do anything if player is missing or we are on cooldown
        if (playerTransform == null || !canScare) return;

        // Calculate the distance between the Chihuahua and the Player
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // If player is within range, trigger the scare
        if (distance < triggerDistance)
        {
            StartCoroutine(TriggerScare());
        }
    }

    IEnumerator TriggerScare()
    {
        canScare = false;
        Debug.Log("WOOF! WOOF! WOOF!");

        // 1. Play Bark Sound
        if (barkSound != null) barkSound.Play();

        // 2. Show Jumpscare Image
        if (jumpscareUI != null) jumpscareUI.SetActive(true);

        // 3. Apply Knockback Force
        ApplyKnockback();

        // Wait for a short duration then hide the image
        yield return new WaitForSeconds(uiDisplayTime);
        if (jumpscareUI != null) jumpscareUI.SetActive(false);

        // Wait for the cooldown before the dog can scare again
        yield return new WaitForSeconds(scareCooldown);
        canScare = true;
    }

    void ApplyKnockback()
    {
        Rigidbody2D playerRb = playerTransform.GetComponent<Rigidbody2D>();

        if (playerRb != null)
        {
            // Calculate direction from the dog to the player
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            // Apply the force as an Impulse (instantaneous shock)
            playerRb.AddForce(direction * knockbackStrength, ForceMode2D.Impulse);
        }
    }

    // Visualize the detection range in the Unity Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}