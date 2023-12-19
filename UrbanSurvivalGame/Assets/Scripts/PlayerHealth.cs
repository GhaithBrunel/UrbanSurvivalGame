using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// new

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // Here, you can also trigger any effects or behaviors that should happen when the player takes damage
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