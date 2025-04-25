using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject[] additionalPanels;

    [Header("Buttons")]
    public Button[] menuButtons;

    [Header("High Score Display")]
    public TextMeshProUGUI highScoreLevel1Text;
    public TextMeshProUGUI highScoreLevel2Text;
    public TextMeshProUGUI highScoreLevel3Text;

    private GameObject currentPanel;
    private GameObject previousPanel;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // Assign each button to open its corresponding panel
        for (int i = 0; i < menuButtons.Length; i++)
        {
            int index = i; // Local copy for closure
            menuButtons[i].onClick.AddListener(() => ShowPanel(additionalPanels[index]));
        }

        currentPanel = mainPanel;
        previousPanel = null;

        ShowPanel(mainPanel);
        UpdateAllHighScoreTexts();
    }

    /// <summary>
    /// Updates high score texts for all levels using PlayerPrefs.
    /// </summary>
    public void UpdateAllHighScoreTexts()
    {
        highScoreLevel1Text.text = $"Level 1: {PlayerPrefs.GetInt("HighScore_Level_1", 0)}";
        highScoreLevel2Text.text = $"Level 2: {PlayerPrefs.GetInt("HighScore_Level_2", 0)}";
        highScoreLevel3Text.text = $"Level 3: {PlayerPrefs.GetInt("HighScore_Level_3", 0)}";
    }

    /// <summary>
    /// Activates the target panel and deactivates others.
    /// </summary>
    /// <param name="targetPanel">Panel to be shown.</param>
    public void ShowPanel(GameObject targetPanel)
    {
        if (targetPanel != currentPanel)
        {
            previousPanel = currentPanel;
            currentPanel = targetPanel;
        }

        DeactivateAllPanels();
        targetPanel.SetActive(true);
    }

    /// <summary>
    /// Deactivates all registered panels.
    /// </summary>
    private void DeactivateAllPanels()
    {
        if (mainPanel != null) mainPanel.SetActive(false);

        foreach (GameObject panel in additionalPanels)
        {
            if (panel != null) panel.SetActive(false);
        }
    }

    /// <summary>
    /// Goes back to the main panel.
    /// </summary>
    public void OnBackButtonPressed()
    {
        ShowPanel(mainPanel);
    }

    /// <summary>
    /// Shortcut back navigation (if used by other buttons).
    /// </summary>
    public void OnBACK01()
    {
        ShowPanel(mainPanel);
    }

    /// <summary>
    /// Loads a scene based on its build index.
    /// </summary>
    /// <param name="sceneIndex">Scene build index.</param>
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
