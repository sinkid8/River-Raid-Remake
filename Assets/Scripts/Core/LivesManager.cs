using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Cinemachine;

public class LivesManager : MonoBehaviour
{
    [SerializeField] public int maxLives = 3; // Changed to public for access from ShipController
    [SerializeField] private GameObject shipPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] public TextMeshProUGUI livesText;
    [SerializeField] public GameObject gameOverPanel;
    [SerializeField] private float respawnDelay = 1.0f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    
    // References to necessary components
    [SerializeField] private InputManager inputManager;
    [SerializeField] private FuelManager fuelManager;
    
    // Reference to the modern Cinemachine camera
    [SerializeField] private CinemachineCamera cinemachineCamera;

    public int currentLives; // Changed to public for access from ShipController
    private GameObject currentShip;
    private bool isProcessingDeath = false; // Flag to prevent multiple death calls

    // Singleton pattern
    public static LivesManager Instance;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Find references if not set
        if (inputManager == null)
            inputManager = FindFirstObjectByType<InputManager>();
            
        if (fuelManager == null)
            fuelManager = FindFirstObjectByType<FuelManager>();
            
        // Find Cinemachine camera if not set
        if (cinemachineCamera == null)
            cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
    }

    private void Start()
    {
        currentLives = maxLives;
        UpdateLivesUI();
        
        // Hide game over panel initially
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        // Spawn initial ship
        SpawnShip();
    }

    public void OnPlayerDeath()
    {
        // Prevent multiple death calls for the same ship
        if (isProcessingDeath)
            return;
            
        isProcessingDeath = true;
        
        Debug.Log("Player died. Remaining lives: " + (currentLives - 1));
        
        currentLives--;
        UpdateLivesUI();

        if (currentLives > 0)
        {
            // Schedule ship respawn after delay
            Invoke("SpawnShip", respawnDelay);
        }
        else
        {
            // Game over
            GameOver();
        }
    }

    private void SpawnShip()
    {
        // Reset the death processing flag
        isProcessingDeath = false;
        
        if (shipPrefab != null && spawnPoint != null)
        {
            // Spawn the ship
            currentShip = Instantiate(shipPrefab, spawnPoint.position, spawnPoint.rotation);
            
            // Connect the ship to required components
            ShipController shipController = currentShip.GetComponent<ShipController>();
            if (shipController != null)
            {
                // Connect to InputManager
                shipController.SetInputManager(inputManager);
                
                // Connect to FuelManager
                shipController.SetFuelManager(fuelManager);
            }
            else
            {
                Debug.LogError("Ship prefab does not have ShipController component!");
            }
            
            // Connect the ship's collision component to the lives manager
            ShipCollision shipCollision = currentShip.GetComponent<ShipCollision>();
            if (shipCollision != null)
            {
                // Set up fuel manager reference
                shipCollision.fuelManager = fuelManager;
                
                // Clear existing listeners to prevent duplicates
                shipCollision.OnShipDestroyed.RemoveAllListeners();
                
                // Set up an event listener for when the ship is destroyed
                shipCollision.OnShipDestroyed.AddListener(OnPlayerDeath);
            }
            else
            {
                Debug.LogError("Ship prefab does not have ShipCollision component!");
            }
            
            // Update Cinemachine camera to follow the new ship
            if (cinemachineCamera != null)
            {
                cinemachineCamera.Follow = currentShip.transform;
            }
            else
            {
                Debug.LogWarning("No Cinemachine camera found to update follow target!");
            }
        }
        else
        {
            Debug.LogError("Ship prefab or spawn point not assigned!");
        }
    }

    private void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {currentLives}";
        }
    }

    private void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        // Return to main menu after a delay
        Invoke("ReturnToMainMenu", 3.0f);
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Public method to reset the level (can be called from a button or event)
    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}