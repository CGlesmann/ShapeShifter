using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    private static int roundCounter = 5;

    private void Awake()
    {
        Advertisement.AddListener(this);
    }

    public void CheckForAutomaticAd(int minutesPassed, string nextScene)
    {
        if (!Debug.isDebugBuild)
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
        } else
            SceneManager.LoadScene(nextScene);
    }

    private void ShowAutomaticAd(string nextScene)
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

    private void ResetCounters() { roundCounter = 5; }
    public void OnUnityAdsReady(string placementId)
    {
        Debug.Log("Ad Ready To Play");
        return;
    }

    public void OnUnityAdsDidError(string message)
    {
        Debug.LogError($"UnityAds Returned an Error: {message}");
        return;
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        Debug.Log("UnityAds Started");
        return;
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        throw new System.NotImplementedException();
    }
}
