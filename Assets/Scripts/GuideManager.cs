using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideManager : MonoBehaviour
{
    public GameObject prefabToSpawn1; // Guide prefab
    public GameObject prefabToSpawn2; // Winning prefab
    public Transform player; // Reference to the player's transform
    public float spawnDistance = 2.0f; // Distance in front of the player to spawn the prefab
    public float despawnDistance = 50.0f; // Distance from the player at which the prefab will despawn

    private GameObject guideObject; // Reference to the currently spawned guide prefab
    private GameObject winningObject; // Reference to the currently spawned winning prefab

    private bool gameIsWon = false;

    void Start()
    {
        // Spawn the guide object as soon as the scene starts
        if (guideObject == null)
        {
            guideObject = SpawnPrefab(prefabToSpawn1);
        }
    }

    void Update()
    {
        int currentCoinCount = InventoryManager.Instance.GetItemQuantity("Coin");

        // Spawn guide prefab when 'E' is pressed if it's not already spawned
        if (Input.GetKeyDown(KeyCode.E) && guideObject == null)
        {
            guideObject = SpawnPrefab(prefabToSpawn1);
        }

        // Spawn winning prefab if the coin count reaches 4 and it's not already spawned
        if (currentCoinCount >= 4 && winningObject == null && !gameIsWon)
        {
            winningObject = SpawnPrefab(prefabToSpawn2);
            gameIsWon = true;
        }

        // Check despawn conditions for both prefabs
        CheckDespawnDistance(ref guideObject);
        CheckDespawnDistance(ref winningObject);
    }

    GameObject SpawnPrefab(GameObject prefabToBeSpawned)
    {
        // Calculate the position in front of the player
        Vector3 spawnPosition = player.position + player.forward * spawnDistance;
        spawnPosition.y += 1.5f;

        // Instantiate the prefab
        GameObject spawnedObject = Instantiate(prefabToBeSpawned, spawnPosition, Quaternion.identity);

        Debug.Log($"Prefab {prefabToBeSpawned.name} spawned at {spawnPosition}!");
        return spawnedObject;
    }

    void CheckDespawnDistance(ref GameObject spawnedObject)
    {
        if (spawnedObject == null) return;

        // Check the distance between the player and the spawned object
        float distance = Vector3.Distance(player.position, spawnedObject.transform.position);

        if (distance > despawnDistance)
        {
            // Destroy the prefab if the player is too far away
            Destroy(spawnedObject);
            spawnedObject = null;
            Debug.Log($"Prefab {spawnedObject?.name} destroyed due to distance.");
        }
    }
}