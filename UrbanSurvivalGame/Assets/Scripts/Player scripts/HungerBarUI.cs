using UnityEngine;
using UnityEngine.UI;

public class HungerBarUI : MonoBehaviour
{
    public PlayerMovement playerMovement;
    private Slider hungerBarSlider;

    void Start()
    {
        hungerBarSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (playerMovement != null)
        {
            hungerBarSlider.value = playerMovement.GetCurrentHunger() / playerMovement.GetMaxHunger();
        }
    }
}
