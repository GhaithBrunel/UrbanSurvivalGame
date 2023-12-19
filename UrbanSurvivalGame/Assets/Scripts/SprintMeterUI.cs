using UnityEngine;
using UnityEngine.UI;
//new
public class SprintMeterUI : MonoBehaviour
{
    public PlayerMovement playerMovement; // Assign this in the inspector
    private Slider sprintMeterSlider;

    void Start()
    {
        sprintMeterSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (playerMovement != null)
        {
            sprintMeterSlider.value = playerMovement.GetCurrentSprintStamina() / playerMovement.GetMaxSprintStamina();
        }
    }
}
