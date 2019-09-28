using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayManager : MonoBehaviour
{
    [Header("Navigation Variables")]
    [SerializeField] private string menuScene = "";

    public void NavigateToMenu() { SceneManager.LoadScene(menuScene); }
}
