using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectManager : HorizontalPanelController
{
    [Header("Manager Variables")]
    [SerializeField] protected string mainMenuScene = "";
    [SerializeField] protected string instructionsScene = "";
    [SerializeField] protected string optionsScene = "";
    public int levelPackIndex = 0;

    [Header("Object References")]
    [SerializeField] private ChallengePreview challengePreview = null;

    [Header("Level GUI References")]
    [SerializeField] private GameObject levelPreviewPanel = null;
    [SerializeField] private Transform[] levelSelectParentList = null;
    [SerializeField] private TextMeshProUGUI levelNameText = null;
    private string currentLevel = "";

    public override void Awake()
    {
        base.Awake();
        StartCoroutine(SetLevelButtonStates());
    }

    private IEnumerator SetLevelButtonStates()
    {
        if (levelSelectParentList != null && levelSelectParentList.Length > 0)
        {
            LevelButton currentButton = null;
            int unlockedCounter = 0, totalCounter = 0, packCounter = 0;

            foreach (Transform levelGroupParent in levelSelectParentList)
            {
                for (int i = 0; i < levelGroupParent.childCount; i++)
                {
                    currentButton = levelGroupParent.GetChild(i).GetComponent<LevelButton>();
                    totalCounter++;

                    if (totalCounter <= DataTracker.gameData.highestCompletedLevel + 1)
                    {
                        unlockedCounter++;
                        if (totalCounter <= DataTracker.gameData.highestDisplayedUnlock)
                            currentButton.SetUnlockDisplay();
                        else
                        {
                            if (packCounter > currentPanelIndex)
                            {
                                yield return new WaitForSeconds(1f);
                                BeginRightTransition();
                                yield return new WaitForSeconds(0.5f);
                            }

                            currentButton.DisplayUnlockAnimation();
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                    else
                        currentButton.SetLockDisplay();
                }

                packCounter++;
            }

            if (unlockedCounter > DataTracker.gameData.highestDisplayedUnlock)
            {
                DataTracker.gameData.highestDisplayedUnlock = unlockedCounter;
                DataTracker.dataTracker.SaveData();
            }
        }
    }

    #region Navigation Methods
    /// <summary>
    /// Navigates to Main Menu
    /// Used by the back button
    /// </summary>
    public void NavigateToMainMenu() { SceneManager.LoadScene(mainMenuScene); }

    public void DisplayLevelPreview(string levelName, int levelIndex)
    {
        // Storing the input
        currentLevel = levelName;

        // Displaying The Level Investigation Screen
        levelPreviewPanel.SetActive(true);
        levelNameText.text = levelName.Replace('_', ' ');

        challengePreview.SetLevelChallengePreview(levelName, levelIndex);
    }

    public void HideLevelPreview()
    {
        currentLevel = "";
        levelPreviewPanel.SetActive(false);
    }

    /// <summary>
    /// Navigate to the selected level
    /// </summary>
    /// <param name="levelName"></param>
    public void NavigateToLevel() { SceneManager.LoadScene(currentLevel); }

    /// <summary>
    /// Navigates to the instructions screen
    /// </summary>
    public void NavigateToInstructions() { SceneManager.LoadScene(instructionsScene); }
    
    /// <summary>
    /// Navigates to the options menu
    /// </summary>
    public void NavigateToOptions() { SceneManager.LoadScene(optionsScene); }
    #endregion
}
