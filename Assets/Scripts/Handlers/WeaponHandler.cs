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
                // Spawn the projectile slightly in front of the fire point to avoid collision with the ship
                Vector3 spawnPosition = firePoint.position + firePoint.up * 0.5f;
                Object.Instantiate(laserPrefab, spawnPosition, firePoint.rotation);
                
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
        // Ensure enough fuel for the energy weapon (at least 2 bars)
        if (Time.time > lastEnergyWeaponFireTime + energyWeaponFireRate)
        {
            if (energyWeaponPrefab != null && firePoint != null)
            {
                // Check if fuel is sufficient (at least 2 bars)
                if (fuelManager != null && fuelManager.GetCurrentFuelLevel() >= 2) // Only fire if fuel is at least 50%
                {
                    // Spawn the energy projectile slightly in front of the fire point
                    Vector3 spawnPosition = firePoint.position + firePoint.up * 0.5f;
                    Object.Instantiate(energyWeaponPrefab, spawnPosition, firePoint.rotation);

                    // Use half the fuel (2 bars)
                    if (!fuelManager.UseHalfFuel())
                    {
                        Debug.Log("Not enough fuel to fire energy weapon.");
                        return false; // Don't fire if fuel is insufficient
                    }

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

    public void UpdateCooldowns(float deltaTime)
    {
        // This method isn't strictly necessary for this implementation,
        // but follows the pattern in your reference code
    }
}