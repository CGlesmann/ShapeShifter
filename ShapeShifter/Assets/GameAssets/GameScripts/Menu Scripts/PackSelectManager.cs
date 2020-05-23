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

    private void Start() { StartCoroutine(SetPackButtonStates()); }
    private IEnumerator SetPackButtonStates()
    {
        PackButton packButton = null;
        bool overrideLocationSet = false;

        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();

        for (int i = 0; i < packButtonParent.childCount; i++)
        {
            packButton = packButtonParent.GetChild(i).GetComponent<PackButton>();
            if (packButton.CheckForUnlock())
            {
                overrideLocationSet = true;

                menuSwipeController.TransitionToPanel(i);
                yield return new WaitForSeconds(menuSwipeController.GetRemainingTransitionTime() + 0.25f);
                packButton.TriggerUnlock();
                saveDataAccessor.SetData(SaveKeys.HIGHEST_DISPLAYED_PACK_UNLOCK, i);
                yield return new WaitForSeconds(1f);
            }
        }

        if (!overrideLocationSet)    
            menuSwipeController.SetCurrentPanel(lastSelectPackIndex);
        DataTracker.dataTracker.SaveData();
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
