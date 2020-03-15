using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    public static string previousMenu = "";
    public static void SetPreviousMenu(string sceneName) { previousMenu = sceneName; }

    public void ExitToPreviousMenu() { SceneManager.LoadScene(previousMenu); }
    public void ResetGameData() { DataTracker.dataTracker.ResetSaveData(); }
}

