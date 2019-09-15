using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Manager Variables")]
    [SerializeField] private string mainMenuScene = "";
    [SerializeField] private string instructionsScene = "";

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

    public void NavigateToInstructions() { SceneManager.LoadScene(instructionsScene); }
}
