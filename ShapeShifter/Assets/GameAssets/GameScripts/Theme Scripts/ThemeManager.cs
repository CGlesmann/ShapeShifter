using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ThemeManager
{
    private const string defaultThemePath = "/Themes/Default";

    public delegate void OnThemeSettingsUpdate(Theme theme, Theme.ColorMode colorMode);
    public static event OnThemeSettingsUpdate onThemeSettingsUpdate;

    public static Theme GetCurrentTheme()
    {
        if (!Application.isPlaying)
            return Resources.Load<Theme>(defaultThemePath);

        return DataTracker.gameData.GetTheme();
    }
    public static Theme.ColorMode GetCurrentColorMode() { return DataTracker.gameData.GetSavedColorMode(); }

    public static void SetTheme(string newTheme) { DataTracker.gameData.SetThemeKey(newTheme); }
    public static void SetColorMode(Theme.ColorMode colorMode) { DataTracker.gameData.SetColorMode(colorMode); }

    public static void InvokeUpdateMethod() { onThemeSettingsUpdate?.Invoke(GetCurrentTheme(), GetCurrentColorMode()); }

    public static Theme LoadTheme(string themeName)
    {
        string path = $"Themes/{themeName}";
        Theme theme = Resources.Load<Theme>(path);

        if (theme == null)
        {
            Debug.Log($"Didn't find a theme file at {path}");
            theme = Resources.Load<Theme>(defaultThemePath);
        }

        return theme;
    }
}
