using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralUIThemeElement : MonoBehaviour
{
    [Header("Element Key")]
    [SerializeField] private Theme.GeneralUIThemeKey elementKey = Theme.GeneralUIThemeKey.Button;

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
            ThemeElementData data = theme.generalUIThemeDictionary.GetElementData(elementKey);
            if (data != null)
            {
                elementImage.sprite = data.GetElementSprite();
                elementImage.type = Image.Type.Sliced;
                elementImage.color = data.GetColorByMode(colorMode);
            }
        }
    }
}
