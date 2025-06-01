using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGun : MonoBehaviour
{
    public GameObject bullet;

    public float shootForce;
    public float upwardForce;

    public float timeBetweenShots;
    public float timeBetweenShooting;
    public float spread;
    public float reloadTime;

    public int magazineSize;
    public int bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft;
    int bulletsShot;

    bool readyToShoot;
    bool shooting;
    bool reloading;

    public Camera fpsCamera;
    public Transform attackPoint;

    bool allowReset = true;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (Input.GetKeyDown(KeyCode.R) && !reloading)
        {
            Reload();
        }

        if(readyToShoot && shooting && !reloading && bulletsLeft> 0)
        {
            bulletsShot = 0;

            ShootGun();
        }
    }

    private void ShootGun()
    {
        readyToShoot = false;

        Ray ray = fpsCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(75);
        }

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);

        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;


        Rigidbody bulletRigidBody = currentBullet.GetComponent<Rigidbody>();
        bulletRigidBody.AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        bulletRigidBody.AddForce(fpsCamera.transform.up * upwardForce, ForceMode.Impulse);

        var bulletScript = currentBullet.GetComponent<CustomBulletScript>();

        if (bulletScript != null)
        {
            bulletScript.shooter = this.gameObject;

            // You need a way to know the team of this gun/shooter.
            // For example, get it from a TeamMember component:
            var teamMember = GetComponent<TeamMember>();
            if (teamMember != null)
            {
                bulletScript.team = teamMember.team;
            }
        }

        bulletsLeft--;
        bulletsShot++;

        if(allowReset)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowReset = false;
        }
        
        if(bulletsShot < bulletsPerTap && bulletsLeft > 0)
        {
            Invoke("ShootGun", timeBetweenShooting);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("HandleReload", reloadTime);
    }

    private void HandleReload()
    {
        bulletsLeft = magazineSize;
        reloading = false;
    }
}
