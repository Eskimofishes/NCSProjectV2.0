using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public string animalName;
    public bool playerInRange;
    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;
    [Header("Sounds")]
    [SerializeField] AudioSource soundChannel;
    [SerializeField] AudioClip animalHitAndScream;
    [SerializeField] AudioClip animalHitAndDie;
    private Animator animator;
    [SerializeField] ParticleSystem bloodSplashParticles;
    [SerializeField] GameObject bloodPuddle;
    public bool isDead;
    enum AnimalType
    {
        Rabbit
    }
    [SerializeField] AnimalType thisAnimalType;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead && Input.GetKeyDown(KeyCode.Mouse0) && playerInRange && SelectionManager.Instance.onTarget && SelectionManager.Instance.selectedObject == gameObject)
        {
            Debug.Log("Item Picked up");
            InventoryManager.Instance.AddToInventory(animalName);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if(isDead == false)
        {
            currentHealth -= damage;
            bloodSplashParticles.Play();

            if(currentHealth <= 0)
            {
                PlayDyingSound();
                animator.SetTrigger("Dies");
                GetComponent<AI_Movement>().enabled = false;
                bloodPuddle.SetActive(true);
                isDead = true;
            }
            else
            {
                PlayHitSound();            
            }            
        }
    }

    private void PlayDyingSound()
    {
        switch(thisAnimalType)
        {
            case AnimalType.Rabbit:
                soundChannel.PlayOneShot(animalHitAndDie);
                break;
            default:
                break;
            //
        }
    }
    private void PlayHitSound()
    {
        switch(thisAnimalType)
        {
            case AnimalType.Rabbit:
                soundChannel.PlayOneShot(animalHitAndScream);
                break;
            default:
                break;
            //
        }
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

}
