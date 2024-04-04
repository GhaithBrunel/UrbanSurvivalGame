using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hovered;
    public Item heldItem;

    private Color opaque = new Color(1, 1, 1, 1);
    private Color transparent = new Color(1, 1, 1, 0);

    private Image thisSlotImage;
    public TMP_Text thisSlotQuantityText;

    public void initialiseSlot()
    {
        thisSlotImage = gameObject.GetComponent<Image>();
        thisSlotQuantityText = transform.GetChild(0).GetComponent<TMP_Text>();
        thisSlotImage.sprite = null;
        thisSlotImage.color = transparent;
        setItem(null);
    }

    public void setItem(Item item)
    {
        heldItem = item; // Assign the item to the heldItem variable.

        if (item != null)
        // If there's an item, update the slot's image and make it opaque. Otherwise, clear the slot.
        {
            thisSlotImage.sprite = heldItem.icon;
            thisSlotImage.color = opaque;
            updateData();
        }
        else
        {
            thisSlotImage.sprite = null;
            thisSlotImage.color = transparent;
            updateData();
        }
    }

    public Item getItem()
    {
        return heldItem;
    }

    public bool hasItem() // checks if the slot has an iterm 
    {
        return heldItem ? true : false;
    }

    public void updateData()
    // If there's an item, display its quantity. Otherwise, clear the text.
    {
        if (heldItem != null)
            thisSlotQuantityText.text = heldItem.currentQuantity.ToString();
        else
            thisSlotQuantityText.text = "";
    }

    //uses the mouse to however over inventory 
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        hovered = false;
    }
}