using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<ItemData> inventoryItems;
    public string currentScene;

    

    public SaveData(List<ItemData> inventoryItems, string currentScene)
    
    {
        this.inventoryItems = inventoryItems;
        this.currentScene = currentScene;
    }
}

[Serializable]
public class ItemData
{
    public string itemName;
    public int quantity;

    


    public ItemData(string itemName, int quantity)
    {
        this.itemName = itemName;
        this.quantity = quantity;
    }
}

