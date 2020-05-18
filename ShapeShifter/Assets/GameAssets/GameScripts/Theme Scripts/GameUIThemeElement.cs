using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIThemeElement : MonoBehaviour
{
    [Header("Element Key")]
    [SerializeField] private Theme.GameUIThemeKey elementKey = Theme.GameUIThemeKey.GameboardSlot;

    private Image elementImage = null;

    private void OnDisable() { ThemeManager.onThemeSettingsUpdate -= LoadElement; }
    private void OnEnable()
    {
        elementImage = GetComponent<Image>();

        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
        ThemeManager.onThemeSettingsUpdate += LoadElement;
    }

    public void LoadElement(Theme theme, Theme.ColorMode colorMode)
    {
        if (theme != null && elementImage != null)
        {
            ThemeElementData data = theme.gameUIThemeDictionary.GetElementData(elementKey);
            if (data != null)
            {
                elementImage.sprite = data.GetElementSprite();
                elementImage.type = Image.Type.Sliced;
                elementImage.color = data.GetColorByMode(colorMode);
            }
        }
    }
}
