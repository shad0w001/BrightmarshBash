using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterController))]
public class PlayerPhysicsHandler : MonoBehaviour, IPhysicsHandler
{
    private CharacterController controller;
    private Vector3 knockbackVelocity;
    private float knockbackTimer;
    private bool wasGroundedLastFrame = false;

    [Header("Knockback Settings")]
    public float airDrag = 1f;
    public float groundDrag = 8f;
    public float knockbackDuration = 0.3f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public void ApplyForce(Vector3 force)
    {
        force.y = Mathf.Clamp(force.y, 0f, 1f);

        knockbackVelocity += force;
        knockbackTimer = knockbackDuration;
    }

    private void Update()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && !wasGroundedLastFrame)
        {
            // Just landed this frame — zero horizontal velocity instantly
            knockbackVelocity.x = 0f;
            knockbackVelocity.z = 0f;

            // Optional: reset vertical velocity if falling or neutral
            if (knockbackVelocity.y <= 0f)
                knockbackVelocity.y = 0f;
        }

        wasGroundedLastFrame = isGrounded;

        // existing knockback move and drag logic...
        if (knockbackVelocity.magnitude > 0.1f)
        {
            controller.Move(knockbackVelocity * Time.deltaTime);

            if (knockbackTimer > 0)
            {
                knockbackTimer -= Time.deltaTime;
            }
            else
            {
                float drag = isGrounded ? groundDrag : airDrag;
                knockbackVelocity = Vector3.MoveTowards(knockbackVelocity, Vector3.zero, drag * Time.deltaTime);
            }
        }
        else
        {
            knockbackVelocity = Vector3.zero;
        }
    }
}
