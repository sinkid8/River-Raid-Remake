using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private LivesManager livesManager; // Reference to LivesManager

    private int score = 0;

    public static GameManager Instance;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Ensure this object persists between scenes if needed
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Subscribe to reset event
        inputManager.OnResetPressed.AddListener(ResetGame);

        // Initialize score display
        UpdateScoreUI();
        
        // Ensure we have a LivesManager
        if (livesManager == null)
        {
            livesManager = FindFirstObjectByType<LivesManager>();
            if (livesManager == null)
            {
                GameObject livesObj = new GameObject("LivesManager");
                livesManager = livesObj.AddComponent<LivesManager>();
            }
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    private void ResetGame()
    {
        // Reset score
        score = 0;
        UpdateScoreUI();

        // Tell the LivesManager to reset the level
        if (livesManager != null)
        {
            livesManager.ResetLevel();
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}