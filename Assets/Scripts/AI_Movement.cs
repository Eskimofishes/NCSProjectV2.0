using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    public float moveSpeed = 2.0f;

    private float walkTime;
    public float walkCounter;
    private float waitTime;
    public float waitCounter;

    private bool isWalking;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Set NavMeshAgent speed
        navMeshAgent.speed = moveSpeed;

        // Randomize walk and wait times to avoid synchronized movement
        walkTime = Random.Range(3, 6);
        waitTime = Random.Range(5, 7);

        waitCounter = waitTime;
        walkCounter = walkTime;

        ChooseDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if (isWalking)
        {
            animator.SetBool("isRunning", true);

            walkCounter -= Time.deltaTime;

            // Check if the agent has reached its destination
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                StopAgent();
            }

            if (walkCounter <= 0)
            {
                StopAgent();
            }
        }
        else
        {
            waitCounter -= Time.deltaTime;

            if (waitCounter <= 0)
            {
                ChooseDestination();
            }
        }
    }

    void ChooseDestination()
    {
        // Choose a random direction to walk in
        Vector3 randomDirection = Random.insideUnitSphere * 10.0f; // Radius of 10 units
        randomDirection += transform.position;

        // Find a valid NavMesh point within the range
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10.0f, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
            isWalking = true;
            walkCounter = walkTime;
        }
    }

    void StopAgent()
    {
        isWalking = false;
        waitCounter = waitTime;
        navMeshAgent.isStopped = true; // Explicitly stop the agent
        navMeshAgent.ResetPath(); // Clear the path to prevent unintended sliding
        animator.SetBool("isRunning", false);
    }
}