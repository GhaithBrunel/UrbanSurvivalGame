using UnityEngine;

public class Ribs : MonoBehaviour
{
    private Inventory inventory;

    void Start()
    {
        // Find the Inventory script in the scene
        inventory = FindObjectOfType<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Inventory script not found in the scene.");
        }
    }

    void Update()
    {
        // Check for left mouse click
        if (Input.GetMouseButtonDown(0))
        {
            EatRibs();
        }
    }

    private void EatRibs()
    {
        // Check if the player can eat the ribs
        if (inventory != null && inventory.playerMovement.GetCurrentHunger() < inventory.playerMovement.GetMaxHunger())
        {
            
            inventory.EatRibs();
            inventory.playerMovement.Eat(25f); // Update hunger with the increased amount
        }
    }

}

