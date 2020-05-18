using UnityEngine;
using UnityEngine.SceneManagement;

public class OptionManager : MonoBehaviour
{
    private string menuToNavigateTo = "";

    [Header("Object References")]
    [SerializeField] private Animator anim = null;

    public static string previousMenu = "";
    public static void SetPreviousMenu(string sceneName) { previousMenu = sceneName; }

    public void ExitToPreviousMenu() { menuToNavigateTo = previousMenu; StartExitAnimation(); }
    public void ResetGameData() { DataTracker.dataTracker.ResetSaveData(); }

    public void StartExitAnimation() { anim.SetTrigger("Exit"); }
    public void ExecuteAnimationFinish() { SceneManager.LoadScene(menuToNavigateTo); }
}

