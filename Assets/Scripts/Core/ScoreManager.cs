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
        if (scoreText == null)
        {
            FindScoreText();
        }
        
        UpdateScoreDisplay();
    }
    
    private void FindScoreText()
    {
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        
        foreach (Canvas canvas in allCanvases)
        {
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
            
            scoreText = FindTextRecursive(canvas.transform, "ScoreText");
            
            if (scoreText != null)
            {
                Debug.Log("Found score text in canvas hierarchy");
                return;
            }
        }
        
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
        Canvas canvas = FindObjectOfType<Canvas>();
        
        if (canvas != null)
        {
            GameObject scoreTextObject = new GameObject("ScoreText");
            scoreTextObject.transform.SetParent(canvas.transform, false);
            
            scoreText = scoreTextObject.AddComponent<TextMeshProUGUI>();
            
            RectTransform rectTransform = scoreText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(-95, -80);
            rectTransform.sizeDelta = new Vector2(200, 50);

            scoreText.alignment = TextAlignmentOptions.Right;
            scoreText.fontSize = 24;
            
            Debug.Log("Created new score text object");
        }
        else
        {
            Debug.LogError("No canvas found in scene to create score text");
        }
    }
    
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreDisplay();
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    public int GetScore()
    {
        return currentScore;
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }
}