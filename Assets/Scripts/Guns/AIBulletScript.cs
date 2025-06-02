using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBulletScript : MonoBehaviour
{
    [Header("Physics")]
    public Rigidbody rigidBody;
    public float bouncinessFactor = 0.6f;
    public bool useGravity = true;

    [Header("Explosion")]
    public float explosionRange = 5f;
    public float explosionForce = 20f;
    public float explosionUpwardForce = 1f;
    public GameObject explosionEffect;
    public LayerMask isHittable;
    public float upwardBiasMultiplier = 2f;

    [Header("Lifetime")]
    public int maxNumberOfCollisions = 3;
    public float maxLifeTimeInSeconds = 5f;
    public bool explodeOnTouch = true;
    private float spawnTime;
    public float gracePeriod = 0.5f;

    [Header("Metadata")]
    public GameObject shooter;
    public Team team;

    private int collisions = 0;
    private bool hasExploded = false;
    private GameObject explosionInstance;

    private void Start()
    {
        SetupPhysics();
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (hasExploded) return;

        maxLifeTimeInSeconds -= Time.deltaTime;
        if (maxLifeTimeInSeconds <= 0f)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded) return;

        if (Time.time - spawnTime < gracePeriod)
            return;

        collisions++;

        if (explodeOnTouch)
        {
            Explode();
        }
    }

    private void SetupPhysics()
    {
        PhysicMaterial mat = new PhysicMaterial
        {
            bounciness = bouncinessFactor,
            frictionCombine = PhysicMaterialCombine.Minimum,
            bounceCombine = PhysicMaterialCombine.Maximum
        };

        var collider = GetComponent<SphereCollider>();
        if (collider != null) collider.material = mat;

        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();

        rigidBody.useGravity = useGravity;
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explosionEffect != null)
        {
            explosionInstance = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosionInstance, 3f);
        }

        Collider[] targets = Physics.OverlapSphere(transform.position, explosionRange, isHittable);

        foreach (var col in targets)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb == null) continue;

            GameObject target = rb.gameObject;

            // Ignore self
            if (target == shooter) continue;

            // Only affect the player
            if (!target.CompareTag("Player"))
            {
                continue;  // skip everything that isn't the player
            }

            // Apply knockback to player
            IPhysicsHandler physicsHandler = target.GetComponent<IPhysicsHandler>();
            Vector3 forceDir = (target.transform.position - transform.position).normalized;
            Vector3 force = forceDir * explosionForce + Vector3.up * explosionUpwardForce * upwardBiasMultiplier;

            if (physicsHandler != null)
            {
                physicsHandler.ApplyForce(force);
            }
            else
            {
                // fallback force application
                rb.AddExplosionForce(explosionForce, transform.position, explosionRange, explosionUpwardForce, ForceMode.Impulse);
            }
        }

        Destroy(gameObject, 0.05f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
