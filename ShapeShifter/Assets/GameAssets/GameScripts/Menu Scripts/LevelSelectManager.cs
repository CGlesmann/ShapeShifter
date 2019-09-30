using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Manager Variables")]
    [SerializeField] private string mainMenuScene = "";
    [SerializeField] private string instructionsScene = "";
    [SerializeField] private string optionsScene = "";

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI levelCountText = null;

    private void Awake() { levelCountText.text = string.Format("Levels Completed: {0}", DataTracker.gameData.levelsCompleted.ToString()); }

    /// <summary>
    /// Navigates to Main Menu
    /// Used by the back button
    /// </summary>
    public void NavigateToMainMenu() { SceneManager.LoadScene(mainMenuScene); }

    /// <summary>
    /// Navigate to the selected level
    /// </summary>
    /// <param name="levelName"></param>
    public void NavigateToLevel(string levelName) { SceneManager.LoadScene(levelName); }

    /// <summary>
    /// Navigates to the instructions screen
    /// </summary>
    public void NavigateToInstructions() { SceneManager.LoadScene(instructionsScene); }
    
    /// <summary>
    /// Navigates to the options menu
    /// </summary>
    public void NavigateToOptions() { SceneManager.LoadScene(optionsScene); }
}
