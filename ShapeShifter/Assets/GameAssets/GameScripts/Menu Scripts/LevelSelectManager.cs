using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectManager : HorizontalPanelController
{

    [Header("Manager Variables")]
    [SerializeField] protected string mainMenuScene = "";
    [SerializeField] protected string instructionsScene = "";
    [SerializeField] protected string optionsScene = "";
    public int levelPackIndex = 0;

    [Header("Level GUI References")]
    [SerializeField] private GameObject levelPreviewPanel = null;
    [SerializeField] private TextMeshProUGUI levelNameText = null;
    private string currentLevel = "";

    #region Navigation Methods
    /// <summary>
    /// Navigates to Main Menu
    /// Used by the back button
    /// </summary>
    public void NavigateToMainMenu() { SceneManager.LoadScene(mainMenuScene); }

    public void DisplayLevelPreview(string levelName)
    {
        // Storing the input
        currentLevel = levelName;

        // Displaying The Level Investigation Screen
        levelPreviewPanel.SetActive(true);
        levelNameText.text = levelName.Replace('_', ' ');
    }

    public void HideLevelPreview()
    {
        currentLevel = "";
        levelPreviewPanel.SetActive(false);
    }

    /// <summary>
    /// Navigate to the selected level
    /// </summary>
    /// <param name="levelName"></param>
    public void NavigateToLevel() { SceneManager.LoadScene(currentLevel); }

    /// <summary>
    /// Navigates to the instructions screen
    /// </summary>
    public void NavigateToInstructions() { SceneManager.LoadScene(instructionsScene); }
    
    /// <summary>
    /// Navigates to the options menu
    /// </summary>
    public void NavigateToOptions() { SceneManager.LoadScene(optionsScene); }
    #endregion
}
