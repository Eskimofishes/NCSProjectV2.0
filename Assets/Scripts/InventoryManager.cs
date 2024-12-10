using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InventoryManager : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string name;
        public int quantity;

        public Item(string name, int quantity)
        {
            this.name = name;
            this.quantity = quantity;
        }
    }

    public static InventoryManager Instance { get; private set; }
    public List<Item> items = new List<Item>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Add an item to the inventory
    public void AddToInventory(string name, int quantity = 1)
    {
        var existingItem = items.Find(item => item.name == name);
        if (existingItem != null)
        {
            existingItem.quantity += quantity;
        }
        else
        {
            items.Add(new Item(name, quantity));
        }

        Debug.Log($"Added {quantity} {name}(s) to inventory.");
        QuestManager.Instance.RefreshTrackerList();
    }

    // Remove an item from the inventory
    public bool RemoveFromInventory(string name, int quantity = 1)
    {
        var existingItem = items.Find(item => item.name == name);
        if (existingItem != null)
        {
            if (existingItem.quantity >= quantity)
            {
                existingItem.quantity -= quantity;
                if (existingItem.quantity == 0)
                {
                    items.Remove(existingItem);
                }
                Debug.Log($"Removed {quantity} {name}(s) from inventory.");
                return true;
            }
            else
            {
                Debug.LogWarning($"Not enough {name}(s) to remove.");
                return false;
            }
        }
        Debug.LogWarning($"{name} is not in the inventory.");
        QuestManager.Instance.RefreshTrackerList();
        return false;
    }

    // Check quantity of a specific item
    public int GetItemQuantity(string name)
    {
        var existingItem = items.Find(item => item.name == name);
        return existingItem != null ? existingItem.quantity : 0;
    }
}