using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("Title Options")]
    [SerializeField] private string playMenu = ""; // Menu to navigate to when user selects "Play"
    [SerializeField] private string tutorialLevel = ""; // Level that contains the FUE

    /// <summary>
    /// Sets the default framerate
    /// </summary>
    private void Awake()
    {
        // Setting frame count to 30fps
        Application.targetFrameRate = 24;
    }

    private void Start()
    {
        // Loading Existing Data
        DataTracker.dataTracker.LoadData();
    }

    /// <summary>
    /// Navigates to the playMenu
    /// </summary>
    public void BeginPlay() { SceneManager.LoadScene(DataTracker.gameData.initialTutorialComplete ? playMenu : tutorialLevel); }
}
