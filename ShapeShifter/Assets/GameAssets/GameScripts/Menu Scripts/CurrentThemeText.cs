using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrentThemeText : MonoBehaviour
{
    private TextMeshProUGUI themeTitleText = null;

    private void Awake()
    {
        themeTitleText = GetComponent<TextMeshProUGUI>();
        UpdateTitleText(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());

        ThemeManager.onThemeSettingsUpdate += UpdateTitleText;
    }

    public void UpdateTitleText(Theme theme, Theme.ColorMode colorMode) 
    {
        Color titleTheme = theme.textElementDictionary.GetElementData(Theme.TextUIThemeKey.StaticPanelText).GetValue(colorMode);
        string titleHighlightString = ColorUtility.ToHtmlStringRGB(titleTheme);
        themeTitleText.text = $"Current Theme: <color=#{titleHighlightString}>{theme.name}</color>"; 
    }
}
