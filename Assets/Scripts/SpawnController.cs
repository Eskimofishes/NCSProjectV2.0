using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public float spawnDelay = 0.5f; 

    public List<MonoBehaviour> currentAlive = new List<MonoBehaviour>();

    [SerializeField] public GameObject Prefab;
    [SerializeField] public int NumberOfSpawnables = 5;

    private enum SpawnableType
    {
        Enemy,
        Animal
    }
    [SerializeField] SpawnableType thisSpawnable;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (currentAlive.Count < NumberOfSpawnables && !isSpawning)
        {
            StartCoroutine(Spawn());
        }
        List<MonoBehaviour> toRemove = new List<MonoBehaviour>();
        foreach (MonoBehaviour entity in currentAlive)
        {
            if (entity is Enemy zombie && zombie.isDead)
            {
                toRemove.Add(zombie);
            }
            else if (entity is Animal animal && animal.isDead)
            {
                toRemove.Add(animal);
            }
        }
        // Remove all entities marked for removal
        foreach (MonoBehaviour entity in toRemove)
        {
            currentAlive.Remove(entity);
        }
        toRemove.Clear();
    }

    private bool isSpawning = false;
    private IEnumerator Spawn()
    {
        isSpawning = true;
        Vector3 spawnOffset = new Vector3(Random.Range(-1f,1f),0f,Random.Range(-1f,1f));
        Vector3 spawnPosition = transform.position +spawnOffset;

        //Instantiate the spawn
        var spawnable = Instantiate(Prefab, spawnPosition, Quaternion.identity);

        switch(thisSpawnable)
        {                           
            case SpawnableType.Animal:
                Animal animalScript = spawnable.GetComponent<Animal>();
                currentAlive.Add(animalScript);
                break;
            case SpawnableType.Enemy:
                Enemy enemyScript = spawnable.GetComponent<Enemy>();
                currentAlive.Add(enemyScript);
                break;
            default:
                break;
            //
        }

        yield return new WaitForSeconds(spawnDelay);
        isSpawning = false;
    }


}
