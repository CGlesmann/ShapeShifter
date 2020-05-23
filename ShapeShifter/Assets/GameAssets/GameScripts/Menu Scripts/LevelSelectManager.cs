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
    private string targetLevel = "";

    [Header("Object References")]
    [SerializeField] private MenuSwipeController levelSelectPanelController = null;
    [SerializeField] private ChallengePreview challengePreview = null;
    [SerializeField] private Animator levelSelectAnimator = null;

    [Header("Level GUI References")]
    [SerializeField] private GameObject levelPreviewPanel = null;
    [SerializeField] private Transform[] levelSelectParentList = null;
    [SerializeField] private TextMeshProUGUI levelNameText = null;
    private string currentLevel = "";

    public void Awake()  { StartCoroutine(SetLevelButtonStates()); }
    private IEnumerator SetLevelButtonStates()
    {
        if (levelSelectParentList != null && levelSelectParentList.Length > 0)
        {
            SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
            Dictionary<int, int> completedLevels = saveDataAccessor.GetDataValue<Dictionary<int, int>>(SaveKeys.COMPLETED_LEVELS_SAVE_KEY);
            int highestLevelUnlocked = saveDataAccessor.GetDataValue<int>(SaveKeys.HIGHEST_DISPLAYED_LEVEL_UNLOCK);

            LevelButton currentButton = null;
            int unlockedCounter = 0, totalCounter = 0, packCounter = 0;

            foreach (Transform levelGroupParent in levelSelectParentList)
            {
                for (int i = 0; i < levelGroupParent.childCount; i++)
                {
                    currentButton = levelGroupParent.GetChild(i).GetComponent<LevelButton>();
                    totalCounter++;

                    if (completedLevels != null && completedLevels.TryGetValue(levelPackIndex + 1, out int highestCompletedLevel))
                    {
                        highestCompletedLevel++;
                        if (totalCounter <= highestCompletedLevel)
                        {
                            unlockedCounter++;
                            if (totalCounter <= highestLevelUnlocked)
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
                    else if (currentButton.requireLevelUnlock)
                        currentButton.SetLockDisplay();
                    else
                        currentButton.SetUnlockDisplay();
                }

                packCounter++;
            }

            if (unlockedCounter > highestLevelUnlocked)
            {
                saveDataAccessor.SetData(SaveKeys.HIGHEST_DISPLAYED_LEVEL_UNLOCK, unlockedCounter);
                DataTracker.dataTracker.SaveData();
            }
        }
    }

    public void NavigateToMainMenu() { targetLevel = mainMenuScene; levelSelectAnimator.SetTrigger("Exit"); }
    public void DisplayLevelPreview(string levelName, int levelIndex)
    {
        // Storing the input
        currentLevel = $"Level_{levelPackIndex + 1}-{levelIndex + 1}";

        // Displaying The Level Investigation Screen
        levelPreviewPanel.SetActive(true);
        levelNameText.text = $"Level {levelPackIndex + 1}-{levelIndex + 1}";

        challengePreview.SetLevelChallengePreview(ChallengeManager.GetCurrentChallengeLog(levelPackIndex + 1, levelIndex + 1), $"Level_{levelPackIndex + 1}-{levelIndex + 1}");
    }

    public void NavigateToLevel() { targetLevel = currentLevel; }
    public void NavigateToInstructions() { targetLevel = instructionsScene; levelSelectAnimator.SetTrigger("Exit"); }
    public void NavigateToOptions()
    {
        OptionManager.SetPreviousMenu(SceneManager.GetActiveScene().name);
        targetLevel = optionsScene;

        levelSelectAnimator.SetTrigger("Exit");
    }

    public void BeginSceneExitTransition() { levelSelectAnimator.SetTrigger("Exit"); }
    public void ExecuteExitTransitionFinish() { SceneManager.LoadScene(targetLevel); }
}
