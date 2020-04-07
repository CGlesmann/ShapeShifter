using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Animator sceneAnimator = null;

    [Header("Title Options")]
    [SerializeField] private string playMenu = "";
    [SerializeField] private string optionsMenu = "";
    [SerializeField] private string tutorialLevel = "";

    private delegate void OnExitFinish(string levelName);
    private event OnExitFinish onExitFinish = null;
    private string targetSceneName = "";

    private void Start()
    {
        Application.targetFrameRate = 60;
        DataTracker.dataTracker.LoadData();
    }

    public void BeginPlay()
    {
        targetSceneName = DataTracker.gameData.initialTutorialComplete ? playMenu : tutorialLevel;
        onExitFinish += SceneManager.LoadScene;

        sceneAnimator.SetTrigger("Exit");
    }

    public void NaviagteToOptions()
    {
        OptionManager.SetPreviousMenu(SceneManager.GetActiveScene().name);

        targetSceneName = optionsMenu;
        onExitFinish += SceneManager.LoadScene;

        sceneAnimator.SetTrigger("Exit");
    }

    public void ExecuteSceneExit() { onExitFinish?.Invoke(targetSceneName); }
}
