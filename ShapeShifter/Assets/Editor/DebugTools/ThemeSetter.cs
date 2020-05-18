using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ThemeSetter : EditorWindow
{
    private Theme selectedTheme = null;
    private Theme.ColorMode colorMode = Theme.ColorMode.Default;

    public static void ShowDebugWindow()
    {
        ThemeSetter themeSetter = GetWindow<ThemeSetter>();
        themeSetter.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Theme Setter", EditorStyles.boldLabel);
        EditorGUILayout.Separator();

        selectedTheme = (Theme)EditorGUILayout.ObjectField("Target Theme", selectedTheme, typeof(Theme), false);
        colorMode = (Theme.ColorMode)EditorGUILayout.EnumPopup("Target Color Mode", colorMode);
        if (selectedTheme != null)
        {
            if (GUILayout.Button("Set Theme"))
            {
                ThemeManager.SetTheme(selectedTheme.name);
                ThemeManager.SetColorMode(colorMode);
                //SetTheme();
            }
        }
    }

    /*
    private void SetTheme()
    {
        BackgroundThemeElement background = GameObject.FindObjectOfType<BackgroundThemeElement>();
        background.LoadBackgroundElement();

        GeneralUIThemeElement[] generalUIThemeElements = GameObject.FindObjectsOfType<GeneralUIThemeElement>();
        foreach (GeneralUIThemeElement generalElement in generalUIThemeElements)
            generalElement.LoadElement();

        ShapeThemeElement[] shapeThemeElements = GameObject.FindObjectsOfType<ShapeThemeElement>();
        foreach (ShapeThemeElement shapeElement in shapeThemeElements)
            shapeElement.LoadElement();

        TextThemeElement[] textThemeElements = GameObject.FindObjectsOfType<TextThemeElement>();
        foreach (TextThemeElement textElement in textThemeElements)
            textElement.LoadElement();
    }
    */
}
