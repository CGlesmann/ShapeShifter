using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelBuilder : EditorWindow
{
    private GameManager gameManager = null;

    private readonly string[] tabTitles = { "Gameboard Tools", "GameSlot Tools" };
    private int tabIndex = 0;

    private float sizing = 0f;

    private SlotLock.LockType desiredLockType = SlotLock.LockType.Destruct;
    private int desiredLockTimer = 1;
    private float desiredLockSize = 0f;

    private GameShape.ShapeType desiredShapeType = GameShape.ShapeType.Circle;
    private Color desiredShapeColor = Color.white;
    private float desiredShapeSize = 0f;

    [MenuItem("Level Builder", menuItem = "Level Tools/Level Builder %&L")]
    public static void DisplayLevelBuilder()
    {
        LevelBuilder builderWindow = GetWindow<LevelBuilder>();
        builderWindow.Show();
    }

    private void OnGUI()
    {
        if (gameManager == null)
        {
            GetManagerReference();
            if (gameManager == null)
            {
                EditorGUILayout.LabelField("Navigate to a level to use the Level Builder");
                return;
            }
        }

        tabIndex = GUILayout.Toolbar(tabIndex, tabTitles, GUILayout.MaxWidth(500f));
        switch(tabIndex)
        {
            case 0:
                DrawGameboardTools();
                break;
            case 1:
                DrawGameSlotTools();
                break;
        }
    }

    private void GetManagerReference() { gameManager = GameObject.Find("LevelManager")?.GetComponent<GameManager>(); }
    private void DrawGameboardTools()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Viewing Options", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Toggle Locks", GUILayout.MaxWidth(150f)))
            GameboardTools.ToggleLocks();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Board Manipulation", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Resize GameShapes", GUILayout.MaxWidth(150f)))
            GameboardTools.ResizeGameShapes(sizing != 0 ? sizing : gameManager.shapeSize);

        sizing = EditorGUILayout.FloatField(sizing, GUILayout.MaxWidth(50f));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        if (GUILayout.Button("Clear Gameboard", GUILayout.MaxWidth(150f)))
            GameboardTools.ClearGameboard(true, true);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Shapes", GUILayout.MaxWidth(150f)))
            GameboardTools.ClearGameboard(true, false);

        if (GUILayout.Button("Clear Locks", GUILayout.MaxWidth(150f)))
            GameboardTools.ClearGameboard(false, true);
        EditorGUILayout.EndHorizontal(); 
    }

    private void DrawGameSlotTools()
    {
        EditorGUILayout.Separator();

        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects != null)
        {
            if (selectedObjects.Length > 1)
                EditorGUILayout.LabelField("Muli Select Editing isn't supported", EditorStyles.helpBox);
            else if (selectedObjects.Length == 1)
            {
                GameSlot currentSelectSlot = selectedObjects[0].GetComponent<GameSlot>();
                if (currentSelectSlot == null)
                    EditorGUILayout.LabelField("Please Select a GameSlot object to begin editing", EditorStyles.helpBox);
                else
                    DrawGameShapeOptions(currentSelectSlot);
            } else {
                EditorGUILayout.LabelField("Please Select a GameSlot object to begin editing", EditorStyles.helpBox);
            }
        }
    }

    private void DrawGameShapeOptions(GameSlot slot)
    {
        GameShape shapeData = slot.GetSlotShape();

        if (shapeData != null)
        {
            EditorGUILayout.LabelField($"Shape Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Shape Color: {shapeData.shapeColor}");
            EditorGUILayout.LabelField($"Shape Type: {shapeData.GetShapeType()}");
        }
        else
            EditorGUILayout.LabelField("Shape Info: No Shape Available");

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Lock Options", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        desiredLockType = (SlotLock.LockType)EditorGUILayout.EnumPopup(desiredLockType, GUILayout.MaxWidth(100f));
        desiredLockTimer = EditorGUILayout.IntField(desiredLockTimer, GUILayout.MaxWidth(50f));
        desiredLockSize = EditorGUILayout.FloatField(desiredLockSize, GUILayout.MaxWidth(50f));

        if (slot.GetSlotLock() != null)
        {
            if (GUILayout.Button("Edit Lock", GUILayout.MaxWidth(150f)))
                GameSlotTools.EditSlotLock(slot.transform, desiredLockType, desiredLockTimer, desiredLockSize);
        } else
            if (GUILayout.Button("Create Lock", GUILayout.MaxWidth(150f)))
                GameSlotTools.CreateSlotLock(slot.transform, desiredLockType, desiredLockTimer, desiredLockSize);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Shape Options", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        desiredShapeType = (GameShape.ShapeType)EditorGUILayout.EnumPopup(desiredShapeType, GUILayout.MaxWidth(100f));
        desiredShapeColor = EditorGUILayout.ColorField(desiredShapeColor, GUILayout.MaxWidth(50f));
        desiredShapeSize = EditorGUILayout.FloatField(desiredShapeSize, GUILayout.MaxWidth(50f));

        if (slot.GetSlotShape() != null)
        {
            if (GUILayout.Button("Edit Shape", GUILayout.MaxWidth(150f)))
                GameSlotTools.EditSlotShape(slot.transform, desiredShapeType, desiredShapeColor, desiredShapeSize);
        }
        else
            if (GUILayout.Button("Create Shape", GUILayout.MaxWidth(150f)))
            GameSlotTools.CreateSlotShape(slot.transform, desiredShapeType, desiredShapeColor, desiredShapeSize);
        EditorGUILayout.EndHorizontal();
    }
}
