using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBulletScript : MonoBehaviour
{
    public Rigidbody rigidBody;
    public GameObject explosion;
    public GameObject explosionEffect;
    public LayerMask isHittable;

    [Range(0f, 1f)]
    public float bouncinessFactor;
    public bool useGravity;

    public float explosionRange;
    public float explosionForce;
    public float explosionUpwardForce;
    private bool hasExploded = false;

    public int maxNumberOfCollisions;
    public float maxLifeTimeInSeconds;
    public bool explodeOnTounch = true;

    public GameObject shooter;
    public Team team;

    int collisions;
    PhysicMaterial collisionMaterial;

    private void Setup()
    {
        collisionMaterial = new PhysicMaterial();
        collisionMaterial.bounciness = bouncinessFactor;
        collisionMaterial.frictionCombine = PhysicMaterialCombine.Minimum;
        collisionMaterial.bounceCombine = PhysicMaterialCombine.Maximum;

        GetComponent<SphereCollider>().material = collisionMaterial;

        rigidBody.useGravity = useGravity;
    }

    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        if(collisions > maxNumberOfCollisions)
        {
            Explode();
        }

        maxLifeTimeInSeconds -= Time.deltaTime;
        if(maxLifeTimeInSeconds <= 0)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasExploded)// prevent double explosion
        {
            return;
        }

        collisions++;

        if (explodeOnTounch)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (hasExploded)// prevent double explosion
        {
            return;
        } 
        hasExploded = true;

        if (explosionEffect is not null)
        {
            //put the explosion into a variable so it can be deleted later
            explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, isHittable);

        for (int i = 0; i < enemies.Length; i++)
        {
            Rigidbody rb = enemies[i].GetComponent<Rigidbody>();
            if (rb == null)
            {
                continue;
            }

            GameObject target = rb.gameObject;

            //if hits self
            if (target == shooter)
            {
                Debug.Log("Hit self");

                IPhysicsHandler physicsHandler = shooter.GetComponent<IPhysicsHandler>();

                if (physicsHandler != null)
                {
                    Vector3 forceDir = (target.transform.position - transform.position).normalized;
                    Vector3 force = forceDir * explosionForce + Vector3.up * explosionUpwardForce;

                    physicsHandler.ApplyForce(force);
                }

                continue;
            }

            //if hits teammate
            TeamMember targetTeamMember = target.GetComponent<TeamMember>();
            if (targetTeamMember != null)
            {
                if (targetTeamMember.team == team)
                {
                    Debug.Log("Hit teammate");
                    // Same team = no knockback
                    continue;
                }
            }

            //else (hits enemy)
            Debug.Log("Hit enemy");
            rb.AddExplosionForce(explosionForce, transform.position, explosionRange, explosionUpwardForce, ForceMode.Impulse);
        }

        DestroyBullet();
    }

    private void DestroyBullet()
    {
        Destroy(gameObject, 0.05f);
        Destroy(explosion, 3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
