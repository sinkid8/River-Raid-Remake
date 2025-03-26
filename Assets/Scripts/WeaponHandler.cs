using UnityEngine;

public class WeaponHandler
{
    private GameObject laserPrefab;
    private GameObject energyWeaponPrefab; // New energy weapon prefab
    private Transform firePoint;
    private float laserFireRate;
    private float energyWeaponFireRate; // Energy weapon fire rate

    private float lastLaserFireTime = 0f;
    private float lastEnergyWeaponFireTime = 0f;

    public WeaponHandler(GameObject laserPrefab, GameObject energyWeaponPrefab,
                          Transform firePoint, float laserFireRate, float energyWeaponFireRate)
    {
        this.laserPrefab = laserPrefab;
        this.energyWeaponPrefab = energyWeaponPrefab;
        this.firePoint = firePoint;
        this.laserFireRate = laserFireRate;
        this.energyWeaponFireRate = energyWeaponFireRate;
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

    public bool FireEnergyWeapon()
    {
        if (Time.time > lastEnergyWeaponFireTime + energyWeaponFireRate)
        {
            if (energyWeaponPrefab != null && firePoint != null)
            {
                Object.Instantiate(energyWeaponPrefab, firePoint.position, firePoint.rotation);
                lastEnergyWeaponFireTime = Time.time;
                return true;
            }
            else
            {
                Debug.LogWarning("Energy Weapon Prefab or FirePoint not assigned to WeaponHandler");
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