using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearWalkState : StateMachineBehaviour
{
    float timer;
    public float walkingTime = 10f;
    Transform player;
    UnityEngine.AI.NavMeshAgent agent;
    public float detectionAreaRadius = 18f;
    public float walkSpeed = 2f;
    List<Transform> waypointsList = new List<Transform>();
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //initialization
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<UnityEngine.AI.NavMeshAgent>();

        agent.speed = walkSpeed;
        timer = 0;

        //get waypoints
        GameObject waypointsCluster = animator.GetComponent<NPCWaypoints>().npcWayPointsCluster;
        foreach(Transform t in waypointsCluster.transform)
        {
            waypointsList.Add(t);
        }

        Vector3 firstPosition = waypointsList[Random.Range(0, waypointsList.Count)].position;
        agent.SetDestination(firstPosition);
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //one way point to the next
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(waypointsList[Random.Range(0, waypointsList.Count)].position);
        }

        //transition to idle
        timer += Time.deltaTime;
        if(timer > walkingTime)
        {
            animator.SetBool("isWalking", false);
        }
        //transition to chase
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if(distanceFromPlayer < detectionAreaRadius)
        {
            animator.SetBool("isChasing",true);
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       agent.SetDestination(agent.transform.position);
    }
}
