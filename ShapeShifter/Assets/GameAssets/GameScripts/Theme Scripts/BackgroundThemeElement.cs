using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundThemeElement : MonoBehaviour
{
    private Camera mainCamera = null;

    private void OnDisable() { ThemeManager.onThemeSettingsUpdate -= LoadBackgroundElement; }
    private void OnEnable()
    {
        mainCamera = GetComponent<Camera>();

        LoadBackgroundElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
        ThemeManager.onThemeSettingsUpdate += LoadBackgroundElement;
    }
    

    public void LoadBackgroundElement(Theme theme, Theme.ColorMode colorMode)
    {
        if (theme != null)
            mainCamera.backgroundColor = theme.background.GetValue(colorMode);
    }
}
