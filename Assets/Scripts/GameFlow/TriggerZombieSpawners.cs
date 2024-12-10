using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZombieSpawners : MonoBehaviour
{
    public bool hasTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            // Add 1 coin to the player's inventory
            InventoryManager.Instance.AddToInventory("Coin", 1);

            // Debug message for confirmation
            Debug.Log("Player entered trigger zone. Coin added to inventory.");
        }
    }
}