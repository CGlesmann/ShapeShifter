using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGameThemeElement : ThemeElementLoader
{
    [Header("Element Keys")]
    [SerializeField] private Theme.GameUIThemeKey normalKey = Theme.GameUIThemeKey.GameboardSlot;
    [SerializeField] private Theme.GameUIThemeKey highlightedKey = Theme.GameUIThemeKey.SelectedGameBoardSlot;
    [SerializeField] private Theme.GameUIThemeKey tertiaryKey = Theme.GameUIThemeKey.SolutionboardSlot;

    private Theme.GameUIThemeKey currentKey;
    private Image elementImage = null;
    private bool highlighted = false;

    private void OnDisable() { if (updateDynamically) ThemeManager.onThemeSettingsUpdate -= LoadElement; }
    private void Awake()
    {
        elementImage = GetComponent<Image>();

        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
        if (updateDynamically)
            ThemeManager.onThemeSettingsUpdate += LoadElement;
    }

    public void SetElementToNewKey(Theme.GameUIThemeKey key)
    {
        currentKey = key;
        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    public void ToggleActiveKey()
    {
        Debug.Log($"Toggling {gameObject.name}'s element");

        highlighted = !highlighted;
        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    public void SetElementToNormal()
    {
        currentKey = normalKey;

        highlighted = false;
        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    public void SetElementToHighlighted()
    {
        currentKey = highlightedKey;
        highlighted = true;
        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    public void SetElementToTertiary()
    {
        currentKey = tertiaryKey;
        highlighted = true;
        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    public override void LoadElement(Theme theme, Theme.ColorMode colorMode)
    {
        ThemeElementData data = theme.gameUIThemeDictionary.GetElementData(currentKey);
        if (data != null)
        {
            elementImage.sprite = data.GetElementSprite();
            elementImage.type = data.GetSpriteType();
            elementImage.color = data.GetColorByMode(colorMode);
        }
    }
}
