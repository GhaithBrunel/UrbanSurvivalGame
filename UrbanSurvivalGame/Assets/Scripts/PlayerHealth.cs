    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.AI;

public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;
        [SerializeField] private Transform spawnPoint;
        public GameObject redScreenEffect;
        [SerializeField] private PlayerMovement playerMovement;
        // Array to hold different blood images
        public Image[] bloodSplatterImages;

    [SerializeField] private float maxRespawnDistance = 500f;
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
                int index = Random.Range(0, bloodSplatterImages.Length);
                var selectedImage = bloodSplatterImages[index];
                selectedImage.gameObject.SetActive(true);
                selectedImage.color = new Color(selectedImage.color.r, selectedImage.color.g, selectedImage.color.b, 1); 
                StartCoroutine(FadeOutBloodSplatter(selectedImage, 2f)); 
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
            yield return new WaitForSeconds(1); // Wait  1 second

            redScreenEffect.SetActive(false); 
            RespawnPlayer(); 
        }

        private void RespawnPlayer()
        {
            currentHealth = maxHealth;
           

        Vector3 respawnPosition = FindRandomNavMeshLocation(maxRespawnDistance);
        transform.position = respawnPosition;
        ResetPlayerStats();

            // Disable and re-enable CharacterController before moving the player
            CharacterController characterController = GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
              

                characterController.enabled = true;
            }
        }


    private Vector3 FindRandomNavMeshLocation(float maxDistance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * maxDistance + transform.position;
        NavMeshHit hit;
        for (int i = 0; i < 30; i++) // Attempt multiple times to find a valid position
        {
            if (NavMesh.SamplePosition(randomDirection, out hit, maxDistance, NavMesh.AllAreas))
            {
                return hit.position; // Return the position if a valid point is found
            }
        }
        return transform.position; // Fallback to the current position if no valid point is found
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
