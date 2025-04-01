using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShipController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private FuelManager fuelManager;
    
    // Don't use SerializeField for these - we'll find them at runtime
    private TextMeshProUGUI beginningText;
    private TextMeshProUGUI level1Text;
    private TextMeshProUGUI level2Text;
    private TextMeshProUGUI level3Text;
    private TextMeshProUGUI gameOverText;
    private TextMeshProUGUI gameEndText;

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
    private bool gameStarted = false; // Track whether the game has started
    private bool level2SoundPlayed = false; // Flag to track if the level 2 sound has been played
    private bool level3SoundPlayed = false; // Flag to track if the level 3 sound has been played
    private bool driftIncreasedAtLevel2 = false;
    private bool driftIncreasedAtLevel3 = false;
    private bool shipStopped = false; // Flag to track if the ship has stopped

    // Method to set InputManager reference at runtime
    public void SetInputManager(InputManager newInputManager)
    {
        inputManager = newInputManager;
        
        if (inputManager != null)
        {
            // Re-register input handlers
            inputManager.OnMove.AddListener(HandleMoveInput);
            inputManager.OnFireLaser.AddListener(HandleFireLaser);
            inputManager.OnFireEnergyWeapon.AddListener(HandleFireEnergyWeapon);
        }
    }
    
    // Method to set FuelManager reference at runtime
    public void SetFuelManager(FuelManager newFuelManager)
    {
        fuelManager = newFuelManager;
        
        if (fuelManager != null && weaponHandler != null)
        {
            // Update the WeaponHandler with the new FuelManager
            weaponHandler.UpdateFuelManager(fuelManager);
            
            // Register fuel events
            fuelManager.OnFuelFull.AddListener(OnEnergyWeaponReady);
            
            // Check initial state
            if (fuelManager.IsFuelFull())
            {
                OnEnergyWeaponReady();
            }
        }
    }

    private void Awake()
    {
        // Find all UI elements very early in the lifecycle
        FindAllCanvasTexts();
    }

    private void Start()
    {
        // Debug log to confirm we found the UI elements
        Debug.Log($"BeginningText found: {beginningText != null}");
        Debug.Log($"Level1 found: {level1Text != null}");
        Debug.Log($"Level2 found: {level2Text != null}");
        Debug.Log($"Level3 found: {level3Text != null}");
        Debug.Log($"GameOverText found: {gameOverText != null}");
        Debug.Log($"GameEndText found: {gameEndText != null}");
        
        rb = GetComponent<Rigidbody2D>();

        // Make sure the ship starts stationary
        rb.linearVelocity = Vector2.zero;

        if (firePoint == null)
        {
            firePoint = transform;
        }

        // Initialize the movement handler with game not started
        movementHandler = new ShipMovementHandler(rb, moveSpeed, 0f, verticalDriftAmount);
        
        // Initialize the WeaponHandler with the FuelManager reference
        weaponHandler = new WeaponHandler(laserPrefab, energyWeaponPrefab,
                                          firePoint, laserFireRate, energyWeaponFireRate, fuelManager);

        // Check if we need to find the InputManager
        if (inputManager == null)
        {
            inputManager = FindFirstObjectByType<InputManager>();
        }
        
        if (inputManager != null)
        {
            // Register input handlers
            inputManager.OnMove.AddListener(HandleMoveInput);
            inputManager.OnFireLaser.AddListener(HandleFireLaser);
            inputManager.OnFireEnergyWeapon.AddListener(HandleFireEnergyWeapon);
        }
        else
        {
            Debug.LogError("InputManager not found!");
        }

        // Look for FuelManager if not set
        if (fuelManager == null)
        {
            fuelManager = FindFirstObjectByType<FuelManager>();
        }
        
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
        else
        {
            Debug.LogError("FuelManager not found!");
        }

        // Initialize UI state if present
        UpdateEnergyWeaponUI();

        // Always start with gameStarted = false and show the start message
        gameStarted = false;
        
        // Set up initial UI state
        SetupInitialUIState();
    }
    
    // Find all TextMeshProUGUI components in the Canvas
    private void FindAllCanvasTexts()
    {
        // First try to find by GameObject name in any canvas
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        
        foreach (Canvas canvas in allCanvases)
        {
            // Try to find by direct children first
            beginningText = FindTextInTransform(canvas.transform, "BeginningText");
            level1Text = FindTextInTransform(canvas.transform, "Level 1");
            level2Text = FindTextInTransform(canvas.transform, "Level 2");
            level3Text = FindTextInTransform(canvas.transform, "Level 3");
            gameOverText = FindTextInTransform(canvas.transform, "GameOverText");
            gameEndText = FindTextInTransform(canvas.transform, "GameEndText");
            
            // If we found all elements, we can stop searching
            if (beginningText != null && level1Text != null && 
                level2Text != null && level3Text != null)
            {
                return;
            }
        }
        
        // If we didn't find the elements by name, look for all TextMeshProUGUI components
        if (beginningText == null || level1Text == null || level2Text == null || level3Text == null ||
            gameOverText == null || gameEndText == null)
        {
            TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
            
            foreach (TextMeshProUGUI text in allTexts)
            {
                // Check for texts by their content or name
                if (beginningText == null && 
                    (text.name.Contains("Beginning") || text.text.Contains("Press Any Key")))
                {
                    beginningText = text;
                    Debug.Log($"Found BeginningText by content: {text.name}");
                }
                else if (level1Text == null && 
                         (text.name.Contains("Level 1") || text.text.Contains("Level 1")))
                {
                    level1Text = text;
                    Debug.Log($"Found Level 1 by content: {text.name}");
                }
                else if (level2Text == null && 
                         (text.name.Contains("Level 2") || text.text.Contains("Level 2")))
                {
                    level2Text = text;
                    Debug.Log($"Found Level 2 by content: {text.name}");
                }
                else if (level3Text == null && 
                         (text.name.Contains("Level 3") || text.text.Contains("Level 3")))
                {
                    level3Text = text;
                    Debug.Log($"Found Level 3 by content: {text.name}");
                }
                else if (gameOverText == null && 
                         (text.name.Contains("Game Over") || text.text.Contains("Game Over")))
                {
                    gameOverText = text;
                    Debug.Log($"Found GameOverText by content: {text.name}");
                }
                else if (gameEndText == null && 
                         (text.name.Contains("Game End") || text.text.Contains("Game End")))
                {
                    gameEndText = text;
                    Debug.Log($"Found GameEndText by content: {text.name}");
                }
            }
        }
    }
    
    // Helper method to find a text component by name
    private TextMeshProUGUI FindTextInTransform(Transform parent, string name)
    {
        // First try direct lookup
        Transform found = parent.Find(name);
        if (found != null)
        {
            TextMeshProUGUI text = found.GetComponent<TextMeshProUGUI>();
            if (text != null)
            {
                Debug.Log($"Found {name} directly in {parent.name}");
                return text;
            }
        }
        
        // Search through all children recursively
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
            {
                TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    Debug.Log($"Found {name} in hierarchy under {parent.name}");
                    return text;
                }
            }
            
            // Recursively search through this child
            if (child.childCount > 0)
            {
                TextMeshProUGUI found2 = FindTextInTransform(child, name);
                if (found2 != null) return found2;
            }
        }
        
        return null;
    }
    
    // Set up UI state at the start of the game
    private void SetupInitialUIState()
    {
        // Show beginning text, hide others
        if (beginningText != null)
        {
            beginningText.gameObject.SetActive(true);
        }
        
        if (level1Text != null)
        {
            level1Text.gameObject.SetActive(false);
        }
        
        if (level2Text != null)
        {
            level2Text.gameObject.SetActive(false);
        }
        
        if (level3Text != null)
        {
            level3Text.gameObject.SetActive(false);
        }
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
        if (gameEndText != null)
        {
            gameEndText.gameObject.SetActive(false);
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
            return;

        weaponHandler.FireLaser();
    }

private void HandleFireEnergyWeapon()
    {
        if (!gameStarted)
            return;

        if (fuelManager.GetCurrentFuelLevel() >= 2)
        {
            if (weaponHandler.FireEnergyWeapon())
            {
                energyWeaponReady = false;
                UpdateEnergyWeaponUI();
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
        // Always call the move handler, it will handle the gameStarted state internally
        movementHandler.Move();
        movementHandler.ApplyDampening(movementDampening);
    }

    // Called when any key is pressed to start the game
    private void Update()
    {
        if (!gameStarted && Input.anyKeyDown && shipStopped == false)
        {
            gameStarted = true;
            
            // Hide start message
            if (beginningText != null)
            {
                beginningText.gameObject.SetActive(false);
                Debug.Log("Hiding BeginningText");
            }
            
            // Show level 1 text
            if (level1Text != null)
            {
                level1Text.gameObject.SetActive(true);
                Debug.Log("Showing Level 1 text");
                StartCoroutine(DeactivateTextAfterDelay(level1Text.gameObject, 2f));
            }
            else
            {
                Debug.LogError("Level 1 text is null - can't show it!");
            }

            // Tell the movement handler the game has started
            movementHandler.StartGame();
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
                StartCoroutine(DeactivateTextAfterDelay(level2Text.gameObject, 2f));
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
                StartCoroutine(DeactivateTextAfterDelay(level3Text.gameObject, 2f));
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
        if (currentPosition.y > 145 && !shipStopped)
        {
            // Reset the game or perform any other action when the ship goes off-screen
            Debug.Log("Ship went off-screen. Resetting the game.");
            movementHandler.StopMovement(); // Stop the ship's movement
            shipStopped = true;
            gameOverText.gameObject.SetActive(true);
            gameEndText.gameObject.SetActive(true);
        }

        if (Input.anyKeyDown && shipStopped)
        {
            gameOverText.gameObject.SetActive(false);
            gameEndText.gameObject.SetActive(false);
            SceneManager.LoadScene("Main Menu"); // Reload the current scene
        }
    }

    private IEnumerator DeactivateTextAfterDelay(GameObject textObject, float delay)
    {
        yield return new WaitForSeconds(delay);  // Wait for the specified delay
        textObject.SetActive(false); // Deactivate the text object after the delay
    }

}