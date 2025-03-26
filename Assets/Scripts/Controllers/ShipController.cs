using UnityEngine;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private FuelManager fuelManager;

    // Movement parameters
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float movementDampening = 0.9f;
    [SerializeField] private float verticalDriftAmount = 0.5f;

    // Weapon parameters
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject energyWeaponPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float laserFireRate = 0.25f;
    [SerializeField] private float energyWeaponFireRate = 2f; 

    // Energy weapon UI feedback
    [SerializeField] private Image energyReadyIndicator;

    private Rigidbody2D rb;
    private ShipMovementHandler movementHandler;
    private WeaponHandler weaponHandler;
    private Vector2 currentInputDirection;
    private bool energyWeaponReady = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (firePoint == null)
        {
            firePoint = transform;
        }

        movementHandler = new ShipMovementHandler(rb, moveSpeed, 0f, verticalDriftAmount);
        weaponHandler = new WeaponHandler(laserPrefab, energyWeaponPrefab,
                                          firePoint, laserFireRate, energyWeaponFireRate);

        // Register input handlers
        inputManager.OnMove.AddListener(HandleMoveInput);
        inputManager.OnFireLaser.AddListener(HandleFireLaser);
        inputManager.OnFireEnergyWeapon.AddListener(HandleFireEnergyWeapon);

        // Register fuel events if fuel manager exists
        if (fuelManager != null)
        {
            // When fuel is full, energy weapon becomes ready
            fuelManager.OnFuelFull.AddListener(OnEnergyWeaponReady);

            // Check initial state
            if (fuelManager.IsFuelFull())
            {
                OnEnergyWeaponReady();
            }
        }

        // Initialize UI state if present
        UpdateEnergyWeaponUI();
    }

    private void HandleMoveInput(Vector2 input)
    {
        currentInputDirection = input;
        movementHandler.UpdateInput(input);
    }

    private void HandleFireLaser()
    {
        weaponHandler.FireLaser();
    }

    private void HandleFireEnergyWeapon()
    {
        if (energyWeaponReady && fuelManager != null)
        {
            if (weaponHandler.FireEnergyWeapon())
            {
                // Use half of the fuel
                fuelManager.UseHalfFuel();

                // Energy weapon is no longer ready until fuel is full again
                energyWeaponReady = false;

                // Update the UI
                UpdateEnergyWeaponUI();
            }
        }
        else
        {
            // Optional: Audio or visual feedback that energy weapon isn't ready
            Debug.Log("Energy Weapon not ready - need full fuel!");
        }
    }

    private void OnEnergyWeaponReady()
    {
        energyWeaponReady = true;
        UpdateEnergyWeaponUI();

        // Optional: Audio or visual feedback that energy weapon is ready
        Debug.Log("Energy Weapon Ready!");
    }

    private void UpdateEnergyWeaponUI()
    {
        if (energyReadyIndicator != null)
        {
            // Update the energy weapon indicator (glowing when ready, dim when not)
            energyReadyIndicator.color = energyWeaponReady ?
                                        Color.cyan :
                                        new Color(0.2f, 0.2f, 0.2f, 0.5f);
        }
    }

    private void FixedUpdate()
    {
        movementHandler.Move();
        movementHandler.ApplyDampening(movementDampening);
    }
}