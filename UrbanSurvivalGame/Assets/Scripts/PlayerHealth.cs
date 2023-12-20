using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// new

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private Transform spawnPoint; // Assign this in the inspector

    public GameObject redScreenEffect;

    void Start()
    {
        currentHealth = maxHealth;
        redScreenEffect.SetActive(false);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {
        redScreenEffect.SetActive(true); // Activate red screen effect
        yield return new WaitForSeconds(1); // Wait for 2 seconds

        redScreenEffect.SetActive(false); // Deactivate red screen effect
        RespawnPlayer(); // Respawn the player
    }



    private void RespawnPlayer()
    {
        currentHealth = maxHealth;
        transform.position = spawnPoint.position; // Reset player's position to the spawn point
                                                  // You can also reset other states or variables related to the player here


        // Disable CharacterController before moving the player
        CharacterController characterController = GetComponent<CharacterController>();
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        // Reposition the player
        transform.position = spawnPoint.position;

        // Re-enable CharacterController
        if (characterController != null)
        {
            characterController.enabled = true;
        }



    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // Similar to TakeDamage, you can trigger healing effects or behaviors
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void TakeFallDamage(float fallDistance)
    {
        float damageAmount = CalculateFallDamage(fallDistance);
        Debug.Log($"Taking fall damage: {damageAmount}");
        TakeDamage(damageAmount);
    }

    private float CalculateFallDamage(float fallDistance)
    {
        float damage = Mathf.Max(0, fallDistance - 10); // Example calculation
        Debug.Log($"Calculated fall damage: {damage} for fall distance: {fallDistance}");
        return damage;
    }



}