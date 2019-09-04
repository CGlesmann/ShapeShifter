using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("Title Options")]
    [SerializeField] private string playMenu = ""; // Menu to navigate to when user selects "Play"

    private void Awake()
    {
        // Setting frame count to 30fps
        Application.targetFrameRate = 30;
    }

    /// <summary>
    /// Navigates to the playMenu
    /// </summary>
    public void BeginPlay() { SceneManager.LoadScene(playMenu); }
}
