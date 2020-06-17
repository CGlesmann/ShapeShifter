using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PackSelectManager : MonoBehaviour
{
    private static int lastSelectPackIndex = 0;

    [Header("Scene Name References")]
    [SerializeField] private string mainMenuScene = "";
    [SerializeField] private string optionScene = "";

    [Header("Object References")]
    [SerializeField] private Transform packButtonParent = null;
    [SerializeField] private MenuSwipeController menuSwipeController = null;
    [SerializeField] private Animator menuAnimator = null;

    private string targetLevel = "";

    private void Awake()
    {
        menuSwipeController.SetCurrentPanel(lastSelectPackIndex);

        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        Dictionary<int, int> completedLevels = saveDataAccessor.GetDataValue<Dictionary<int, int>>(SaveKeys.COMPLETED_LEVELS_SAVE_KEY);

        int highestDisplayedPackUnlock = saveDataAccessor.GetDataValue<int>(SaveKeys.HIGHEST_DISPLAYED_PACK_UNLOCK);
        int highestCompletedPackLevel;
        int lastUnlock = 0;

        PackButton currentPackButton;
        Queue<Action> buttonActions = new Queue<Action>();

        for(int i = 0; i < packButtonParent.childCount; i++)
        {
            if (completedLevels == null)
                highestCompletedPackLevel = 0;
            else if (completedLevels.TryGetValue(i + 0, out int highestLeve))
                highestCompletedPackLevel = highestLeve;
            else
                highestCompletedPackLevel = 0;

            currentPackButton = packButtonParent.GetChild(i).GetComponent<PackButton>();

            Action buttonAction = currentPackButton.CheckForUnlock(highestCompletedPackLevel, highestDisplayedPackUnlock, out bool displayUnlock);
            if (buttonAction != null && displayUnlock)
            {
                buttonActions.Enqueue(menuSwipeController.BeginRightTransition);
                buttonActions.Enqueue(buttonAction);

                lastUnlock = i;
            }
        }

        if (lastUnlock > highestDisplayedPackUnlock)
        {
            saveDataAccessor.SetData(SaveKeys.HIGHEST_DISPLAYED_PACK_UNLOCK, lastUnlock);
            DataTracker.dataTracker.SaveData();
        }

        StartCoroutine(SetButtonStates(buttonActions));
    }

    private IEnumerator SetButtonStates(Queue<Action> buttonActions)
    {
        int actionCount = buttonActions?.Count ?? 0;
        if (actionCount == 0)
            yield break;

        yield return new WaitForSeconds(0.75f);
        for(int i = 0; i < actionCount; i++)
        {
            Action currentAction = buttonActions.Dequeue();
            currentAction.Invoke();

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void NavigateToLevelSelect(string packScreen)
    {
        lastSelectPackIndex = menuSwipeController.currentPanelIndex;
        targetLevel = packScreen;

        menuAnimator.SetTrigger("Exit");
    }

    public void NavigateToMainMenu()
    {
        lastSelectPackIndex = menuSwipeController.currentPanelIndex;
        targetLevel = mainMenuScene;

        menuAnimator.SetTrigger("Exit");
    }

    public void NavigateToOptions()
    {
        lastSelectPackIndex = menuSwipeController.currentPanelIndex;
        OptionManager.SetPreviousMenu(SceneManager.GetActiveScene().name);
        targetLevel = optionScene;

        menuAnimator.SetTrigger("Exit");
    }

    public void ExecuteExitLevel()
    {
        SceneManager.LoadScene(targetLevel);
    }
}
