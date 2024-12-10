using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearChaseState : StateMachineBehaviour
{
    Transform player;
    UnityEngine.AI.NavMeshAgent agent;
    public float chaseSpeed = 6f;
    public float stopChasingDistance = 21;
    public float attackingDistance = 2.5f;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       player = GameObject.FindGameObjectWithTag("Player").transform;
       agent = animator.GetComponent<UnityEngine.AI.NavMeshAgent>();

       agent.speed = chaseSpeed;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       agent.SetDestination(player.position);
       animator.transform.LookAt(player);

       float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

       //agent stop chasing (transition to walk)
       if(distanceFromPlayer > stopChasingDistance)
       {
        animator.SetBool("isChasing",false);
       }
       //transition to attacking
       if(distanceFromPlayer < attackingDistance)
       {
        animator.SetBool("isAttacking",true);
       }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       agent.SetDestination(agent.transform.position);
    }
}