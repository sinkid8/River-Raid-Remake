using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject controlsPanel1;  // Reference to Controls Panel 1
    [SerializeField] private GameObject controlsPanel2;  // Reference to Controls Panel 2
    [SerializeField] private GameObject controlsPanel3;  // Reference to Controls Panel 3

    private int currentPanelIndex = 0;
    private GameObject[] panels;

    private void Start()
    {
        panels = new GameObject[] { controlsPanel1, controlsPanel2, controlsPanel3 };
        HideAllPanels();  // Hide all panels at the start
    }

    // Start the game by loading Level 1
    public void StartGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    // Show a specific panel
    public void ShowPanel(int panelIndex)
    {
        HideAllPanels();  // Hide other panels
        if (panelIndex >= 0 && panelIndex < panels.Length)
        {
            panels[panelIndex].SetActive(true);
            currentPanelIndex = panelIndex;
        }
    }

    // Go to the next panel
    public void NextPanel()
    {
        currentPanelIndex = (currentPanelIndex + 1) % panels.Length;
        ShowPanel(currentPanelIndex);
    }

    // Go to the previous panel
    public void PreviousPanel()
    {
        currentPanelIndex--;
        if (currentPanelIndex < 0)
        {
            currentPanelIndex = panels.Length - 1;
        }
        ShowPanel(currentPanelIndex);
    }

    // Hide all panels
    private void HideAllPanels()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    // Hide the controls (if you want to hide controls with a button click)
    public void HideControls()
    {
        HideAllPanels();  // Hide all panels when controls are closed
    }

    // Quit the game (optional)
    public void QuitGame()
    {
        Application.Quit();
    }
}
