using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThemeSelector : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private TextMeshProUGUI selectedThemeTitleText = null;

    private Theme[] availableThemes = null;
    private Theme currentlySelectedTheme = null;
    private int currentlySelectedThemeIndex;

    private void OnEnable()
    {
        availableThemes = Resources.LoadAll<Theme>("Themes");

        int counter = 0;
        foreach (Theme theme in availableThemes)
        {
            if (theme.name == DataTracker.gameData.GetTheme().name)
            {
                currentlySelectedTheme = theme;
                currentlySelectedThemeIndex = counter;
                selectedThemeTitleText.text = theme.name;
            }
            else
                counter++;
        }
    }

    public void SelectNextTheme()
    {
        currentlySelectedThemeIndex++;
        if (currentlySelectedThemeIndex > availableThemes.Length - 1)
            currentlySelectedThemeIndex = 0;

        currentlySelectedTheme = availableThemes[currentlySelectedThemeIndex];
        selectedThemeTitleText.text = currentlySelectedTheme.name;
        DataTracker.gameData.SetThemeKey(currentlySelectedTheme.name);
        DataTracker.dataTracker.SaveData();
    }

    public void SelectPreviousTheme()
    {
        currentlySelectedThemeIndex--;
        if (currentlySelectedThemeIndex < 0)
            currentlySelectedThemeIndex = availableThemes.Length - 1;

        currentlySelectedTheme = availableThemes[currentlySelectedThemeIndex];
        selectedThemeTitleText.text = currentlySelectedTheme.name;
        DataTracker.gameData.SetThemeKey(currentlySelectedTheme.name);
        DataTracker.dataTracker.SaveData();
    }
}
