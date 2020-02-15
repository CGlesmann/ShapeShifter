using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public static class AdManager
{
    private static int roundCounter = 5;

    public static void CheckForAutomaticAd(int minutesPassed, string nextScene)
    {
        if (minutesPassed >= 4)
        {
            ShowAutomaticAd(nextScene);
            ResetCounters();
        }
        else
        {
            roundCounter -= 1;
            if (roundCounter <= 0)
            {
                ShowAutomaticAd(nextScene);
                ResetCounters();
            }
            else
                SceneManager.LoadScene(nextScene);
        }
    }

    private static void ShowAutomaticAd(string nextScene)
    {
        if (Advertisement.IsReady("video"))
        {
            ShowOptions showOptions = new ShowOptions();
            showOptions.resultCallback += ctx => GameManager.manager.ResumeGame();

            Advertisement.Show("video", showOptions);
            SceneManager.LoadScene(nextScene);
            GameManager.manager.PauseGame();
        }
    }

    private static void ResetCounters()
    {
        roundCounter = 5;
    }
}
