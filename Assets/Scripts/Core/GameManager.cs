using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private LivesManager livesManager;

    private int score = 0;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        inputManager.OnResetPressed.AddListener(ResetGame);
        UpdateScoreUI();

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
        score = 0;
        UpdateScoreUI();

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