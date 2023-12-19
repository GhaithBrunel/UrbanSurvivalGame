using UnityEngine;
using UnityEngine.UI;
//new
public class HealthBarUI : MonoBehaviour
{
    public PlayerHealth playerHealth;
    private Slider healthBarSlider;

    void Start()
    {
        healthBarSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (playerHealth != null)
        {
            healthBarSlider.value = playerHealth.GetCurrentHealth() / playerHealth.GetMaxHealth();
        }
    }
}