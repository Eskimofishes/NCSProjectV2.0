using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public string enemyName;
    public bool playerInRange;

    public int currentHealth;
    [SerializeField] public int maxHealth = 100;
    [SerializeField] public int damage = 5;

    [Header("Sounds")]
    [SerializeField] AudioSource soundChannel;
    [SerializeField] AudioClip enemyHitAndScream;
    [SerializeField] AudioClip enemyHitAndDie;

    [Header("Animations")]
    [SerializeField] ParticleSystem bloodSplashParticles;
    [SerializeField] GameObject bloodPuddle;

    private Animator animator;
    private UnityEngine.AI.NavMeshAgent navAgent;

    public bool isDead;


    enum EnemyType
    {
        Zombie,
        Bear
    }

    [SerializeField] EnemyType thisEnemyType;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void TakeDamage(int damageAmount)
    {
        switch(thisEnemyType)
        {
            case(EnemyType.Zombie):
                if(!isDead)
                {
                    currentHealth -= damageAmount;
                    if(currentHealth <= 0)
                    {
                        isDead = true;
                        int randomValue = Random.Range(0,2); // 0 or 1

                        if(randomValue == 0)
                        {
                            animator.SetTrigger("DIE1");
                        }
                        else
                        {
                            animator.SetTrigger("DIE2");
                        }
                        soundChannel.PlayOneShot(enemyHitAndDie);
                        DestroyAttackingPoint("AttackingPoint");
                        InventoryManager.Instance.AddToInventory("Zombie");
                    }
                    else
                    {
                        animator.SetTrigger("DAMAGE");
                        soundChannel.PlayOneShot(enemyHitAndScream);
                    }
                }              
                break;
            case(EnemyType.Bear):
                if(!isDead)
                {
                    currentHealth -= damageAmount;
                    if(currentHealth <=0)
                    {
                        isDead = true;
                        animator.SetTrigger("DIE");
                        DestroyAttackingPoint("AttackingPoint");
                        soundChannel.PlayOneShot(enemyHitAndDie);
                        bloodPuddle.SetActive(true);
                        InventoryManager.Instance.AddToInventory("Bear");
                    }
                    else
                    {
                        bloodSplashParticles.Play();
                        animator.SetTrigger("DAMAGE");
                        soundChannel.PlayOneShot(enemyHitAndScream);
                        
                    }
                }
                break;
            default:
                break;

        }
    }
    private bool coroutineStarted = false;
    void Update()
    {
        if (isDead && !coroutineStarted)
        {
            coroutineStarted = true; // Set the flag to prevent further calls
            StartCoroutine(WaitForSecondsCoroutine());
        }
    }

    IEnumerator WaitForSecondsCoroutine()
    {
        Debug.Log("Waiting starts...");
        
        // Pause execution for 5 seconds
        yield return new WaitForSeconds(5);

        Debug.Log("5 seconds have passed. Destroying the GameObject...");
        
        // Destroy the GameObject after the wait
        Destroy(gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void DestroyAttackingPoint(string tag)
    {
        // Find all children under this GameObject's hierarchy
        foreach (Transform child in GetComponentsInChildren<Transform>(true)) // Include inactive children
        {
            if (child.CompareTag(tag))
            {
                Destroy(child.gameObject);
                Debug.Log($"Destroyed child object: {child.name} with tag: {tag}");
            }
        }
    }

    // private void PlayDyingSound()
    // {
    //     // switch(thisEnemyType)
    //     // {
    //     //     // case EnemyType.Zombie:
    //     //     //     soundChannel.PlayOneShot(enemyHitAndDie);
    //     //     //     break;
    //     //     // case EnemyType.Bear:
    //     //     //     soundChannel.PlayOneShot(enemyHitAndDie);
    //     //     // default:
    //     //     //     break;
    //     //     // //
    //     // }
    // }
    // private void PlayHitSound()
    // {
    //     // switch(thisEnemyType)
    //     // {
    //     //     case EnemyType.Zombie:
    //     //         soundChannel.PlayOneShot(enemyHitAndScream);
    //     //         break;
    //     //     case EnemyType.Bear:
    //     //         soundChannel.PlayOneShot(enemyHitAndScream);
    //     //     default:
    //     //         break;
    //     //     //
    //     // }
    // }

}
