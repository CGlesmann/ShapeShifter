using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeThemeElement : ThemeElementLoader
{
    private GameShape gameShape = null;
    private Image elementImage = null;

    private void OnDisable() { if (updateDynamically) ThemeManager.onThemeSettingsUpdate -= LoadElement; }
    private void OnEnable()
    {
        gameShape = GetComponent<GameShape>();
        elementImage = GetComponent<Image>();

        LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
        if (updateDynamically)
            ThemeManager.onThemeSettingsUpdate += LoadElement;
    }

    public override void LoadElement(Theme theme, Theme.ColorMode colorMode)
    {
        if (gameShape == null)
            gameShape = GetComponent<GameShape>();

        if (elementImage == null)
            elementImage = GetComponent<Image>();

        if (theme != null && elementImage != null)
        {
            Sprite shapeSprite = theme.gameShapeThemeDictionary.GetElementData(gameShape.GetShapeType());
            Color shapeColor = theme.gameShapeThemeDictionary.GetElementData(gameShape.GetShapeColor()).GetValue(colorMode);

            elementImage.sprite = shapeSprite;
            elementImage.color = shapeColor;
        }
    }
}
