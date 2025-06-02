using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyProjectileGun : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform attackPoint;
    public GameObject shooter;

    public float shootForce = 20f;
    public float upwardForce = 1f;
    public float timeBetweenShooting = 1f;
    public int magazineSize = 5;
    public float reloadTime = 2f;
    public float spread = 0.1f;

    private int bulletsLeft;
    private bool readyToShoot = true;
    private bool reloading = false;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    public void ShootAt(Vector3 targetPosition)
    {
        if (!readyToShoot || reloading || bulletsLeft <= 0) return;

        readyToShoot = false;
        bulletsLeft--;

        Vector3 direction = (targetPosition - attackPoint.position).normalized;

        float spreadX = Random.Range(-spread, spread);
        float spreadY = Random.Range(-spread, spread);
        Vector3 spreadOffset = new Vector3(spreadX, spreadY, 0);
        Vector3 finalDirection = direction + attackPoint.TransformDirection(spreadOffset);

        GameObject currentBullet = Instantiate(bulletPrefab, attackPoint.position + attackPoint.forward * 0.5f, Quaternion.LookRotation(finalDirection));

        // IGNORE COLLISIONS BETWEEN BULLET AND SHOOTER COLLIDERS
        Collider bulletCollider = currentBullet.GetComponent<Collider>();
        if (bulletCollider != null && shooter != null)
        {
            Collider[] shooterColliders = shooter.GetComponentsInChildren<Collider>();
            foreach (var sc in shooterColliders)
            {
                if (sc != null)
                    Physics.IgnoreCollision(bulletCollider, sc);
            } 
        }

        Rigidbody rb = currentBullet.GetComponent<Rigidbody>();
        rb.AddForce(finalDirection.normalized * shootForce, ForceMode.Impulse);
        rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);

        var bulletScript = currentBullet.GetComponent<CustomBulletScript>();
        if (bulletScript != null)
        {
            bulletScript.shooter = this.gameObject;
            var teamMember = shooter.GetComponent<TeamMember>();
            if (teamMember != null)
                bulletScript.team = teamMember.team;
        }

        Invoke(nameof(ResetShot), timeBetweenShooting);

        if (bulletsLeft <= 0)
            Invoke(nameof(Reload), reloadTime);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke(nameof(FinishReload), reloadTime);
    }

    private void FinishReload()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }

    public bool IsReadyToShoot()
    {
        return readyToShoot && !reloading && bulletsLeft > 0;
    }
}

