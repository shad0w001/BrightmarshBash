using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrolling,
    Chasing,
    Attacking
}

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask isGround, isPlayer;

    public Vector3 walkPoint;
    bool hasSetWalkPoint;
    public float walkPointRange;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private EnemyState currentState;

    private void Awake()
    {
        player = GameObject.Find("PlayerCapsule").transform;
        agent = GetComponent<NavMeshAgent>();
        currentState = EnemyState.Patrolling;
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, isPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, isPlayer);

        // Transition between states
        if (playerInAttackRange)
        {
            SwitchState(EnemyState.Attacking);
        }
        else if (playerInSightRange)
        {
            SwitchState(EnemyState.Chasing);
        }
        else
        {
            SwitchState(EnemyState.Patrolling);
        }

        // Act based on current state
        switch (currentState)
        {
            case EnemyState.Patrolling:
                Patroling();
                break;
            case EnemyState.Chasing:
                ChasePlayer();
                break;
            case EnemyState.Attacking:
                if (agent.enabled && agent.isOnNavMesh)
                {
                    agent.SetDestination(transform.position);
                }
                // Attack logic here
                break;
        }
    }

    private void SwitchState(EnemyState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            hasSetWalkPoint = false; // reset patrol path if state changed
        }
    }

    private void Patroling()
    {
        if (!hasSetWalkPoint)
        {
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, isGround))
            {
                hasSetWalkPoint = true;
            }
        }

        if (hasSetWalkPoint)
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.SetDestination(walkPoint);
            }
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            hasSetWalkPoint = false;
        }
    }

    private void ChasePlayer()
    {
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        } 
    }
}
