using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Cinemachine;

public class LivesManager : MonoBehaviour
{
    [SerializeField] public int maxLives = 3;
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

    public int currentLives; 
    private GameObject currentShip;
    private bool isProcessingDeath = false;

    public static LivesManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        if (inputManager == null)
            inputManager = FindFirstObjectByType<InputManager>();
            
        if (fuelManager == null)
            fuelManager = FindFirstObjectByType<FuelManager>();
            
        if (cinemachineCamera == null)
            cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
    }

    private void Start()
    {
        currentLives = maxLives;
        UpdateLivesUI();
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        SpawnShip();
    }

    public void OnPlayerDeath()
    {
        if (isProcessingDeath)
            return;
            
        isProcessingDeath = true;
        
        Debug.Log("Player died. Remaining lives: " + (currentLives - 1));
        
        currentLives--;
        UpdateLivesUI();

        if (currentLives > 0)
        {
            Invoke("SpawnShip", respawnDelay);
        }
        else
        {
            GameOver();
        }
    }

    private void SpawnShip()
    {
        isProcessingDeath = false;
        
        if (shipPrefab != null && spawnPoint != null)
        {
            currentShip = Instantiate(shipPrefab, spawnPoint.position, spawnPoint.rotation);
            
            ShipController shipController = currentShip.GetComponent<ShipController>();
            if (shipController != null)
            {
                shipController.SetInputManager(inputManager);

                shipController.SetFuelManager(fuelManager);
            }
            else
            {
                Debug.LogError("Ship prefab does not have ShipController component!");
            }
            
            ShipCollision shipCollision = currentShip.GetComponent<ShipCollision>();
            if (shipCollision != null)
            {
                shipCollision.fuelManager = fuelManager;

                shipCollision.OnShipDestroyed.RemoveAllListeners();

                shipCollision.OnShipDestroyed.AddListener(OnPlayerDeath);
            }
            else
            {
                Debug.LogError("Ship prefab does not have ShipCollision component!");
            }

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

        Invoke("ReturnToMainMenu", 3.0f);
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}