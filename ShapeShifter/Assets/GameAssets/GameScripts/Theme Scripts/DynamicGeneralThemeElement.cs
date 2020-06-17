using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGeneralThemeElement : ThemeElementLoader
{
    [Header("Element Keys")]
    [SerializeField] private Theme.GeneralUIThemeKey primaryKey = Theme.GeneralUIThemeKey.Button;
    [SerializeField] private Theme.GeneralUIThemeKey secondaryKey = Theme.GeneralUIThemeKey.HighlightButton;

    private Theme currentTheme;
    private Theme.ColorMode currentColorMode;

    private ThemeElementData primaryKeyData;
    private ThemeElementData secondaryKeyData;

    private Image elementImage = null;
    private bool toggled = false;

    private void OnDisable() { if (updateDynamically) ThemeManager.onThemeSettingsUpdate -= LoadElement; }
    private void Awake()
    {
        elementImage = GetComponent<Image>();
        if (elementImage == null)
            Debug.LogError($"Couldn't find Image for object {gameObject.name}");

        currentTheme = ThemeManager.GetCurrentTheme();
        currentColorMode = ThemeManager.GetCurrentColorMode();

        StoreThemeElements(currentTheme, currentColorMode);
        LoadElement(primaryKeyData, currentColorMode);

        if (updateDynamically)
            ThemeManager.onThemeSettingsUpdate += LoadElement;
    }

    private void StoreThemeElements(Theme theme, Theme.ColorMode colorMode)
    {
        primaryKeyData = theme.generalUIThemeDictionary.GetElementData(primaryKey);
        secondaryKeyData = theme.generalUIThemeDictionary.GetElementData(secondaryKey);
    }

    public void ToggleActiveKey()
    {
        toggled = !toggled;
        LoadElement(toggled ? secondaryKeyData : primaryKeyData, currentColorMode);
    }

    public void SetElementToNormal()
    {
        toggled = false;
        LoadElement(primaryKeyData, currentColorMode);
    }

    public void SetElementToHighlighted()
    {
        toggled = true;
        LoadElement(secondaryKeyData, currentColorMode);
    }

    private void LoadElement(ThemeElementData data, Theme.ColorMode colorMode)
    {
        if (data != null)
        {
            elementImage.sprite = data.GetElementSprite();
            elementImage.type = data.GetSpriteType();
            elementImage.color = data.GetColorByMode(colorMode);
        }
    }

    public override void LoadElement(Theme theme, Theme.ColorMode colorMode)
    {
        ThemeElementData data = theme.generalUIThemeDictionary.GetElementData(toggled ? secondaryKey : primaryKey);
        if (data != null)
        {
            elementImage.sprite = data.GetElementSprite();
            elementImage.type = data.GetSpriteType();
            elementImage.color = data.GetColorByMode(colorMode);
        }
    }
}
