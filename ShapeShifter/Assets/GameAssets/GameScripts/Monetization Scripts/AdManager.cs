using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public static class AdManager
{
    private static int roundCounter = 3;

    public static void CheckForAutomaticAd(int minutesPassed, string nextScene)
    {
        if (minutesPassed >= 5)
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
            showOptions.resultCallback += ctx => SceneManager.LoadScene(nextScene);

            Advertisement.Show("video", showOptions);
        }
    }

    private static void ResetCounters()
    {
        roundCounter = 3;
    }
}
