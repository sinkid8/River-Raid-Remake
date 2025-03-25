using UnityEngine;

public class WeaponHandler
{
    private GameObject laserPrefab;
    private GameObject missilePrefab;
    private Transform firePoint;
    private float laserFireRate;
    private float missileFireRate;

    private float lastLaserFireTime = 0f;
    private float lastMissileFireTime = 0f;

    public WeaponHandler(GameObject laserPrefab, GameObject missilePrefab, Transform firePoint,
                          float laserFireRate, float missileFireRate)
    {
        this.laserPrefab = laserPrefab;
        this.missilePrefab = missilePrefab;
        this.firePoint = firePoint;
        this.laserFireRate = laserFireRate;
        this.missileFireRate = missileFireRate;
    }

    public bool FireLaser()
    {
        if (Time.time > lastLaserFireTime + laserFireRate)
        {
            if (laserPrefab != null && firePoint != null)
            {
                Object.Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
                lastLaserFireTime = Time.time;
                return true;
            }
            else
            {
                Debug.LogWarning("Laser Prefab or FirePoint not assigned to WeaponHandler");
            }
        }
        return false;
    }

    public bool FireMissile()
    {
        if (Time.time > lastMissileFireTime + missileFireRate)
        {
            if (missilePrefab != null && firePoint != null)
            {
                Object.Instantiate(missilePrefab, firePoint.position, firePoint.rotation);
                lastMissileFireTime = Time.time;
                return true;
            }
            else
            {
                Debug.LogWarning("Missile Prefab or FirePoint not assigned to WeaponHandler");
            }
        }
        return false;
    }

    public void UpdateCooldowns(float deltaTime)
    {
        // This method isn't strictly necessary for this implementation,
        // but follows the pattern in your reference code
    }
}