using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for UI components

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private Transform spawnPoint;
    public GameObject redScreenEffect;
    [SerializeField] private PlayerMovement playerMovement;
    // Array to hold different blood splatter Images
    public Image[] bloodSplatterImages;
    void Start()
    {
        currentHealth = maxHealth;
        redScreenEffect.SetActive(false);
        foreach (var image in bloodSplatterImages)
        {
            image.gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (bloodSplatterImages.Length > 0)
        {
            // Randomly select a blood splatter image and activate it
            int index = Random.Range(0, bloodSplatterImages.Length);
            var selectedImage = bloodSplatterImages[index];
            selectedImage.gameObject.SetActive(true);
            selectedImage.color = new Color(selectedImage.color.r, selectedImage.color.g, selectedImage.color.b, 1); // Ensure alpha is set to 1
            StartCoroutine(FadeOutBloodSplatter(selectedImage, 2f)); // Fades out the blood splatter after 2 seconds
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator FadeOutBloodSplatter(Image bloodSplatter, float duration)
    {
        Color originalColor = bloodSplatter.color;
        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            bloodSplatter.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(originalColor.a, 0, t));
            yield return null;
        }
        bloodSplatter.gameObject.SetActive(false);
    }


    private IEnumerator HandleDeath()
    {
        redScreenEffect.SetActive(true); // Activate red screen effect
        yield return new WaitForSeconds(1); // Wait for 1 second

        redScreenEffect.SetActive(false); // Deactivate red screen effect
        RespawnPlayer(); // Respawn the player
    }

    private void RespawnPlayer()
    {
        currentHealth = maxHealth;
        transform.position = spawnPoint.position;

        // Reset hunger and stamina
        ResetPlayerStats();

        // Disable and re-enable CharacterController before moving the player
        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
            transform.position = spawnPoint.position;
            characterController.enabled = true;
        }
    }

    private void ResetPlayerStats()
    {
        if (playerMovement != null)
        {
            playerMovement.ResetHungerAndStamina();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    // Fall damage related methods have been removed
}
