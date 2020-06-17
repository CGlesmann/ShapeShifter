using System;
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

    private void Awake()
    {
        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        Dictionary<int, int> displayedLevelUnlocks = saveDataAccessor.GetDataValue<Dictionary<int, int>>(SaveKeys.HIGHEST_DISPLAYED_LEVEL_UNLOCK);
        Dictionary<int, int> completedLevels = saveDataAccessor.GetDataValue<Dictionary<int, int>>(SaveKeys.COMPLETED_LEVELS_SAVE_KEY);

        int highestDisplayedUnlock;
        if (displayedLevelUnlocks != null)
            highestDisplayedUnlock = displayedLevelUnlocks.TryGetValue(levelPackIndex + 1, out int hdu) ? hdu : 0;
        else
            highestDisplayedUnlock = 0;

        int highestLevelUnlocked;
        if (completedLevels != null)
            highestLevelUnlocked = completedLevels.TryGetValue(levelPackIndex + 1, out int hlu) ? hlu : 0;
        else
            highestLevelUnlocked = 0;

        Queue<Action> buttonActions = new Queue<Action>();
        LevelButton currentLevelButton;
        int currentPackTracker = 0, lastPanelToUnlockALevel = 0;

        foreach(Transform panelParent in levelSelectParentList)
        {
            for (int i = 0; i < panelParent.childCount; i++)
            {
                currentLevelButton = panelParent.GetChild(i).GetComponent<LevelButton>();

                Action newAction = currentLevelButton.SetLevelButtonState(highestLevelUnlocked, highestDisplayedUnlock, out bool displayUnlock);
                if (newAction != null)
                    buttonActions.Enqueue(newAction);

                if (currentPackTracker > lastPanelToUnlockALevel && displayUnlock)
                {
                    lastPanelToUnlockALevel = currentPackTracker;
                    buttonActions.Enqueue(levelSelectPanelController.BeginRightTransition);
                }
            }

            currentPackTracker++;
        }

        #region Save Displayed Level Unlock Value
        if (displayedLevelUnlocks == null)
        {
            displayedLevelUnlocks = new Dictionary<int, int>();
            displayedLevelUnlocks.Add(levelPackIndex + 1, highestLevelUnlocked);

            saveDataAccessor.SetData(SaveKeys.HIGHEST_DISPLAYED_LEVEL_UNLOCK, displayedLevelUnlocks);
            DataTracker.dataTracker.SaveData();
        } else if (!displayedLevelUnlocks.ContainsKey(levelPackIndex + 1)) {
            displayedLevelUnlocks.Add(levelPackIndex + 1, highestLevelUnlocked);

            saveDataAccessor.SetData(SaveKeys.HIGHEST_DISPLAYED_LEVEL_UNLOCK, displayedLevelUnlocks);
            DataTracker.dataTracker.SaveData();
        } else if (highestLevelUnlocked > highestDisplayedUnlock) {
            displayedLevelUnlocks[levelPackIndex + 1] = highestLevelUnlocked;

            saveDataAccessor.SetData(SaveKeys.HIGHEST_DISPLAYED_LEVEL_UNLOCK, displayedLevelUnlocks);
            DataTracker.dataTracker.SaveData();
        }
        #endregion

        StartCoroutine(SetButtonStates(buttonActions));
    }

    private IEnumerator SetButtonStates(Queue<Action> buttonActions)
    {
        yield return new WaitForSeconds(0.75f);

        int actionCount = buttonActions.Count;
        for (int i = 0; i < actionCount; i++)
        {
            Action currentAction = buttonActions.Dequeue();
            currentAction.Invoke();

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void NavigateToMainMenu() { targetLevel = mainMenuScene; levelSelectAnimator.SetTrigger("Exit"); }
    public void DisplayLevelPreview(string levelName, int levelIndex)
    {
        // Storing the input
        currentLevel = $"Level_{levelPackIndex + 1}-{levelIndex + 1}";

        // Displaying The Level Investigation Screen
        levelPreviewPanel.SetActive(true);
        levelPreviewPanel.GetComponent<LevelPreview>().SetLevelPreview(levelPackIndex, levelIndex);
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
