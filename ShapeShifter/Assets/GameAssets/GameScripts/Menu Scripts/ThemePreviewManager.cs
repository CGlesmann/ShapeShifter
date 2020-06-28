using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemePreviewManager : MonoBehaviour
{
    private static int currentlySelectedThemeIndex = -1;

    [Header("Object Reference")]
    [SerializeField] private ThemeHeader themeHeader = null;
    [SerializeField] private MenuSwipeController themeCardParent = null;

    private void OnEnable()
    {
        if (currentlySelectedThemeIndex == -1)
        {
            Theme currentTheme = ThemeManager.GetCurrentTheme();
            Theme[] allThemes = Resources.LoadAll<Theme>("Themes/");

            for (int i = 0; i < allThemes.Length; i++)
            {
                if (allThemes[i] == currentTheme)
                {
                    currentlySelectedThemeIndex = i;
                    break;
                }
            }
        }

        themeCardParent.SetCurrentPanel(currentlySelectedThemeIndex);
        themeHeader.UpdateHeaderText(currentlySelectedThemeIndex);
    }

    public void SetNewThemeIndex(int newIndex) { currentlySelectedThemeIndex = newIndex; }
    public void PlayExitAnimation() { GetComponent<Animator>().SetTrigger("Exit"); }
    public void DisableThemePreview() { gameObject.SetActive(false); }
}
