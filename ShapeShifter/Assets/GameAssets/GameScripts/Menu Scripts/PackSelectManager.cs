using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PackSelectManager : MonoBehaviour
{
    [Header("Scene Name References")]
    [SerializeField] private string mainMenuScene = "";
    [SerializeField] private string optionScene = "";

    [Header("Object References")]
    [SerializeField] private Transform packButtonParent = null;
    [SerializeField] private MenuSwipeController menuSwipeController = null;

    private void Start() { StartCoroutine(SetPackButtonStates()); }
    private IEnumerator SetPackButtonStates()
    {
        PackButton packButton = null;
        for (int i = 0; i < packButtonParent.childCount; i++)
        {
            packButton = packButtonParent.GetChild(i).GetComponent<PackButton>();
            if (packButton.CheckForUnlock())
            {
                menuSwipeController.TransitionToPanel(i);
                yield return new WaitForSeconds(menuSwipeController.GetRemainingTransitionTime() + 0.25f);
                packButton.TriggerUnlock();
                DataTracker.gameData.highestPackUnlocked = i;
                yield return new WaitForSeconds(1f);
            }
        }

        DataTracker.dataTracker.SaveData();
    }

    public void NavigateToLevelSelect(string packScreen) { SceneManager.LoadScene(packScreen); }
    public void NavigateToMainMenu() { SceneManager.LoadScene(mainMenuScene); }
    public void NavigateToOptions()
    {
        OptionManager.SetPreviousMenu(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(optionScene);
    }
}
