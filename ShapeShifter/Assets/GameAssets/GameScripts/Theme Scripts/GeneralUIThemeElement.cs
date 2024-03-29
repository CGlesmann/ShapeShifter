﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralUIThemeElement : ThemeElementLoader
{
    [Header("Element Key")]
    [SerializeField] private Theme.GeneralUIThemeKey elementKey = Theme.GeneralUIThemeKey.Button;

    private Image elementImage = null;

    private void OnDisable() { if (updateDynamically) ThemeManager.onThemeSettingsUpdate -= LoadElement; }
    private void OnEnable()
    {
        elementImage = GetComponent<Image>();

        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
        if (updateDynamically)
            ThemeManager.onThemeSettingsUpdate += LoadElement;
    }

    public override void LoadElement(Theme theme, Theme.ColorMode colorMode)
    {
        if (theme != null && elementImage != null)
        {
            ThemeElementData data = theme.generalUIThemeDictionary.GetElementData(elementKey);
            if (data != null)
            {
                elementImage.sprite = data.GetElementSprite();
                elementImage.type = data.GetSpriteType();
                elementImage.color = data.GetColorByMode(colorMode);
            }
        }
    }
}
