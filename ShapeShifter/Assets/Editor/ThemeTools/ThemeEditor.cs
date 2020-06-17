using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ThemeEditor : EditorWindow
{
    private const string themeFolderPath = "Themes/";
    private static Theme currentTheme = null;

    private bool creatingTheme = false;
    private string newThemeName = "";

    private Vector2 scrollPosition = Vector2.zero;
    private bool viewingBackgroundSection = false;
    private bool viewingGeneralUISection = false;
    private bool viewingGameUISection = false;
    private bool viewingGameShapeSection = false;
    private bool viewingGameTextSection = false;

    [MenuItem("Theme Editor", menuItem = "Global Tools/Theme Editor")]
    public static void ShowEditor()
    {
        ThemeEditor editor = GetWindow<ThemeEditor>();
        editor.Show();
    }

    private void OnGUI()
    {
        #region Heading
        GUIStyle editorHeader = new GUIStyle();
        editorHeader.fontSize = 24;
        editorHeader.fontStyle = FontStyle.Bold;
        editorHeader.normal.textColor = Color.black;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Theme Editor", editorHeader, GUILayout.MaxWidth(250f));

        if (!creatingTheme)
        {
            if (GUILayout.Button("Toggle Active Theme", GUILayout.MaxWidth(250f)))
            {
                GenericMenu themeSelector = new GenericMenu();

                Theme[] createdThemes = Resources.LoadAll<Theme>(themeFolderPath);
                if (createdThemes != null && createdThemes.Length > 0)
                {
                    for (int i = 0; i < createdThemes.Length; i++)
                        themeSelector.AddItem(new GUIContent($"{createdThemes[i].name}"), false, ToggleActiveTheme, createdThemes[i]);
                }

                themeSelector.AddSeparator("");
                themeSelector.AddItem(new GUIContent("Create a new Theme"), false, BeginCreatingNewTheme);

                themeSelector.ShowAsContext();
            }
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Theme Creation Panel
        if (creatingTheme)
        {
            DrawThemeCreatePanel();
            return;
        }
        #endregion

        #region Theme Editor
        EditorGUILayout.Separator();
        if (currentTheme == null)
        {
            EditorGUILayout.LabelField("Please Select a theme to begin editing", EditorStyles.centeredGreyMiniLabel);
            return;
        }

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField($"Editing {currentTheme.name} Theme", EditorStyles.boldLabel);
        EditorGUILayout.Separator();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        #region Background Section
        if (viewingBackgroundSection = EditorGUILayout.BeginFoldoutHeaderGroup(viewingBackgroundSection, "Background"))
        {
            DrawColorDictionary(currentTheme.background);
            EditorGUILayout.Separator();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        #region General UI Section
        if (viewingGeneralUISection = EditorGUILayout.BeginFoldoutHeaderGroup(viewingGeneralUISection, "General UI Section"))
        {
            int generalUIKeyCount = Enum.GetNames(typeof(Theme.GeneralUIThemeKey)).Length;
            Theme.GeneralUIThemeKey currentKey;
            ThemeElementData data;
            for (int i = 0; i < generalUIKeyCount; i++)
            {
                currentKey = (Theme.GeneralUIThemeKey)i;
                if (!currentTheme.generalUIThemeDictionary.Contains(currentKey, out int index))
                {
                    data = new ThemeElementData();
                    currentTheme.generalUIThemeDictionary.Add(currentKey, data);
                }
                else
                    data = currentTheme.generalUIThemeDictionary.GetElementData(index);

                DrawThemeData($"{currentKey}", data);
                EditorGUILayout.Separator();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        #region GameShape Section
        if (viewingGameShapeSection = EditorGUILayout.BeginFoldoutHeaderGroup(viewingGameShapeSection, "Game Shape Section"))
        {
            int shapeTypeCount = Enum.GetNames(typeof(GameShape.ShapeType)).Length;
            GameShape.ShapeType shapeType;
            for (int i = 0; i < shapeTypeCount; i++)
            {
                shapeType = (GameShape.ShapeType)i;
                if (!currentTheme.gameShapeThemeDictionary.Contains(shapeType, out int index))
                    currentTheme.gameShapeThemeDictionary.Add(shapeType, null);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"{shapeType} Sprite", GUILayout.MaxWidth(150f));
                currentTheme.gameShapeThemeDictionary.SetValue(i, (Sprite)EditorGUILayout.ObjectField(currentTheme.gameShapeThemeDictionary.GetElementData(shapeType), typeof(Sprite), false, GUILayout.MaxWidth(150f)));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Separator();

            int colorTypeCount = Enum.GetNames(typeof(GameShape.ColorType)).Length;
            GameShape.ColorType colorType;
            for (int i = 0; i < shapeTypeCount; i++)
            {
                colorType = (GameShape.ColorType)i;
                if (!currentTheme.gameShapeThemeDictionary.Contains(colorType, out int index))
                    currentTheme.gameShapeThemeDictionary.Add(colorType, new ColorDictionary());

                EditorGUILayout.LabelField($"{colorType} Color", EditorStyles.boldLabel, GUILayout.MaxWidth(150f));
                DrawColorDictionary(currentTheme.gameShapeThemeDictionary.GetElementData(colorType));
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        #region Game UI Section
        if (viewingGameUISection = EditorGUILayout.BeginFoldoutHeaderGroup(viewingGameUISection, "Game UI Section"))
        {
            int gameUIKeyCount = Enum.GetNames(typeof(Theme.GameUIThemeKey)).Length;
            Theme.GameUIThemeKey currentKey;
            ThemeElementData data;
            for (int i = 0; i < gameUIKeyCount; i++)
            {
                currentKey = (Theme.GameUIThemeKey)i;
                if (!currentTheme.gameUIThemeDictionary.Contains(currentKey, out int index))
                {
                    data = new ThemeElementData();
                    currentTheme.gameUIThemeDictionary.Add(currentKey, data);
                }
                else
                    data = currentTheme.gameUIThemeDictionary.GetElementData(index);

                DrawThemeData($"{currentKey}", data);
                EditorGUILayout.Separator();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion

        #region Game Text Section
        if (viewingGameTextSection = EditorGUILayout.BeginFoldoutHeaderGroup(viewingGameTextSection, "Game Text Section"))
        {
            int textLabelCount = Enum.GetNames(typeof(Theme.TextUIThemeKey)).Length;
            Theme.TextUIThemeKey currentKey;

            for(int i = 0; i < textLabelCount; i++)
            {
                currentKey = (Theme.TextUIThemeKey)i;
                if (!currentTheme.textElementDictionary.Contains(currentKey, out int index))
                    currentTheme.textElementDictionary.Add(currentKey, new ColorDictionary());

                EditorGUILayout.LabelField($"{currentKey} Colors", EditorStyles.boldLabel, GUILayout.MaxWidth(150f));
                DrawColorDictionary(currentTheme.textElementDictionary.GetElementData(currentKey));
                EditorGUILayout.Separator();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        #endregion
        EditorGUILayout.EndScrollView();

        if (GUI.changed)
            EditorUtility.SetDirty(currentTheme);
        #endregion
    }

    #region Theme Creation Functions
    private void BeginCreatingNewTheme() { creatingTheme = true; newThemeName = ""; }
    private void ToggleActiveTheme(object themeAsset) { currentTheme = (Theme)themeAsset; }
    private void DrawThemeCreatePanel()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        newThemeName = EditorGUILayout.TextField("Theme Asset Name", newThemeName, GUILayout.MaxWidth(350f));

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Theme", GUILayout.MaxWidth(150f)))
        {
            Theme newThemeAsset = CreateInstance<Theme>();
            newThemeAsset.name = newThemeName;

            AssetDatabase.CreateAsset(newThemeAsset, $"Assets/Resources/{themeFolderPath}{newThemeName}.asset");
            AssetDatabase.Refresh();

            creatingTheme = false;
        }

        if (GUILayout.Button("Cancel", GUILayout.MaxWidth(150f)))
        {
            creatingTheme = false;
        }
        EditorGUILayout.EndHorizontal();
    }
    #endregion

    #region Draw Functions
    private void DrawColorDictionary(ColorDictionary dictionary)
    {
        int colorCount = Enum.GetNames(typeof(Theme.ColorMode)).Length;
        for(int i = 0; i < colorCount; i++)
        {
            Theme.ColorMode key = (Theme.ColorMode)i;
            if (!dictionary.Contains(key, out int index))
                dictionary.Add(key, Color.white);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{key} Color:", GUILayout.MaxWidth(150f));
            dictionary.SetValue(i, EditorGUILayout.ColorField(dictionary.GetValue(i), GUILayout.MaxWidth(150f)));
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawThemeData(string dataTitle, ThemeElementData data)
    {
        EditorGUILayout.LabelField($"{dataTitle} Data", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        data.SetSprite((Sprite)EditorGUILayout.ObjectField(data.GetElementSprite(), typeof(Sprite), false, GUILayout.MaxWidth(200f)));
        data.SetSpriteType((Image.Type)EditorGUILayout.EnumPopup(data.GetSpriteType()));
        EditorGUILayout.EndHorizontal();

        ColorDictionary dictionary = data.GetDictionary();
        DrawColorDictionary(dictionary);
    }
    #endregion
}
