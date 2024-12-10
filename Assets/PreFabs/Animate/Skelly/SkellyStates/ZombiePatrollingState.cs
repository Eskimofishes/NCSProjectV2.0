using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePatrollingState : StateMachineBehaviour
{
    float timer;
    public float patrollingTime = 10f;
    Transform player;
    UnityEngine.AI.NavMeshAgent agent;
    public float detectionAreaRadius = 18f;
    public float patrolSpeed = 2f;

    List<Transform> waypointsList = new List<Transform>();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //initialization
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<UnityEngine.AI.NavMeshAgent>();

        agent.speed = patrolSpeed;
        timer = 0;

        //get waypoints
        GameObject waypointsCluster = GameObject.FindGameObjectWithTag("Waypoints");
        foreach(Transform t in waypointsCluster.transform)
        {
            waypointsList.Add(t);
        }

        Vector3 nextPosition = waypointsList[Random.Range(0, waypointsList.Count)].position;
        agent.SetDestination(nextPosition);
       
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       //if arrived at next waypoint
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(waypointsList[Random.Range(0, waypointsList.Count)].position);
        }
        //back to idle
        timer += Time.deltaTime;
        if(timer > patrollingTime)
        {
            animator.SetBool("isPatrolling",false);
        }

        //to chase
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if(distanceFromPlayer < detectionAreaRadius)
        {
            animator.SetBool("isChasing",true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       //stop the agent
       agent.SetDestination(agent.transform.position);
    }
}