using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


 
public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance{get; set;}

    public GameObject interaction_Info_UI;

    public GameObject selectedObject;

    public bool onTarget;

    public Image centerDotImage;
    public Image handIcon;

    private GameObject player; // Reference to the player object
    private Rigidbody playerRigidbody; // Reference to the player's Rigidbody
    private Vector3 playerVelocity; // Store the player's velocity

    [Header("Animator")]
    [SerializeField] private Animator animator;




    CharacterController controller;
    AudioSource audioSource;

    Text interaction_text;
 
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //get player rigid body 
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Get the Rigidbody component of the player
            playerRigidbody = player.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("Player object not found! Make sure the player has the 'Player' tag.");
        }

        onTarget = false;
        interaction_text = interaction_Info_UI.GetComponent<Text>();
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
 
    void Update()
    {
        // Get the player's velocity
        playerVelocity = playerRigidbody.velocity;
        SetAnimations();


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;



        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;
            InteractableObject interactable = selectionTransform.GetComponent<InteractableObject>();

            NPC npc = selectionTransform.GetComponent<NPC>();
            if(npc && npc.playerInRange)
            {
                interaction_text.text = "Talk";
                interaction_Info_UI.SetActive(true);

                if(Input.GetMouseButtonDown(0) && npc.isTalkingWithPlayer == false)
                {
                    npc.StartConversation();
                }

                if(DialogSystem.Instance.dialogUIActive)
                {
                    interaction_Info_UI.SetActive(false);
                    centerDotImage.gameObject.SetActive(false);
                }

            }

            //Hittables
            Animal animal = selectionTransform.GetComponent<Animal>();

            if(animal && animal.playerInRange)
            {
                if(animal.isDead)
                {
                    interaction_text.text = animal.animalName + " (lootable)"; 
                    interaction_Info_UI.SetActive(true);

                    onTarget = true;
                    selectedObject = animal.gameObject;

                    centerDotImage.gameObject.SetActive(false);
                    handIcon.gameObject.SetActive(true);


                    //handIsVisible = true;
                }
                else
                {
                    interaction_text.text = animal.animalName;
                    interaction_Info_UI.SetActive(true);

                    centerDotImage.gameObject.SetActive(true);
                    handIcon.gameObject.SetActive(false);

                    //handIsVisible = false;

                    if(Input.GetMouseButtonDown(0) && Time.time >= nextDamageTime)
                    {
                        nextDamageTime = Time.time + attackCooldown;
                        Attack();
                        if (!isDamageCoroutineRunning)
                        {
                            StartCoroutine(DealDamageToAnimal(animal, damageDelay, PlayerState.Instance.damage));                        
                        }
                    }                    
                }
            }


            Enemy enemy = selectionTransform.GetComponent<Enemy>();

            if(enemy && enemy.playerInRange)
            {
                interaction_text.text = enemy.enemyName + " (" + enemy.currentHealth + " / " + enemy.maxHealth + ")"; 
                interaction_Info_UI.SetActive(true);
                centerDotImage.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
                if(Input.GetMouseButtonDown(0) && Time.time >= nextDamageTime)
                {
                    nextDamageTime = Time.time + attackCooldown;
                    Attack();
                    if (!isDamageCoroutineRunning)
                    {
                        StartCoroutine(DealDamageToEnemy(enemy, damageDelay, PlayerState.Instance.damage));
                    }
                }
            }

            
 
            if (interactable && interactable.playerInRange)
            {
                onTarget = true;
                selectedObject = interactable.gameObject;
                interaction_text.text = interactable.GetItemName();
                interaction_Info_UI.SetActive(true);
                centerDotImage.gameObject.SetActive(false);
                handIcon.gameObject.SetActive(true);

                //handIsVisible = true;
            }


            //if no hit
            if(!interactable && !animal)
            {
                onTarget = false;
                centerDotImage.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
                //handIsVisible = false;
            }
            if(!npc && !interactable && !animal && !enemy)
            {
                interaction_text.text = "";
                interaction_Info_UI.SetActive(false);
            }
 
        }
    }
    private bool isDamageCoroutineRunning = false;
    IEnumerator DealDamageToAnimal(Animal animal, float delay, int damage)
    {
        isDamageCoroutineRunning = true;
        yield return new WaitForSeconds(delay);
        animal.TakeDamage(damage);
        isDamageCoroutineRunning = false;
    }

    IEnumerator DealDamageToEnemy(Enemy enemy, float delay, int damage)
    {
        isDamageCoroutineRunning = true;
        yield return new WaitForSeconds(delay);
        enemy.TakeDamage(damage);
        isDamageCoroutineRunning = false;
    }
    // ---------------------//
    // ATTACKING ANIMATIONS //
    // ------------------- //

    [SerializeField] public float attackCooldown = 1f;
    private float nextDamageTime = 0f;
    public const string IDLE = "Idle";
    public const string WALK = "Walk";
    public const string ATTACK1 = "Attack 1";
    public const string ATTACK2 = "Attack 2";

    string currentAnimationState;

    public void ChangeAnimationState(string newState) 
    {
        // STOP THE SAME ANIMATION FROM INTERRUPTING WITH ITSELF //
        if (currentAnimationState == newState) return;

        // PLAY THE ANIMATION //
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

    void SetAnimations()
    {
        // If player is not attacking
        if(!attacking)
        {
            if(Mathf.Abs(playerVelocity.x) < 0.01f && Mathf.Abs(playerVelocity.z) < 0.01f)
            { ChangeAnimationState(IDLE); }
            else
            { ChangeAnimationState(WALK); }
        }
    }

    // ------------------- //
    // ATTACKING BEHAVIOUR //
    // ------------------- //

    [Header("Attacking")]
    public float damageDelay = 0.4f;
    public float attackSpeed = 1f;

    public GameObject hitEffect;
    public AudioClip swordSwing;
    public AudioClip hitSound;

    bool attacking = false;
    bool readyToAttack = true;
    int attackCount;

    public void Attack()
    {
        if(!readyToAttack || attacking) return;

        readyToAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);

        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(swordSwing);

        if(attackCount == 0)
        {
            ChangeAnimationState(ATTACK1);
            attackCount++;
        }
        else
        {
            ChangeAnimationState(ATTACK2);
            attackCount = 0;
        }
    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }

    void HitTarget(Vector3 pos)
    {
        audioSource.pitch = 1;
        audioSource.PlayOneShot(hitSound);

        GameObject GO = Instantiate(hitEffect, pos, Quaternion.identity);
        Destroy(GO, 20);
    }
}
