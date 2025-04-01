using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ShipController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private FuelManager fuelManager;
    
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
    [SerializeField] private float energyWeaponFireRate = 1f;

    // Energy weapon UI feedback
    [SerializeField] private Image energyReadyIndicator;

    private Rigidbody2D rb;
    private ShipMovementHandler movementHandler;
    private WeaponHandler weaponHandler;
    private Vector2 currentInputDirection;
    private bool energyWeaponReady = false;
    private bool gameStarted = false;
    private bool level2SoundPlayed = false; 
    private bool level3SoundPlayed = false;
    private bool driftIncreasedAtLevel2 = false;
    private bool driftIncreasedAtLevel3 = false;
    private bool shipStopped = false; 

    public void SetInputManager(InputManager newInputManager)
    {
        inputManager = newInputManager;
        
        if (inputManager != null)
        {
            inputManager.OnMove.AddListener(HandleMoveInput);
            inputManager.OnFireLaser.AddListener(HandleFireLaser);
            inputManager.OnFireEnergyWeapon.AddListener(HandleFireEnergyWeapon);
        }
    }

    public void SetFuelManager(FuelManager newFuelManager)
    {
        fuelManager = newFuelManager;
        
        if (fuelManager != null && weaponHandler != null)
        {
            weaponHandler.UpdateFuelManager(fuelManager);

            fuelManager.OnFuelFull.AddListener(OnEnergyWeaponReady);

            if (fuelManager.IsFuelFull())
            {
                OnEnergyWeaponReady();
            }
        }
    }

    private void Awake()
    {
        FindAllCanvasTexts();
    }

    private void Start()
    {
        Debug.Log($"BeginningText found: {beginningText != null}");
        Debug.Log($"Level1 found: {level1Text != null}");
        Debug.Log($"Level2 found: {level2Text != null}");
        Debug.Log($"Level3 found: {level3Text != null}");
        Debug.Log($"GameOverText found: {gameOverText != null}");
        Debug.Log($"GameEndText found: {gameEndText != null}");
        
        rb = GetComponent<Rigidbody2D>();

        rb.linearVelocity = Vector2.zero;

        if (firePoint == null)
        {
            firePoint = transform;
        }

        movementHandler = new ShipMovementHandler(rb, moveSpeed, 0f, verticalDriftAmount);

        weaponHandler = new WeaponHandler(laserPrefab, energyWeaponPrefab,
                                          firePoint, laserFireRate, energyWeaponFireRate, fuelManager);

        if (inputManager == null)
        {
            inputManager = FindFirstObjectByType<InputManager>();
        }
        
        if (inputManager != null)
        {
            inputManager.OnMove.AddListener(HandleMoveInput);
            inputManager.OnFireLaser.AddListener(HandleFireLaser);
            inputManager.OnFireEnergyWeapon.AddListener(HandleFireEnergyWeapon);
        }
        else
        {
            Debug.LogError("InputManager not found!");
        }

        if (fuelManager == null)
        {
            fuelManager = FindFirstObjectByType<FuelManager>();
        }
        
        if (fuelManager != null)
        {
            fuelManager.OnFuelFull.AddListener(OnEnergyWeaponReady);

            if (fuelManager.IsFuelFull())
            {
                OnEnergyWeaponReady();
            }
        }
        else
        {
            Debug.LogError("FuelManager not found!");
        }

        UpdateEnergyWeaponUI();

        gameStarted = false;

        SetupInitialUIState();
    }
    
    private void FindAllCanvasTexts()
    {
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        
        foreach (Canvas canvas in allCanvases)
        {
            beginningText = FindTextInTransform(canvas.transform, "BeginningText");
            level1Text = FindTextInTransform(canvas.transform, "Level 1");
            level2Text = FindTextInTransform(canvas.transform, "Level 2");
            level3Text = FindTextInTransform(canvas.transform, "Level 3");
            gameOverText = FindTextInTransform(canvas.transform, "GameOverText");
            gameEndText = FindTextInTransform(canvas.transform, "GameEndText");
            
            if (beginningText != null && level1Text != null && 
                level2Text != null && level3Text != null)
            {
                return;
            }
        }
        
        if (beginningText == null || level1Text == null || level2Text == null || level3Text == null ||
            gameOverText == null || gameEndText == null)
        {
            TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>();
            
            foreach (TextMeshProUGUI text in allTexts)
            {
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
    
    private TextMeshProUGUI FindTextInTransform(Transform parent, string name)
    {
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
            
            if (child.childCount > 0)
            {
                TextMeshProUGUI found2 = FindTextInTransform(child, name);
                if (found2 != null) return found2;
            }
        }
        
        return null;
    }
    
    private void SetupInitialUIState()
    {
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
            return;

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
        energyWeaponReady = true;
        UpdateEnergyWeaponUI();
    }

    private void UpdateEnergyWeaponUI()
    {
        if (energyReadyIndicator != null)
        {
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

    private void Update()
    {
        if (!gameStarted && Input.anyKeyDown && shipStopped == false)
        {
            gameStarted = true;
            
            if (beginningText != null)
            {
                beginningText.gameObject.SetActive(false);
                Debug.Log("Hiding BeginningText");
            }
            
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

            movementHandler.StartGame();
        }

        Vector3 currentPosition = transform.position;

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

        if (currentPosition.y > 45 && !driftIncreasedAtLevel2)
        {
            movementHandler.IncreaseVerticalDrift(1f);
            driftIncreasedAtLevel2 = true;
        }

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

        if (currentPosition.y > 105 && !driftIncreasedAtLevel3)
        {
            movementHandler.IncreaseVerticalDrift(1f);
            driftIncreasedAtLevel3 = true;
        }
        if (currentPosition.y > 145 && !shipStopped)
        {
            Debug.Log("Ship went off-screen. Resetting the game.");
            movementHandler.StopMovement();
            shipStopped = true;
            gameOverText.gameObject.SetActive(true);
            gameEndText.gameObject.SetActive(true);
        }

        if (Input.anyKeyDown && shipStopped)
        {
            gameOverText.gameObject.SetActive(false);
            gameEndText.gameObject.SetActive(false);
            SceneManager.LoadScene("Main Menu");
        }
    }

    private IEnumerator DeactivateTextAfterDelay(GameObject textObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        textObject.SetActive(false);
    }

}