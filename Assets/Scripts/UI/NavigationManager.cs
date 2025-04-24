
using UnityEngine;
using UnityEngine.UI;

public class NavigationManager : MonoBehaviour
{
    // Assign in Unity Inspector
    public GameObject mainPanel;
    public GameObject[] additionalPanels; // Use this to add more panels
    public Button[] menuButtons;

    public Button backButton;
    public GameObject currentPanel;
    public GameObject previousPanel;

    private void Start()
    {
        

        // Initialize panels
        currentPanel = mainPanel;
        previousPanel = null;
        ShowPanel(mainPanel);

        backButton.onClick.AddListener(() => OnBackButtonPressed());
    }

    private void Initialize()
    {
        for(int i = 0;i<menuButtons.Length;i++)
        {
            menuButtons[i].onClick.AddListener(() => ShowPanel(additionalPanels[i]));
        }
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
