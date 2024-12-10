using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTracker : MonoBehaviour
{
    [SerializeField] private MoveThePriest moveThePriest; // Reference to MoveThePriest script
    private int previousCoinCount = 0; // Tracks the last known coin count
    public GameObject[] zombieSpawners;

    public DayNightSystem dayNightSystem;

    [SerializeField] public int damageToZombies = 400;

    private void Start()
    {
        dayNightSystem.setTime(12);
    }

    private void Update()
    {
        // Get the current coin count from the inventory
        int currentCoinCount = InventoryManager.Instance.GetItemQuantity("Coin");
        int killedZombies = InventoryManager.Instance.GetItemQuantity("Zombie");
        // Check if coin count has increased
        if (currentCoinCount > previousCoinCount)
        {
            // switch(currentCoinCount)
            // {
            //     case(1)
            // }
            if (currentCoinCount == 1)
            {
                moveThePriest.MoveToFirstLocation();
            }
            else if (currentCoinCount == 2)
            {
                moveThePriest.MoveToSecondLocation();
            }
            else if (currentCoinCount == 3)
            {
                if ((dayNightSystem.currentHour > 20 && dayNightSystem.currentHour < 24) || 
                    (dayNightSystem.currentHour >= 0 && dayNightSystem.currentHour < 5))
                {
                }
                else
                {
                    dayNightSystem.setTime(20);
                }
                TurnOnZombieSpawners();
            }
            else if (currentCoinCount == 4)
            {
                if ((dayNightSystem.currentHour >= 20 && dayNightSystem.currentHour < 24) || (dayNightSystem.currentHour >= 0 && dayNightSystem.currentHour < 5))
                {
                    dayNightSystem.setTime(5);                           
                }
                killAllZombies();
                TurnOffZombieSpawners();
            }
            else if(currentCoinCount == 5)
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.victorySound);
            }

            // Update the previous coin count
            previousCoinCount = currentCoinCount;
        }

        if(killedZombies >= 30)
        {
            InventoryManager.Instance.AddToInventory("Coin", 1);
        }
    }

    private void TurnOnZombieSpawners()
    {
        foreach (GameObject spawner in zombieSpawners)
        {
            if (spawner != null)
            {
                spawner.SetActive(true); // Enable the ZombieSpawner GameObjects
            }
        }

        Debug.Log("Zombie Spawners activated!");
    }
    private void TurnOffZombieSpawners()
    {
        foreach (GameObject spawner in zombieSpawners)
        {
            if (spawner != null)
            {
                spawner.SetActive(false); // disable the ZombieSpawner GameObjects
            }
        }

        Debug.Log("Zombie Spawners disabled!");
    }

    public void killAllZombies()
    {
        // Find all GameObjects with the tag "Zombie"
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");

        // Debug log the number of zombies found
        Debug.Log($"Found {zombies.Length} zombies in the scene.");

        // Loop through each zombie and reduce its health
        foreach (GameObject zombie in zombies)
        {
            // Try to get a health component from the zombie
            Enemy enemyComponent = zombie.GetComponent<Enemy>();

            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(damageToZombies);
            }
        }
    }
}
