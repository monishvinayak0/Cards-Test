
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NavigationManager : MonoBehaviour
{
    // Assign in Unity Inspector
    public GameObject mainPanel;
    public GameObject[] additionalPanels; // Use this to add more panels
    public Button[] menuButtons;

    public Button backButton;
    public GameObject currentPanel;
    public GameObject previousPanel;
    public TextMeshProUGUI highScoreLevel1Text;
    public TextMeshProUGUI highScoreLevel2Text;
    public TextMeshProUGUI highScoreLevel3Text;

    private void Start()
    {


        // Initialize panels

        Initialize();
       
    }

    private void Initialize()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            int index = i; 
            menuButtons[i].onClick.AddListener(() => ShowPanel(additionalPanels[index]));
        }

        currentPanel = mainPanel;
        previousPanel = null;
        ShowPanel(mainPanel);
        UpdateAllHighScoreTexts();
        backButton.onClick.AddListener(() => OnBackButtonPressed());
    }

    public void UpdateAllHighScoreTexts()
    {
        highScoreLevel1Text.text = $"Level 1: {PlayerPrefs.GetInt("HighScore_Level_1", 0)}";
        highScoreLevel2Text.text = $"Level 2: {PlayerPrefs.GetInt("HighScore_Level_2", 0)}";
        highScoreLevel3Text.text = $"Level 3: {PlayerPrefs.GetInt("HighScore_Level_3", 0)}";
    }


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


    public void LoadScene(int val)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(val);
    }

    private void DeactivateAllPanels()
    {
        if (mainPanel != null) mainPanel.SetActive(false);

        foreach (GameObject panel in additionalPanels)
        {
            if (panel != null) panel.SetActive(false);
        }
    }

    public void OnBackButtonPressed()
    {

        ShowPanel(mainPanel);
    }

    public void OnBACK01()
    {

        {
            ShowPanel(mainPanel);
        }
    }
}
