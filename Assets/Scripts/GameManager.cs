using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private InputManager inputManager;

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

        // Other reset operations can be added here

        // Example: Respawn player if needed
        // if (playerShip != null) playerShip.Respawn();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}