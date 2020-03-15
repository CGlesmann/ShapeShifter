using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Manager Variables")]
    [SerializeField] protected string mainMenuScene = "";
    [SerializeField] protected string instructionsScene = "";
    [SerializeField] protected string optionsScene = "";
    public int levelPackIndex = 0;

    [Header("Object References")]
    [SerializeField] private MenuSwipeController levelSelectPanelController = null;
    [SerializeField] private ChallengePreview challengePreview = null;

    [Header("Level GUI References")]
    [SerializeField] private GameObject levelPreviewPanel = null;
    [SerializeField] private Transform[] levelSelectParentList = null;
    [SerializeField] private TextMeshProUGUI levelNameText = null;
    private string currentLevel = "";

    public void Awake() { StartCoroutine(SetLevelButtonStates()); }

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
                        if (totalCounter <= DataTracker.gameData.highestLevelUnlocked)
                            currentButton.SetUnlockDisplay();
                        else
                        {
                            if (packCounter > levelSelectPanelController.currentPanelIndex)
                            {
                                yield return new WaitForSeconds(1f);
                                levelSelectPanelController.BeginRightTransition();
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

            if (unlockedCounter > DataTracker.gameData.highestLevelUnlocked)
            {
                DataTracker.gameData.highestLevelUnlocked = unlockedCounter;
                DataTracker.dataTracker.SaveData();
            }
        }
    }

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

    public void NavigateToLevel() { SceneManager.LoadScene(currentLevel); }
    public void NavigateToInstructions() { SceneManager.LoadScene(instructionsScene); }
    public void NavigateToOptions()
    {
        OptionManager.SetPreviousMenu(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(optionsScene);
    }
}
