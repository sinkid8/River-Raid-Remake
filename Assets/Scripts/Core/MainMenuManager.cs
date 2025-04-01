using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject controlsPanel1;
    [SerializeField] private GameObject controlsPanel2;
    [SerializeField] private GameObject controlsPanel3;

    private int currentPanelIndex = 0;
    private GameObject[] panels;

    private void Start()
    {
        panels = new GameObject[] { controlsPanel1, controlsPanel2, controlsPanel3 };
        HideAllPanels();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level 1");
    }

    public void ShowPanel(int panelIndex)
    {
        HideAllPanels();
        if (panelIndex >= 0 && panelIndex < panels.Length)
        {
            panels[panelIndex].SetActive(true);
            currentPanelIndex = panelIndex;
        }
    }

    public void NextPanel()
    {
        currentPanelIndex = (currentPanelIndex + 1) % panels.Length;
        ShowPanel(currentPanelIndex);
    }

    public void PreviousPanel()
    {
        currentPanelIndex--;
        if (currentPanelIndex < 0)
        {
            currentPanelIndex = panels.Length - 1;
        }
        ShowPanel(currentPanelIndex);
    }

    private void HideAllPanels()
    {
        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }
    }

    public void HideControls()
    {
        HideAllPanels();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
