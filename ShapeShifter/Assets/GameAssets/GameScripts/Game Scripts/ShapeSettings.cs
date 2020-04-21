using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShapeSettings
{
    private static ShapeSettingsConfig shapeSettingsConfig = null;

    private static void LoadConfigFile() { shapeSettingsConfig = Resources.Load<ShapeSettingsConfig>("Settings/ShapeSettingsConfig"); }
    public static Sprite GetShapeSprite(GameShape.ShapeType key)
    {
        if (shapeSettingsConfig == null)
            LoadConfigFile();

        return shapeSettingsConfig.shapeSprites.GetValue(key);
    }

    public static Color GetShapeColor(GameShape.ColorType key)
    {
        if (shapeSettingsConfig == null)
            LoadConfigFile();

        return shapeSettingsConfig.shapeColors.GetValue(key);
    }
}
