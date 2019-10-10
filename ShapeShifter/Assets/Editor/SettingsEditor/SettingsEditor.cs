using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SettingsEditor : EditorWindow
{
    [MenuItem("Shape Settings", menuItem = "Settings/Shape Settings")]
    public static void DisplayWindow()
    {
        SettingsEditor window = (SettingsEditor)CreateInstance(typeof(SettingsEditor));
        window.Show();
    }

    private void OnGUI()
    {
        // Drawing all the color fields
        Rect fieldPos = new Rect(16f, 16f, 200, 54f);
        DrawColorField(fieldPos, "Red Color", ref ShapeSettings.RED_COLOR);
        fieldPos.y += 64f;

        DrawColorField(fieldPos, "Blue Color", ref ShapeSettings.BLUE_COLOR);
        fieldPos.y += 64f;

        DrawColorField(fieldPos, "Green Color", ref ShapeSettings.GREEN_COLOR);
        fieldPos.y += 64f;

        DrawColorField(fieldPos, "Yellow Color", ref ShapeSettings.YELLOW_COLOR);
        fieldPos.y += 64f;

        // Offsetting the Draw Position
        fieldPos.x += 232f;
        fieldPos.y -= 256f;
        fieldPos.width = fieldPos.height = 128f;

        // Drawing all the shape sprite fields
        DrawSpriteField(fieldPos, "Square Sprite", ref ShapeSettings.SQUARE_SPRITE);
        fieldPos.y += 138f;

        DrawSpriteField(fieldPos, "Circle Sprite", ref ShapeSettings.CIRCLE_SPRITE);
        fieldPos.y += 138f;

        fieldPos.x += 160f;
        fieldPos.y -= 276f;
        DrawSpriteField(fieldPos, "Triangle Sprite", ref ShapeSettings.TRIANGLE_SPRITE);
        fieldPos.y += 138f;

        DrawSpriteField(fieldPos, "Diamond Sprite", ref ShapeSettings.DIAMOND_SPRITE);
        fieldPos.y += 138f;
    }

    private void DrawColorField(Rect pos, string fieldName, ref Color colorID)
    {
        // Drawing the background box
        GUI.Box(pos, fieldName);

        // Drawing the color field on top of the background
        Rect colorField = new Rect(pos.x + 16f, pos.y + 24, pos.width - 32f, 24f);
        colorID = EditorGUI.ColorField(colorField, colorID);
    }

    private void DrawSpriteField(Rect pos, string fieldName, ref Sprite spriteID)
    {
        // Drawing the background box
        GUI.Box(pos, fieldName);

        // Drawing the color field on top of the background
        Rect redField = new Rect(pos.x + 16f, pos.y + 24, pos.width - 32f, 100f);
        spriteID = (Sprite)EditorGUI.ObjectField(redField, spriteID, typeof(Sprite), false);
    }
}
