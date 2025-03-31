using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    
    [SerializeField] private TextMeshProUGUI scoreText;
    
    private int currentScore = 0;
    
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
    }
    
    private void Start()
    {
        // Try to find the score text if not assigned
        if (scoreText == null)
        {
            FindScoreText();
        }
        
        // Initialize the score display
        UpdateScoreDisplay();
    }
    
    private void FindScoreText()
    {
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        
        foreach (Canvas canvas in allCanvases)
        {
            // Try to find by direct children first
            Transform scoreTextTransform = canvas.transform.Find("ScoreText");
            
            if (scoreTextTransform != null)
            {
                scoreText = scoreTextTransform.GetComponent<TextMeshProUGUI>();
                if (scoreText != null)
                {
                    Debug.Log("Found score text directly in canvas");
                    return;
                }
            }
            
            // If not found, search all children recursively
            scoreText = FindTextRecursive(canvas.transform, "ScoreText");
            
            if (scoreText != null)
            {
                Debug.Log("Found score text in canvas hierarchy");
                return;
            }
        }
        
        // If we still haven't found it, create one
        CreateScoreText();
    }
    
    private TextMeshProUGUI FindTextRecursive(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            
            if (child.name == name)
            {
                TextMeshProUGUI text = child.GetComponent<TextMeshProUGUI>();
                if (text != null)
                {
                    return text;
                }
            }
            
            // Search child's children
            if (child.childCount > 0)
            {
                TextMeshProUGUI text = FindTextRecursive(child, name);
                if (text != null)
                {
                    return text;
                }
            }
        }
        
        return null;
    }
    
    private void CreateScoreText()
    {
        // Find a canvas to add the score text to
        Canvas canvas = FindObjectOfType<Canvas>();
        
        if (canvas != null)
        {
            // Create a new GameObject for the score text
            GameObject scoreTextObject = new GameObject("ScoreText");
            scoreTextObject.transform.SetParent(canvas.transform, false);
            
            // Add TextMeshProUGUI component
            scoreText = scoreTextObject.AddComponent<TextMeshProUGUI>();
            
            // Set up RectTransform
            RectTransform rectTransform = scoreText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 1); // Top right
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(-10, -10); // 10 pixels from top right
            rectTransform.sizeDelta = new Vector2(300, 50);
            
            // Configure text
            scoreText.alignment = TextAlignmentOptions.Right;
            scoreText.fontSize = 44;
            
            Debug.Log("Created new score text object");
        }
        else
        {
            Debug.LogError("No canvas found in scene to create score text");
        }
    }
    
    // Add points to the score
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
    }
    
    // Update the UI display
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }
    
    // Get the current score
    public int GetScore()
    {
        return currentScore;
    }
    
    // Reset the score to zero
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }
}