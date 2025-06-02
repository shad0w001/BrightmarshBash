using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))]
public class EnemyPhysicsHandler : MonoBehaviour, IPhysicsHandler
{
    private Rigidbody rb;
    private NavMeshAgent agent;

    [Header("Knockback Settings")]
    public float disableAgentDuration = 0.5f;
    public float landCheckDelay = 0.1f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    public float groundCheckRadius = 0.3f; // tweak this as needed
    public Transform groundCheckPoint; // an empty GameObject child near feet

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void ApplyForce(Vector3 force)
    {
        Debug.Log("ApplyForce called with force: " + force);

        if (agent.enabled)
            agent.enabled = false;

        // Prevent tilt during knockback
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        rb.isKinematic = false;
        rb.AddForce(force, ForceMode.Impulse);

        StartCoroutine(EnableAgentWhenGrounded());
    }

    private IEnumerator EnableAgentWhenGrounded()
    {
        Debug.Log("Coroutine started");
        yield return new WaitForSeconds(landCheckDelay);
        Debug.Log("Waited landCheckDelay");

        while (!IsGrounded())
        {
            Debug.Log("Waiting to be grounded...");
            yield return null;
        }

        Debug.Log("Is grounded, resetting rotation and re-enabling NavMeshAgent");

        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 2f, NavMesh.AllAreas))
        {
            Debug.Log("SamplePosition succeeded at " + hit.position);
            transform.position = hit.position;

            if (!agent.enabled)
            {
                agent.enabled = true;
                Debug.Log("NavMeshAgent re-enabled");
            }
        }
        else
        {
            Debug.LogWarning("Failed to find NavMesh position");
        }

        yield return null;
    }

    private bool IsGrounded()
    {
        Debug.DrawRay(groundCheckPoint.position, Vector3.down * 0.1f, Color.green);

        return Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundMask);
    }
}
