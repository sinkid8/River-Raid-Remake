using UnityEngine;

public class WeaponHandler
{
    private GameObject laserPrefab;
    private GameObject energyWeaponPrefab; 
    private Transform firePoint;
    private float laserFireRate;
    private float energyWeaponFireRate; 

    private float lastLaserFireTime = 0f;
    private float lastEnergyWeaponFireTime = 0f;

    private FuelManager fuelManager;

    public WeaponHandler(GameObject laserPrefab, GameObject energyWeaponPrefab,
                          Transform firePoint, float laserFireRate, float energyWeaponFireRate, FuelManager fuelManager)
    {
        this.laserPrefab = laserPrefab;
        this.energyWeaponPrefab = energyWeaponPrefab;
        this.firePoint = firePoint;
        this.laserFireRate = laserFireRate;
        this.energyWeaponFireRate = energyWeaponFireRate;
        this.fuelManager = fuelManager;
    }

    public void UpdateFuelManager(FuelManager newFuelManager)
    {
        this.fuelManager = newFuelManager;
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
                if (fuelManager != null && fuelManager.GetCurrentFuelLevel() >= 2)
                {
                    Object.Instantiate(energyWeaponPrefab, firePoint.position, firePoint.rotation);

                    fuelManager.UseExactFuel(2);
                    
                    lastEnergyWeaponFireTime = Time.time;
                    return true;
                }
                else
                {
                    Debug.Log("Not enough fuel to fire energy weapon.");
                }
            }
            else
            {
                Debug.LogWarning("Energy Weapon Prefab or FirePoint not assigned to WeaponHandler");
            }
        }
        return false;
    }
}