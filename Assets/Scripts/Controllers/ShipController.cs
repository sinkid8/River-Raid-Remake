using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShipController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private FuelManager fuelManager;
    [SerializeField] private TextMeshProUGUI level1Text;
    [SerializeField] private TextMeshProUGUI level2Text;
    [SerializeField] private TextMeshProUGUI level3Text;

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
    private bool level2SoundPlayed = false; // Flag to track if the level 2 sound has been played
    private bool level3SoundPlayed = false; // Flag to track if the level 3 sound has been played
    private bool driftIncreasedAtLevel2 = false;
    private bool driftIncreasedAtLevel3 = false;

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
                level1Text.gameObject.SetActive(true); // Show the level 1 text
                Destroy(level1Text.gameObject, 2f); // Destroy it after 2 seconds
            }

            movementHandler.StartGame(); // Inform the movement handler that the game has started
        }

        // Track ship position
        Vector3 currentPosition = transform.position;

        // Trigger level 2 at y = 45
        if (currentPosition.y > 45 && !level2SoundPlayed)
        {
            Debug.Log("y position is 45, welcome to level 2.");
            level2SoundPlayed = true;
            if (level2Text != null)
            {
                level2Text.gameObject.SetActive(true);
                Destroy(level2Text.gameObject, 2f);
            }

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayLevelupSound(AudioManager.instance.levelupClip);
            }
        }

        // Increase vertical drift when crossing y = 45
        if (currentPosition.y > 45 && !driftIncreasedAtLevel2)
        {
            movementHandler.IncreaseVerticalDrift(1f);
            driftIncreasedAtLevel2 = true;
        }

        // Trigger level 3 at y = 105
        if (currentPosition.y > 105 && !level3SoundPlayed)
        {
            Debug.Log("y position is 105, welcome to level 3.");
            level3SoundPlayed = true;
            if (level3Text != null)
            {
                level3Text.gameObject.SetActive(true);
                Destroy(level3Text.gameObject, 2f);
            }

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlayLevelupSound(AudioManager.instance.levelupClip);
            }
        }

        // Increase vertical drift when crossing y = 105
        if (currentPosition.y > 105 && !driftIncreasedAtLevel3)
        {
            movementHandler.IncreaseVerticalDrift(1f);
            driftIncreasedAtLevel3 = true;
        }
    }
}
