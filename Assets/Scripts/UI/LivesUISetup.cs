using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LivesUISetup : MonoBehaviour
{
    [SerializeField] private Canvas gameCanvas;
    [SerializeField] private TextMeshProUGUI livesTextPrefab;
    [SerializeField] private GameObject gameOverPanelPrefab;

    private void Awake()
    {
        if (gameCanvas == null)
        {
            // Create a canvas if it doesn't exist
            GameObject canvasObj = new GameObject("GameCanvas");
            gameCanvas = canvasObj.AddComponent<Canvas>();
            gameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // Create lives text if it doesn't exist
        if (livesTextPrefab != null && GameObject.FindFirstObjectByType<LivesManager>() != null)
        {
            TextMeshProUGUI livesText = Instantiate(livesTextPrefab, gameCanvas.transform);
            RectTransform livesRect = livesText.GetComponent<RectTransform>();
            livesRect.anchorMin = new Vector2(0, 1);
            livesRect.anchorMax = new Vector2(0, 1);
            livesRect.pivot = new Vector2(0, 1);
            livesRect.anchoredPosition = new Vector2(10, -10);
            
            // Assign to LivesManager
            LivesManager.Instance.GetComponent<LivesManager>().livesText = livesText;
        }

        // Create game over panel if it doesn't exist
        if (gameOverPanelPrefab != null && GameObject.FindFirstObjectByType<LivesManager>() != null)
        {
            GameObject gameOverPanel = Instantiate(gameOverPanelPrefab, gameCanvas.transform);
            RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = Vector2.zero;
            
            // Set up "Game Over" text
            TextMeshProUGUI gameOverText = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (gameOverText == null)
            {
                GameObject textObj = new GameObject("GameOverText");
                textObj.transform.SetParent(gameOverPanel.transform);
                gameOverText = textObj.AddComponent<TextMeshProUGUI>();
                RectTransform textRect = gameOverText.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0, 0);
                textRect.anchorMax = new Vector2(1, 1);
                textRect.pivot = new Vector2(0.5f, 0.5f);
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
            }
            
            gameOverText.text = "GAME OVER";
            gameOverText.fontSize = 48;
            gameOverText.alignment = TextAlignmentOptions.Center;
            gameOverText.color = Color.red;
            
            // Hide panel initially
            gameOverPanel.SetActive(false);
            
            // Assign to LivesManager
            LivesManager.Instance.GetComponent<LivesManager>().gameOverPanel = gameOverPanel;
        }
    }
}