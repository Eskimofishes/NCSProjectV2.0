using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    // Start is called before the first frame update
    public static PlayerState Instance {get; set;}

    // ---- Player Health ---- //
    public int currentHealth;
    public int maxHealth;
    [SerializeField] public int damage = 5;

    public Transform playerBody;

    private void Start()
    {
        currentHealth = maxHealth;
        if (playerBody == null)
        {
            playerBody = GameObject.FindWithTag("Player").transform; // This assumes the player GameObject has the "Player" tag
        }

    }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N)) // Health Bar Testing (Key N to gain Health)
        {
            currentHealth += 10; 
        }
        // if (Input.GetKeyDown(KeyCode.V))
        // {
        //     InventoryManager.Instance.AddToInventory("Coin", 1); //GameFlow Test
        // }
        // if (Input.GetKeyDown(KeyCode.B))
        // {
        //     InventoryManager.Instance.AddToInventory("Zombie", 100); //GameFlow Test
        // }
    }

        // Method to adjust health
    public void LoseHealth(int amount)
    {
        if (currentHealth <= 0)
        {
            Debug.Log("Player Dead");
        }
        else
        {
            currentHealth -= amount; 
        }

        Debug.Log($"Player health adjusted. New health: {currentHealth}");
    }
}
