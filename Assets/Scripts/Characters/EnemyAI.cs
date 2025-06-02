using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum EnemyState
{
    SeekingCapturePoint,
    Patrolling,
    Chasing,
    Attacking
}

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public Transform capturePoint;

    public LayerMask isGround, isPlayer;

    public float sightRange = 10f;
    public float attackRange = 2f;
    public float capturePointRange = 5f;
    public float patrolPointThreshold = 1.5f;

    private EnemyState currentState;

    private Vector3 patrolTarget;
    private bool hasPatrolTarget;

    private bool playerInSightRange;
    private bool playerInAttackRange;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("PlayerCapsule").transform;

        if (capturePoint == null)
        {
            Debug.LogError("Capture Point not assigned.");
        }

        Debug.Log($"{capturePoint.position.ToString()}");
        currentState = EnemyState.SeekingCapturePoint;
    }

    private void Update()
    {
        if (capturePoint == null || agent == null || !agent.isOnNavMesh) return;

        // Check player proximity
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, isPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, isPlayer);

        // State transitions (priority order)
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
            float distToCapture = Vector3.Distance(transform.position, capturePoint.position);

            if (distToCapture > capturePointRange)
                SwitchState(EnemyState.SeekingCapturePoint);
            else
                SwitchState(EnemyState.Patrolling);
        }

        // Handle state behavior
        switch (currentState)
        {
            case EnemyState.SeekingCapturePoint:
                Debug.Log("seeking capture point");
                SeekCapturePoint();
                break;
            case EnemyState.Patrolling:
                Debug.Log("patroling");
                PatrolAroundCapturePoint();
                break;
            case EnemyState.Chasing:
                Debug.Log("chasing");
                ChasePlayer();
                break;
            case EnemyState.Attacking:
                Debug.Log("attacking");
                StopAndAttack();
                break;
        }
    }

    private void SwitchState(EnemyState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            hasPatrolTarget = false;
        }
    }

    private void SeekCapturePoint()
    {
        if (agent.enabled)
        {
            agent.SetDestination(capturePoint.position);
        }
    }

    private void PatrolAroundCapturePoint()
    {
        if (!hasPatrolTarget)
        {
            Vector2 offset = Random.insideUnitCircle * capturePointRange;
            patrolTarget = capturePoint.position + new Vector3(offset.x, 0, offset.y);

            if (Physics.Raycast(patrolTarget + Vector3.up * 2f, Vector3.down, 4f, isGround))
            {
                hasPatrolTarget = true;
            }
        }

        if (hasPatrolTarget && agent.enabled)
        {
            agent.SetDestination(patrolTarget);
        }

        if (Vector3.Distance(transform.position, patrolTarget) < patrolPointThreshold)
        {
            hasPatrolTarget = false;
        }
    }

    private void ChasePlayer()
    {
        if (agent.enabled)
        {
            agent.SetDestination(player.position);
        }
    }

    private void StopAndAttack()
    {
        if (agent.enabled)
        {
            agent.ResetPath(); // Stop moving
        }

        // TODO: Trigger attack animations/effects
    }
}