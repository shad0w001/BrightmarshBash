using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyPhysicsHandler : MonoBehaviour, IPhysicsHandler
{

    private Rigidbody rb;
    private Vector3 knockbackVelocity;
    private float knockbackTimer;

    public float drag = 5f;
    public float knockbackDuration = 0.3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void ApplyForce(Vector3 force)
    {
        knockbackVelocity += force;
        knockbackTimer = knockbackDuration;
        rb.isKinematic = false;
    }

    private void FixedUpdate()
    {
        if (knockbackVelocity.magnitude > 0.1f)
        {
            rb.AddForce(knockbackVelocity, ForceMode.Acceleration);

            if (knockbackTimer > 0)
            {
                knockbackTimer -= Time.fixedDeltaTime;
            }
            else
            {
                knockbackVelocity = Vector3.MoveTowards(knockbackVelocity, Vector3.zero, drag * Time.fixedDeltaTime);
                if (knockbackVelocity.magnitude < 0.1f)
                {
                    knockbackVelocity = Vector3.zero;
                    rb.velocity = Vector3.zero;
                    rb.isKinematic = true; // optional, freeze enemy after knockback stops
                }
            }
        }
    }
}
