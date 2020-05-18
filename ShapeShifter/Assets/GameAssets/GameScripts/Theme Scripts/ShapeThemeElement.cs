using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeThemeElement : MonoBehaviour
{
    private GameShape gameShape = null;
    private Image elementImage = null;

    private void OnDisable() { ThemeManager.onThemeSettingsUpdate -= LoadElement; }
    private void OnEnable()
    {
        gameShape = GetComponent<GameShape>();
        elementImage = GetComponent<Image>();

        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
        ThemeManager.onThemeSettingsUpdate += LoadElement;
    }

    public void LoadElement(Theme theme, Theme.ColorMode colorMode)
    {
        if (gameShape != null)
        {
            if (theme != null && elementImage != null)
            {
                Sprite shapeSprite = theme.gameShapeThemeDictionary.GetElementData(gameShape.GetShapeType());
                Color shapeColor = theme.gameShapeThemeDictionary.GetElementData(gameShape.GetShapeColor()).GetValue(colorMode);

                elementImage.sprite = shapeSprite;
                elementImage.color = shapeColor;
            }
        }
    }
}
