using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("Title Options")]
    [SerializeField] private string playMenu = "";
    [SerializeField] private string optionsMenu = "";
    [SerializeField] private string tutorialLevel = "";

    private void Start() { Application.targetFrameRate = 60; DataTracker.dataTracker.LoadData(); }

    public void BeginPlay() { SceneManager.LoadScene(DataTracker.gameData.initialTutorialComplete ? playMenu : tutorialLevel); }
    public void NaviagteToOptions()
    {
        OptionManager.SetPreviousMenu(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(optionsMenu);
    }
}
