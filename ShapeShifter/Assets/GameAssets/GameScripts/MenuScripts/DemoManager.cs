using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoManager : MonoBehaviour
{
    [Header("Target Scene")]
    [SerializeField] private string targetLevel = "";

    public void NavigateToMenu() { SceneManager.LoadScene(targetLevel); }
}
