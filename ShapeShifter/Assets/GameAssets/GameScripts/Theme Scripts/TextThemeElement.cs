using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextThemeElement : ThemeElementLoader
{
    [Header("Element Settings")]
    [SerializeField] private Theme.TextUIThemeKey elementKey = Theme.TextUIThemeKey.ButtonText;

    private TextMeshProUGUI elementText = null;

    private void OnDisable() { if (updateDynamically) ThemeManager.onThemeSettingsUpdate -= LoadElement; }
    private void OnEnable()
    {
        elementText = GetComponent<TextMeshProUGUI>();

        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
        if (updateDynamically)
            ThemeManager.onThemeSettingsUpdate += LoadElement;
    }

    public override void LoadElement(Theme theme, Theme.ColorMode colorMode)
    {
        if (theme != null)
        {
            Color textColor = theme.textElementDictionary.GetElementData(elementKey).GetValue(colorMode);
            elementText.color = textColor;
        }
    }
}
