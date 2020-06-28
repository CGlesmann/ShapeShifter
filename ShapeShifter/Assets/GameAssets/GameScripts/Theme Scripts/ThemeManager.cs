using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ThemeManager
{
    private const string defaultThemePath = "Themes/Default";

    private static SaveDataAccessor saveDataAccessor = new SaveDataAccessor();

    public delegate void OnThemeSettingsUpdate(Theme theme, Theme.ColorMode colorMode);
    public static event OnThemeSettingsUpdate onThemeSettingsUpdate;

    public static Theme GetCurrentTheme()
    {
        if (!Application.isPlaying)
            return Resources.Load<Theme>(defaultThemePath);

        string currentThemeKey = saveDataAccessor.GetDataValue<string>(SaveKeys.SELECTED_THEME_KEY);
        if (currentThemeKey == null || currentThemeKey == "")
            return LoadDefaultTheme();

        Theme theme = Resources.Load<Theme>($"Themes/{currentThemeKey}");
        return (theme != null ? theme : LoadDefaultTheme());
    }
    public static Theme.ColorMode GetCurrentColorMode()
    {
        if (!Application.isPlaying)
            return Theme.ColorMode.Default;

        return saveDataAccessor.GetDataValue<Theme.ColorMode>(SaveKeys.SELECTED_COLOR_MODE);
    }

    public static void SetTheme(string newTheme) { saveDataAccessor.SetData(SaveKeys.SELECTED_THEME_KEY, newTheme); }
    public static void SetTheme(Theme newTheme) { saveDataAccessor.SetData(SaveKeys.SELECTED_THEME_KEY, newTheme.name); }
    public static void SetColorMode(Theme.ColorMode colorMode) { saveDataAccessor.SetData(SaveKeys.SELECTED_COLOR_MODE, colorMode); }
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

    public static Theme LoadDefaultTheme()
    {
        Theme defaultTheme = Resources.Load<Theme>(defaultThemePath);
        return defaultTheme;
    }
}
