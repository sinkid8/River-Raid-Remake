using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject controlsPanel;  // Reference to the Controls panel

    private void Start()
    {
        HideControls(); // Hide controls at start
    }

    // Start the game by loading Level 1
    public void StartGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    // Show the controls panel
    public void ShowControls()
    {
        controlsPanel.SetActive(true);
    }

    // Hide the controls panel
    public void HideControls()
    {
        controlsPanel.SetActive(false);
    }

    // Quit the game (optional)
    public void QuitGame()
    {
        Application.Quit();
    }
}
