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
    
    [SerializeField] private InputManager inputManager;
    [SerializeField] private FuelManager fuelManager;
    
    [SerializeField] private CinemachineCamera cinemachineCamera;

    public int currentLives; 
    private GameObject currentShip;
    private bool isProcessingDeath = false;
    private bool isFirstLoad = true;
    private int livesToRestoreAfterReload = 0;
    private bool isGameOver = false; 

    public static LivesManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentLives = maxLives;
            isGameOver = false;
        }
        else
        {
            if (Instance.livesToRestoreAfterReload > 0)
            {
                Instance.currentLives = Instance.livesToRestoreAfterReload;
                Instance.livesToRestoreAfterReload = 0;
            }
            isGameOver = Instance.isGameOver;
            
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == mainMenuSceneName)
        {
            isGameOver = false;
            currentLives = maxLives;
            return;
        }

        FindReferences();
        
        UpdateLivesUI();
        
        if (isGameOver)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                Invoke("ReturnToMainMenu", 3.0f);
            }
            else
            {
                Debug.LogError("Game over panel not found!");
            }
            return;
        }
        else if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        if (!isGameOver)
        {
            SpawnShip();
        }
        CleanupProjectiles();
    }
    
    private void FindReferences()
    {
        if (inputManager == null)
            inputManager = FindFirstObjectByType<InputManager>();
            
        if (fuelManager == null)
            fuelManager = FindFirstObjectByType<FuelManager>();
            
        if (cinemachineCamera == null)
            cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();

        if (livesText == null)
            livesText = GameObject.FindWithTag("LivesText")?.GetComponent<TextMeshProUGUI>();

        if (gameOverPanel == null)
            gameOverPanel = GameObject.FindWithTag("GameOverPanel");

        if (spawnPoint == null)
            spawnPoint = GameObject.FindWithTag("SpawnPoint")?.transform;
    }
    
    private void CleanupProjectiles()
    {
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject projectile in projectiles)
        {
            Destroy(projectile);
        }
    }

    private void Start()
    {
        if (isFirstLoad)
        {
            isFirstLoad = false;
        }

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
            livesToRestoreAfterReload = currentLives;
            
            Invoke("ReloadCurrentScene", respawnDelay);
        }
        else
        {
            GameOver();
        }
    }

    private void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        isGameOver = true;
        
        Debug.Log("Game Over triggered!");
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("Game Over panel activated");
        }
        else
        {
            Debug.LogError("Game over panel not found!");
            gameOverPanel = GameObject.FindWithTag("GameOverPanel");
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                Debug.Log("Game Over panel found and activated");
            }
        }

        Invoke("ReturnToMainMenu", 4.0f);
    }

    private void ReturnToMainMenu()
    {
        Debug.Log("Returning to main menu");
        isGameOver = false;
        currentLives = maxLives;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}