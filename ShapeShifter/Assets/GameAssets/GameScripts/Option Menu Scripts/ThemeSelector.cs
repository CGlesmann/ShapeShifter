using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThemeSelector : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private TextMeshProUGUI selectedThemeTitleText = null;

    private SaveDataAccessor saveDataAccessor;

    private Theme[] availableThemes = null;
    private Theme currentlySelectedTheme = null;
    private int currentlySelectedThemeIndex;

    private void OnEnable()
    {
        saveDataAccessor = new SaveDataAccessor();
        availableThemes = Resources.LoadAll<Theme>("Themes");
        string currentlySelectedThemeName = ThemeManager.GetCurrentTheme().name;

        int counter = 0;
        foreach (Theme theme in availableThemes)
        {
            if (theme.name == currentlySelectedThemeName)
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

        SetThemeKey();
    }

    public void SelectPreviousTheme()
    {
        currentlySelectedThemeIndex--;
        if (currentlySelectedThemeIndex < 0)
            currentlySelectedThemeIndex = availableThemes.Length - 1;

        SetThemeKey();
    }

    private void SetThemeKey()
    {
        currentlySelectedTheme = availableThemes[currentlySelectedThemeIndex];
        selectedThemeTitleText.text = currentlySelectedTheme.name;

        saveDataAccessor.SetData(SaveKeys.SELECTED_THEME_KEY, currentlySelectedTheme.name);
        DataTracker.dataTracker.SaveData();

        ThemeManager.InvokeUpdateMethod();
    }
}
