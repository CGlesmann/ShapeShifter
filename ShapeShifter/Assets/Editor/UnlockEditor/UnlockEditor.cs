using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class UnlockEditor : EditorWindow
{
    private static UnlockSettings settings = null;
    private const string settingsFilePath = "Unlocks/UnlockSettings";

    private Vector2 scrollPosition = Vector2.zero;

    [MenuItem("Unlock Settings", menuItem = "Global Tools/Unlock Settings")]
    public static void ShowEditor()
    {
        UnlockEditor editor = GetWindow<UnlockEditor>();
        editor.Show();
    }

    private void GetUnlockSettings() { settings = Resources.Load<UnlockSettings>(settingsFilePath); }
    private void OnGUI()
    {
        if (settings == null)
            GetUnlockSettings();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Unlock Settings", EditorStyles.boldLabel, GUILayout.MaxWidth(150f));

        if (GUILayout.Button("Add Unlock", GUILayout.MaxWidth(150f)))
        {
            GenericMenu unlockAdditionMenu = new GenericMenu();
            List<Type> derivedUnlockTypes = GetAllUnlockTypes();

            foreach(Type dType in derivedUnlockTypes)
                unlockAdditionMenu.AddItem(new GUIContent($"Add {dType.Name}"), false, AddUnlock, CastUnlockType(dType));

            unlockAdditionMenu.ShowAsContext();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();

        GUIStyle menuHeader = new GUIStyle();
        menuHeader.fontStyle = FontStyle.Bold;
        menuHeader.fontSize = 24;
        menuHeader.normal.textColor = Color.black;
        EditorGUILayout.LabelField("Unlocks", menuHeader);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        Unlock unlock;
        for(int i = 0; i < settings.unlocks.Count; i++)
        {
            unlock = settings.unlocks[i];
            DrawBaseUnlock(unlock, i);

            EditorGUILayout.Separator();
        }
        EditorGUILayout.EndScrollView();

        if (GUI.changed)
            EditorUtility.SetDirty(settings);
    }

    #region Draw Functions
    private void DrawBaseUnlock(Unlock unlock, int index)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("X", GUILayout.MaxWidth(24)))
        {
            settings.unlocks.RemoveAt(index);
            return;
        }

        GUIStyle unlockHeader = new GUIStyle();
        unlockHeader.fontStyle = FontStyle.BoldAndItalic;
        unlockHeader.fontSize = 16;
        unlockHeader.normal.textColor = Color.black;
        EditorGUILayout.LabelField($"{unlock.GetType().Name}", unlockHeader);
        EditorGUILayout.EndHorizontal();

        unlock.unlockDescription = EditorGUILayout.TextField("Description", unlock.unlockDescription, GUILayout.MaxWidth(350f));
        unlock.unlockPrefab = (GameObject)EditorGUILayout.ObjectField("Notification Prefab", unlock.unlockPrefab, typeof(GameObject), false, GUILayout.MaxWidth(350f));

        EditorGUILayout.Separator();
        DrawTypeSpecificFields(unlock);
    }

    private void DrawTypeSpecificFields(Unlock unlock)
    {
        if (unlock.GetType() == typeof(PackUnlock))
            DrawPackUnlock(unlock as PackUnlock);
    }

    private void DrawPackUnlock(PackUnlock packUnlock)
    {
        packUnlock.packIndex = EditorGUILayout.IntField("Required Pack Index", packUnlock.packIndex, GUILayout.MaxWidth(350f));
        packUnlock.requiredLevel = EditorGUILayout.IntField("Required Level Index", packUnlock.requiredLevel, GUILayout.MaxWidth(350f));
    }
    #endregion

    #region Helper Functions
    private Unlock CastUnlockType(Type unlockType)
    {
        Unlock unlock = null;
        if (unlockType == typeof(PackUnlock))
            unlock = new PackUnlock();

        return unlock;
    }

    private void AddUnlock(object newUnlock)
    {
        if (settings == null)
            GetUnlockSettings();

        settings.unlocks.Add((Unlock)newUnlock);
    }

    private List<Type> GetAllUnlockTypes()
    {
        List<Type> challengeTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from assemblyType in domainAssembly.GetTypes()
                                     where typeof(Unlock).IsAssignableFrom(assemblyType)
                                     select assemblyType).ToList<Type>();

        challengeTypes.Remove(typeof(Unlock));
        return challengeTypes;
    }
    #endregion
}
