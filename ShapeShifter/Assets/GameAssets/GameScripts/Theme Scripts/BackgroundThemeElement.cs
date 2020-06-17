using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundThemeElement : ThemeElementLoader
{
    private Camera mainCamera = null;
    private Image backgroundImage = null;

    private void OnDisable() { if (updateDynamically) ThemeManager.onThemeSettingsUpdate -= LoadElement; }
    private void OnEnable()
    {
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
            backgroundImage = GetComponent<Image>();

        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
        if (updateDynamically)
            ThemeManager.onThemeSettingsUpdate += LoadElement;
    }

    public override void LoadElement(Theme theme, Theme.ColorMode colorMode)
    {
        if (theme != null)
        {
            if (mainCamera != null)
                mainCamera.backgroundColor = theme.background.GetValue(colorMode);
            else if (backgroundImage != null)
                backgroundImage.color = theme.background.GetValue(colorMode);
        }
    }
}
