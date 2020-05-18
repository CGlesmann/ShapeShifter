using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGameThemeElement : MonoBehaviour
{
    [Header("Element Keys")]
    [SerializeField] private Theme.GameUIThemeKey normalKey = Theme.GameUIThemeKey.GameboardSlot;
    [SerializeField] private Theme.GameUIThemeKey highlightedKey = Theme.GameUIThemeKey.SelectedGameBoardSlot;

    private Image elementImage = null;
    private bool highlighted = false;

    private void OnDisable() { ThemeManager.onThemeSettingsUpdate -= LoadElement; }
    private void Awake()
    {
        elementImage = GetComponent<Image>();

        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
        ThemeManager.onThemeSettingsUpdate += LoadElement;
    }

    public void ToggleActiveKey()
    {
        highlighted = !highlighted;
        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    public void SetElementToNormal()
    {
        highlighted = false;
        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    public void SetElementToHighlighted()
    {
        highlighted = true;
        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    private void LoadElement(Theme theme, Theme.ColorMode colorMode)
    {
        ThemeElementData data = theme.gameUIThemeDictionary.GetElementData(highlighted ? highlightedKey : normalKey);
        if (data != null)
        {
            elementImage.sprite = data.GetElementSprite();
            elementImage.type = Image.Type.Sliced;
            elementImage.color = data.GetColorByMode(colorMode);
        }
    }
}
