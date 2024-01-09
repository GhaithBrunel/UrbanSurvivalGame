using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{

    [Header("Player")]
    public PlayerMovement playerMovement;


    [Header("UI")]
    public GameObject inventory;
    private List<Slot> allInventorySlots = new List<Slot>();
    public List<Slot> inventorySlots = new List<Slot>();
    public List<Slot> hotbarSlots = new List<Slot>();
    public Image crosshair;
    public TMP_Text itemHoverText;

    [Header("Raycast")]
    public float raycastDistance = 5f;
    public LayerMask itemLayer;
    public Transform dropLocation; // The location items will be dropped from.

    [Header("Drag and Drop")]
    public Image dragIconImage;
    private Item currentDraggedItem;
    private int currentDragSlotIndex = -1;

    [Header("Equippable Items")]
    public List<GameObject> equippableItems = new List<GameObject>();
    public Transform selectedItemImage;

    [Header("Crafting")]
    public List<Recipe> itemRecipes = new List<Recipe>();

    public void Start()
    {
        toggleInventory(false);

        allInventorySlots.AddRange(hotbarSlots);
        allInventorySlots.AddRange(inventorySlots);

        foreach (Slot uiSlot in allInventorySlots)
        {
            uiSlot.initialiseSlot();
        }
    }

    public void Update()
    {
        itemRaycast(Input.GetKeyDown(KeyCode.E));

        if (Input.GetKeyDown(KeyCode.Tab))
            toggleInventory(!inventory.activeInHierarchy);

        if (inventory.activeInHierarchy && Input.GetMouseButtonDown(0))
        {
            dragInventoryIcon();
        }
        else if (currentDragSlotIndex != -1 && Input.GetMouseButtonUp(0) || currentDragSlotIndex != -1 && !inventory.activeInHierarchy) // If we are hovered over a slot and release, if we are dragging an item and close the inventory
        {
            dropInventoryIcon();
        }

        if (Input.GetKeyDown(KeyCode.Q)) // The button we need to press to drop items from the inventory
            dropItem();

        for (int i = 1; i < hotbarSlots.Count + 1; i++)
        {
            if (Input.GetKeyDown(i.ToString()))
            {
                enableHotbarItem(i - 1);
            }
        }

        dragIconImage.transform.position = Input.mousePosition;
    }

    private void itemRaycast(bool hasClicked = false)
    {
        itemHoverText.text = "";
        Ray ray = Camera.main.ScreenPointToRay(crosshair.transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, itemLayer))
        {
            if (hit.collider != null)
            {
                if (hasClicked) // Pick up
                {
                    Item newItem = hit.collider.GetComponent<Item>();
                    if (newItem)
                    {
                        addItemToInventory(newItem);
                    }
                }
                else // Get the name
                {
                    Item newItem = hit.collider.GetComponent<Item>();

                    if (newItem)
                    {
                        itemHoverText.text = newItem.name;
                    }
                }
            }
        }
    }

    private void addItemToInventory(Item itemToAdd)
    {
        int leftoverQuantity = itemToAdd.currentQuantity;
        Slot openSlot = null;
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Item heldItem = allInventorySlots[i].getItem();

            if (heldItem != null && itemToAdd.name == heldItem.name)
            {
                int freeSpaceInSlot = heldItem.maxQuantity - heldItem.currentQuantity;

                if (freeSpaceInSlot >= leftoverQuantity)
                {
                    heldItem.currentQuantity += leftoverQuantity;
                    Destroy(itemToAdd.gameObject);
                    allInventorySlots[i].updateData();
                    return;
                }
                else // Add as much as we can to the current slot
                {
                    heldItem.currentQuantity = heldItem.maxQuantity;
                    leftoverQuantity -= freeSpaceInSlot;
                }
            }
            else if (heldItem == null)
            {
                if (!openSlot)
                    openSlot = allInventorySlots[i];
            }

            allInventorySlots[i].updateData();
        }

        if (leftoverQuantity > 0 && openSlot)
        {
            openSlot.setItem(itemToAdd);
            itemToAdd.currentQuantity = leftoverQuantity;
            itemToAdd.gameObject.SetActive(false);
        }
        else
        {
            itemToAdd.currentQuantity = leftoverQuantity;
        }
    }

    private void toggleInventory(bool enable)
    {
        inventory.SetActive(enable);

        Cursor.lockState = enable ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = enable;

        // Disable or enable player movement and camera control
        if (playerMovement != null)
        {
            playerMovement.SetPlayerMovement(!enable); // Disable movement when inventory is open
            playerMovement.SetCameraMovement(!enable); // Disable camera movement when inventory is open
            playerMovement.SetCursorState(enable);     // Set cursor state based on inventory
        }
    }


    private void dropItem()
    {
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];
            if (curSlot.hovered && curSlot.hasItem())
            {
                curSlot.getItem().gameObject.SetActive(true);
                curSlot.getItem().transform.position = dropLocation.position;
                curSlot.setItem(null);
                break;
            }
        }
    }

    private void dragInventoryIcon()
    {
        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];
            if (curSlot.hovered && curSlot.hasItem())
            {
                currentDragSlotIndex = i; // Update the current drag slot index variable.

                currentDraggedItem = curSlot.getItem(); // Get the item from the current slot
                dragIconImage.sprite = currentDraggedItem.icon;
                dragIconImage.color = new Color(1, 1, 1, 1); // Make the follow mouse icon opaque (visible).

                curSlot.setItem(null); // Remove the item from the slot we just picked up the item from.
            }
        }
    }

    private void dropInventoryIcon()
    {
        // Reset our drag item variables
        dragIconImage.sprite = null;
        dragIconImage.color = new Color(1, 1, 1, 0); // Make invisible.

        for (int i = 0; i < allInventorySlots.Count; i++)
        {
            Slot curSlot = allInventorySlots[i];
            if (curSlot.hovered)
            {
                if (curSlot.hasItem()) // Swap the items.
                {
                    Item itemToSwap = curSlot.getItem();

                    curSlot.setItem(currentDraggedItem);

                    allInventorySlots[currentDragSlotIndex].setItem(itemToSwap);

                    resetDragVariables();
                    return;
                }
                else // Place the item with no swap.
                {
                    curSlot.setItem(currentDraggedItem);
                    resetDragVariables();
                    return;
                }
            }
        }

        // If we get to this point we dropped the item in an invalid location (or closed the inventory).
        allInventorySlots[currentDragSlotIndex].setItem(currentDraggedItem);
        resetDragVariables();
    }

    private void resetDragVariables()
    {
        currentDraggedItem = null;
        currentDragSlotIndex = -1;
    }
    // new one below
    private void enableHotbarItem(int hotbarIndex)
    {
        // Deactivate and reset all items
        foreach (GameObject item in equippableItems)
        {
            if (item.activeSelf)
            {
                // Call ResetWeapon on the active item
                Axe axeScript = item.GetComponent<Axe>();
                if (axeScript != null)
                {
                    axeScript.ResetWeapon();
                }
                // sword
                Sword swordScript = item.GetComponent<Sword>();
                if (swordScript != null)
                {
                    swordScript.ResetWeapon();
                }

                item.SetActive(false);
            }
        }

        // Activate the selected hotbar item
        Slot hotbarSlot = hotbarSlots[hotbarIndex];
        selectedItemImage.transform.position = hotbarSlot.transform.position;

        if (hotbarSlot.hasItem() && hotbarSlot.getItem().equippableItemIndex != -1)
        {
            GameObject selectedItem = equippableItems[hotbarSlot.getItem().equippableItemIndex];
            selectedItem.SetActive(true);
        }
    }




    public void EatRibs()
    {
        // Assuming each slot has a quantity and an identifier for the item type (like "Ribs")
        foreach (var slot in hotbarSlots)
        {
            if (slot.hasItem() && slot.getItem().name == "Ribs")
            {
                slot.getItem().currentQuantity -= 1; // Decrease the quantity of ribs
                slot.updateData(); // Update the slot UI

                if (slot.getItem().currentQuantity <= 0)
                {
                    // If no more ribs, disable them in the hotbar
                    slot.setItem(null);
                    DisableEquippedRibs();
                }
                break; // Exit the loop after processing the first rib stack found
            }
        }
    }

    private void DisableEquippedRibs()
    {
        // Logic to disable the ribs from the player's hand or hotbar
        foreach (GameObject item in equippableItems)
        {
            if (item.name == "Ribs")
            {
                item.SetActive(false);
                break;
            }
        }
    }
    public void craftItem(string itemName)
    {
        foreach (Recipe recipe in itemRecipes)
        {
            if (recipe.createdItemPrefab.GetComponent<Item>().name == itemName)
            {
                bool haveAllIngredients = true;
                for (int i = 0; i < recipe.requiredIngredients.Count; i++)
                {
                    if (haveAllIngredients)
                        haveAllIngredients = haveIngredient(recipe.requiredIngredients[i].itemName, recipe.requiredIngredients[i].requiredQuantity);
                }

                if (haveAllIngredients)
                {
                    for (int i = 0; i < recipe.requiredIngredients.Count; i++)
                    {
                        removeIngredient(recipe.requiredIngredients[i].itemName, recipe.requiredIngredients[i].requiredQuantity);
                    }

                    GameObject craftedItem = Instantiate(recipe.createdItemPrefab, dropLocation.position, Quaternion.identity);
                    craftedItem.GetComponent<Item>().currentQuantity = recipe.quantityProduced;

                    addItemToInventory(craftedItem.GetComponent<Item>());
                }

                break;
            }
        }
    }


    private void removeIngredient(string itemName, int quantity)
    {
        if (!haveIngredient(itemName, quantity))
            return;

        int remainingQuantity = quantity;

        foreach (Slot curSlot in allInventorySlots)
        {
            Item item = curSlot.getItem();

            if (item != null && item.name == itemName)
            {
                if (item.currentQuantity >= remainingQuantity)
                {
                    item.currentQuantity -= remainingQuantity;

                    if (item.currentQuantity == 0)
                    {
                        curSlot.setItem(null);
                        curSlot.updateData();
                    }

                    return;
                }
                else
                {
                    remainingQuantity -= item.currentQuantity;
                    curSlot.setItem(null);
                }
            }
        }
    }

    private bool haveIngredient(string itemName, int quantity)
    {
        int foundQuantity = 0;
        foreach (Slot curSlot in allInventorySlots)
        {
            if (curSlot.hasItem() && curSlot.getItem().name == itemName)
            {
                foundQuantity += curSlot.getItem().currentQuantity;

                if (foundQuantity >= quantity)
                    return true;
            }
        }

        return false;
    }


}