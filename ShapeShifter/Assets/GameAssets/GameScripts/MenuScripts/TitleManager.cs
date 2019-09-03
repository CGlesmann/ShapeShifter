using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("Title Options")]
    [SerializeField] private string playMenu = ""; // Menu to navigate to when user selects "Play"

    /// <summary>
    /// Navigates to the playMenu
    /// </summary>
    public void BeginPlay() { SceneManager.LoadScene(playMenu); }
}
