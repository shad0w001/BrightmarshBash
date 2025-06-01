using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBulletScript : MonoBehaviour
{
    public Rigidbody rigidBody;
    public GameObject explosionEffect;
    public LayerMask isHittable;

    [Range(0f, 1f)]
    public float bouncinessFactor;
    public bool useGravity;

    public float explosionRange;
    public float explosionForce;
    public float explosionUpwardForce;

    public int maxNumberOfCollisions;
    public float maxLifeTimeInSeconds;
    public bool explodeOnTounch = true;

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
        collisions++;

        if (collision.collider.CompareTag("Enemy") && explodeOnTounch)
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (explosionEffect is not null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider[] enemies = Physics.OverlapSphere(transform.position, explosionRange, isHittable);

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].GetComponent<Rigidbody>())
            {
                enemies[i].GetComponent<Rigidbody>().AddExplosionForce(explosionForce, transform.position, explosionRange, explosionUpwardForce, ForceMode.VelocityChange);
            }
        }

        Invoke("DestroyBullet", 0.05f);
    }

    private void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
