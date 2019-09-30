using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    [Header("Navigation Options")]
    [SerializeField] private string menuNavigation = "";

    /// <summary>
    /// Invokes Navigation to the Level Select Screen
    /// </summary>
    public void ExitToMenu() { SceneManager.LoadScene(menuNavigation); }

    /// <summary>
    /// Resets all game data
    /// </summary>
    public void ResetGameData() { DataTracker.dataTracker.ResetSaveData(); }
}

