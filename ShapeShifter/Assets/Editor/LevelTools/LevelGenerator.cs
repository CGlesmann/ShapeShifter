using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelGenerator : EditorWindow
{
    private static GameManager manager;

    private BoardPreferences preferences;
    private int currentTab = 0;
        
    [MenuItem("Level Generator", menuItem = "Level Tools/Level Generator")]
    public static void DisplayGenerator()
    {
        // Displaying the Window
        LevelGenerator lgWindow = CreateInstance<LevelGenerator>();
        lgWindow.Show();

        // Attempt to Retrieve the GameManager Reference
        GetManagerReference();
    }

    /// <summary>
    /// Gets the GameManager reference and stores in manager
    /// </summary>
    private static void GetManagerReference()
    {
        // Grabbing the manager reference
        manager = (GameManager)FindObjectOfType<GameManager>();
    }

    /// <summary>
    /// Function for retrieving the manager reference if lost after deselecting the window
    /// </summary>
    private void OnFocus()
    {
        if (preferences == null)
            preferences = new BoardPreferences();

        if (manager == null)
            GetManagerReference();
    }

    /// <summary>
    /// Function for drawing the window
    /// </summary>
    private void OnGUI()
    {
        // Getting tab selection
        currentTab = GUILayout.Toolbar(currentTab, new string[] { "Generate Board", "Generate Solution"});
        
        // Drawing the currently selected tab
        switch(currentTab)
        {
            // Board Generator
            case 0:
                DrawBoardGenerator();
                break;

            // Solution Generator
            case 1:
                DrawSolutionGenerator();
                break;

            // Invalid input, draw error message
            default:
                EditorGUILayout.LabelField("currentTab set to invalid value: " + currentTab.ToString());
                break;
        }
    }

    /// <summary>
    /// Function that draws all the fields related to the Board Generator
    /// Called by OnGUI
    /// </summary>
    private void DrawBoardGenerator()
    {
        // Drawing the Shape settings group
        EditorGUILayout.LabelField("Shape Settings", EditorStyles.boldLabel);

        // Shape Toggles
        EditorGUILayout.BeginHorizontal();
        preferences.squareEnabled = EditorGUILayout.Toggle("Square", preferences.squareEnabled);
        preferences.circleEnabled = EditorGUILayout.Toggle("Circle", preferences.circleEnabled);
        preferences.triangleEnabled = EditorGUILayout.Toggle("Triangle", preferences.triangleEnabled);
        preferences.diamondEnabled = EditorGUILayout.Toggle("Diamond", preferences.diamondEnabled);
        EditorGUILayout.EndHorizontal();

        // Color Toggles
        EditorGUILayout.BeginHorizontal();
        preferences.redEnabled = EditorGUILayout.Toggle("Red", preferences.redEnabled);
        preferences.blueEnabled = EditorGUILayout.Toggle("Blue", preferences.blueEnabled);
        preferences.greenEnabled = EditorGUILayout.Toggle("Green", preferences.greenEnabled);
        preferences.yellowEnabled = EditorGUILayout.Toggle("Yellow", preferences.yellowEnabled);
        EditorGUILayout.EndHorizontal();

        // Drawing the Board Settings Group
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Board Settings", EditorStyles.boldLabel);
        preferences.shapeCount = (int)EditorGUILayout.Slider("Shape Count", preferences.shapeCount, 1, manager != null ? manager.boardSize : 1);

        // Drawing the Tab Options
        EditorGUILayout.Separator();
        if (GUILayout.Button("Clear Game Board"))
        {
            LevelTools.ClearGameBoard();
        }

        if (GUILayout.Button("Generate Board"))
        {
            GenerateGameBoard();
        }
    }

    /// <summary>
    /// Function that adds colored shapes to the game board referenced by the manager
    /// Shapes are generated based on BoardPreference values
    /// </summary>
    private void GenerateGameBoard()
    {
        // Clear the Board
        LevelTools.ClearGameBoard();

        // Checking to make sure a manager and board reference are available
        if (manager == null)
        {
            Debug.LogError("Manager Reference Not Available, Can't Generate Board");
            return;
        } else if (manager.gameBoardParent == null) {
            Debug.LogError("GameBoardParent Reference Not Available, Can't Generate Board");
            return;
        }

        // Getting the List of available shape types / colors
        List<GameShape.ShapeType> availableTypes = preferences.GetAvailableTypes();
        List<Color> availableColors = preferences.GetAvailableColors();

        // Getting the board parent from the manager
        Transform boardParent = manager.gameBoardParent;

        // Getting Reference List
        List<int> availableSlots = new List<int>();
        for (int i = 0; i < boardParent.childCount; i++)
            availableSlots.Add(i);

        // Filling slots
        GameShape newShape;
        Transform slotTransform;
        int currentSlot;

        for(int i = 0; i < preferences.shapeCount; i++)
        {
            // Getting a random slot
            currentSlot = availableSlots[Random.Range(0, availableSlots.Count)];

            // Generate Shape for this Slot
            slotTransform = boardParent.GetChild(currentSlot);
            newShape = Instantiate(manager.shapePrefab, slotTransform).GetComponent<GameShape>();
            newShape.transform.localPosition = new Vector3(0.5f, -0.5f);

            newShape.SetShapeColor(availableColors[Random.Range(0, availableColors.Count)]);
            newShape.SetShapeType(availableTypes[Random.Range(0, availableTypes.Count)]);

            // Removing the current slot from available list
            availableSlots.Remove(currentSlot);
        }
    }

    /// <summary>
    /// Function that draws all the fields related to the Solution Generator
    /// Called by OnGUI
    /// </summary>
    private void DrawSolutionGenerator()
    {
        EditorGUILayout.LabelField("This is the Solution generator");
    }
}

public class BoardPreferences
{
    // Shape Variables
    public bool squareEnabled = false, circleEnabled = false, triangleEnabled = false, diamondEnabled = false;
    public bool redEnabled = false, blueEnabled = false, greenEnabled = false, yellowEnabled = false;

    // Board Variables
    public int shapeCount = 0;

    /// <summary>
    /// Public method for getting an enum list of type ShapeType
    /// </summary>
    /// <returns></returns>
    public List<GameShape.ShapeType> GetAvailableTypes()
    {
        // Creating a list
        List<GameShape.ShapeType> types = new List<GameShape.ShapeType>();

        // Getting the available types
        if (squareEnabled) types.Add(GameShape.ShapeType.Square);
        if (circleEnabled) types.Add(GameShape.ShapeType.Circle);
        if (triangleEnabled) types.Add(GameShape.ShapeType.Triangle);
        if (diamondEnabled) types.Add(GameShape.ShapeType.Diamond);

        // returning the resulting list, or null if nothing is in the list
        return types.Count > 0 ? types : null;
    }

    /// <summary>
    /// Public method for getting a color list
    /// </summary>
    /// <returns></returns>
    public List<Color> GetAvailableColors()
    {
        // Creating a list
        List<Color> types = new List<Color>();

        // Getting the available types
        if (redEnabled) types.Add(ShapeSettings.RED_COLOR);
        if (blueEnabled) types.Add(ShapeSettings.BLUE_COLOR);
        if (greenEnabled) types.Add(ShapeSettings.GREEN_COLOR);
        if (yellowEnabled) types.Add(ShapeSettings.YELLOW_COLOR);

        // returning the resulting list, or null if nothing is in the list
        return types.Count > 0 ? types : null;
    }
}
