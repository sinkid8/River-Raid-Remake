using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private FuelManager fuelManager;
    [SerializeField] private TextMeshProUGUI levelText;

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
    [SerializeField] private TextMeshProUGUI startMessage; // TextMeshPro for the start message

    private Rigidbody2D rb;
    private ShipMovementHandler movementHandler;
    private WeaponHandler weaponHandler;
    private Vector2 currentInputDirection;
    private bool energyWeaponReady = false;
    private bool gameStarted = false; // Track whether the game has started

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Make sure the ship starts stationary
        rb.linearVelocity = Vector2.zero;  // Set initial velocity to zero

        if (firePoint == null)
        {
            firePoint = transform;
        }

        // Initialize the WeaponHandler with the FuelManager reference
        weaponHandler = new WeaponHandler(laserPrefab, energyWeaponPrefab,
                                          firePoint, laserFireRate, energyWeaponFireRate, fuelManager);

        movementHandler = new ShipMovementHandler(rb, moveSpeed, 0f, verticalDriftAmount);

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

        // Show the start message
        if (startMessage != null)
        {
            startMessage.gameObject.SetActive(true); // Make sure the message is visible initially
        }
    }

    private void HandleMoveInput(Vector2 input)
    {
        if (!gameStarted)
            return; // Do not handle movement if the game hasn't started

        currentInputDirection = input;
        movementHandler.UpdateInput(input);
    }

    private void HandleFireLaser()
    {
        if (!gameStarted)
            return; // Don't fire if the game hasn't started

        weaponHandler.FireLaser();
    }

    private void HandleFireEnergyWeapon()
    {
        if (!gameStarted)
            return; // Don't fire if the game hasn't started

        if (fuelManager.GetCurrentFuelLevel() >= 2) // Check if fuel is at least 50% (2 bars)
        {
            if (weaponHandler.FireEnergyWeapon())
            {
                // Use 50% of the fuel (2 bars)
                if (fuelManager.UseHalfFuel())
                {
                    // Energy weapon is no longer ready until fuel is at least 2 bars
                    energyWeaponReady = false;
                    UpdateEnergyWeaponUI();
                }
            }
        }
        else
        {
            Debug.Log("Not enough fuel to fire energy weapon. Need at least 2 bars.");
        }
    }

    private void OnEnergyWeaponReady()
    {
        // Energy weapon is ready when fuel is full (4 bars)
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

    // Called when any key is pressed to start the game
    private void Update()
    {
        if (!gameStarted && Input.anyKeyDown)
        {
            gameStarted = true;
            if (startMessage != null)
            {
                startMessage.gameObject.SetActive(false); // Hide the start message
            }

            movementHandler.StartGame(); // Inform the movement handler that the game has started
        }
        Vector3 currentPosition = transform.position;
        if (currentPosition.y > 45)
        {
            Debug.Log("y position is 45, activating level text.");
            // Activate the text when the player reaches Y = 45
            if (levelText != null)
            {
                levelText.gameObject.SetActive(true); // Set the text object active
                Destroy(levelText.gameObject, 2f); // Destroy it after 2 seconds
            }
        }
    }
}
