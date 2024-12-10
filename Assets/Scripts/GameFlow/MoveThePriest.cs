using UnityEngine;
using UnityEngine.AI;

public class MoveThePriest : MonoBehaviour
{
    public Transform firstTargetLocation; // First movement target
    public Transform secondTargetLocation; // Second movement target
    public Animator animator; // Reference to the Animator component

    private NavMeshAgent navMeshAgent;
    public BoxCollider interactionBoxCollider;
    public BoxCollider lazydebughardcodecollider;

    private void Start()
    {
        // Get the NavMeshAgent component
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component missing on priest!");
        }
    }

    public void MoveToFirstLocation()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(firstTargetLocation.position);
            StartWalkingAnimation();
        }
    }

    public void MoveToSecondLocation()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(secondTargetLocation.position);
            StartWalkingAnimation();
        }
    }

    private void Update()
    {
        // Stop walking animation if the agent reaches the destination
        if (navMeshAgent != null && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && !navMeshAgent.pathPending)
        {
            StopWalkingAnimation();
        }
    }

    private void StartWalkingAnimation()
    {
        interactionBoxCollider.enabled = false;
        lazydebughardcodecollider.enabled = false;

        // Trigger walking animation
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }
    }

    private void StopWalkingAnimation()
    {
        interactionBoxCollider.enabled = true;
        lazydebughardcodecollider.enabled = true;

        // Stop walking animation
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
    }
}