using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelGenerator : EditorWindow
{
    private static GameManager manager;

    private BoardPreferences boardPreferences;
    private SolutionPreferences solutionPreferences;
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
        if (boardPreferences == null)
            boardPreferences = new BoardPreferences();
        if (solutionPreferences == null)
            solutionPreferences = new SolutionPreferences();

        if (manager == null)
            GetManagerReference();
    }

    /// <summary>
    /// Function for drawing the window
    /// </summary>
    private void OnGUI()
    {
        if (manager != null)
        {
            // Getting tab selection
            currentTab = GUILayout.Toolbar(currentTab, new string[] { "Generate Board", "Generate Solution" });

            // Drawing the currently selected tab
            switch (currentTab)
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
        } else
            EditorGUILayout.LabelField("Level Generator isn't available in menu scenes", EditorStyles.boldLabel);
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
        boardPreferences.squareEnabled = EditorGUILayout.Toggle("Square", boardPreferences.squareEnabled);
        boardPreferences.circleEnabled = EditorGUILayout.Toggle("Circle", boardPreferences.circleEnabled);
        boardPreferences.triangleEnabled = EditorGUILayout.Toggle("Triangle", boardPreferences.triangleEnabled);
        boardPreferences.diamondEnabled = EditorGUILayout.Toggle("Diamond", boardPreferences.diamondEnabled);
        EditorGUILayout.EndHorizontal();

        // Color Toggles
        EditorGUILayout.BeginHorizontal();
        boardPreferences.redEnabled = EditorGUILayout.Toggle("Red", boardPreferences.redEnabled);
        boardPreferences.blueEnabled = EditorGUILayout.Toggle("Blue", boardPreferences.blueEnabled);
        boardPreferences.greenEnabled = EditorGUILayout.Toggle("Green", boardPreferences.greenEnabled);
        boardPreferences.yellowEnabled = EditorGUILayout.Toggle("Yellow", boardPreferences.yellowEnabled);
        EditorGUILayout.EndHorizontal();

        // Drawing the Board Settings Group
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Board Settings", EditorStyles.boldLabel);
        boardPreferences.shapeCount = (int)EditorGUILayout.Slider("Shape Count", boardPreferences.shapeCount, 1, manager != null ? manager.boardSize : 1);

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
        List<GameShape.ShapeType> availableTypes = boardPreferences.GetAvailableTypes();
        List<Color> availableColors = boardPreferences.GetAvailableColors();

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

        for(int i = 0; i < boardPreferences.shapeCount; i++)
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
        EditorGUILayout.LabelField("Solution Settings", EditorStyles.boldLabel);
        solutionPreferences.amountOfMoves = EditorGUILayout.IntField("Amount of Moves", solutionPreferences.amountOfMoves);
        EditorGUILayout.BeginHorizontal();
        solutionPreferences.shapeDestroyEnabled = EditorGUILayout.Toggle("Destroy By Shape", solutionPreferences.shapeDestroyEnabled);
        solutionPreferences.colorDestroyEnabled = EditorGUILayout.Toggle("Destroy By Color", solutionPreferences.colorDestroyEnabled);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Separator();

        if (GUILayout.Button("Clear Solution"))
            LevelTools.ClearSolutionBoard();

        if (GUILayout.Button("Generate Solution"))
            GenerateSolution();

        if (GUILayout.Button("Toggle Active Boards"))
            manager.ToggleSolutionBoard();
    }

    private void GenerateSolution()
    {
        // Clear the Solution Board
        LevelTools.ClearSolutionBoard();

        // Adding all of the shapes from the game board onto the solution board
        List<int> availableSlots = new List<int>(); // This list tracks which slots have shapes, used for selecting slots

        GameShape newShape, refShape;
        GameSlot gbShapeSlot;
        Transform gameBoardParent = manager.gameBoardParent;
        Transform solutionBoardParent = manager.solutionBoardParent;
        Transform gbSlot, sbSlot;

        for (int i = 0; i < gameBoardParent.childCount; i++)
        {
            gbSlot = gameBoardParent.GetChild(i);
            sbSlot = solutionBoardParent.GetChild(i);

            gbShapeSlot = gbSlot.GetComponent<GameSlot>();
            refShape = gbShapeSlot != null ? gbShapeSlot.GetSlotShape() : null;

            if (refShape != null)
            {
                newShape = Instantiate(manager.shapePrefab, sbSlot).GetComponent<GameShape>();
                newShape.transform.localPosition = new Vector3(0.5f, -0.5f);

                newShape.SetShapeColor(refShape.GetShapeColor());
                newShape.SetShapeType(refShape.GetShapeType());

                availableSlots.Add(i);
            }
        }

        // Making amountOfMoves moves on the solution board
        List<GameManager.DestroyMethod> availableMethods = solutionPreferences.GetAvailableDestroyMethods();
        GameManager.DestroyMethod method;
        int slot1Index, slot2Index;
        GameSlot slot1, slot2;
        for(int i = 0; i < solutionPreferences.amountOfMoves; i++)
        {
            // Setting the Destroy By Method...
            method = availableMethods[Random.Range(0, availableMethods.Count)];
            manager.ToggleDestoryMethod(method);

            // Getting two slot references
            slot1Index = availableSlots[Random.Range(0, availableSlots.Count)];
            slot1 = solutionBoardParent.GetChild(slot1Index).GetComponent<GameSlot>();
            availableSlots.Remove(slot1Index);

            slot2Index = availableSlots[Random.Range(0, availableSlots.Count)];
            slot2 = solutionBoardParent.GetChild(slot2Index).GetComponent<GameSlot>();

            Debug.Log("DestroyMethod: " + method.ToString() + " | " + "Switching SlotShape " + slot1Index + " and SlotShape " + slot2Index);

            // Selecting two slots
            manager.SwitchSolutionShapes(slot1, slot1Index, slot2, slot2Index);

            availableSlots.Clear();
            for(int j = 0; j < solutionBoardParent.childCount; j++)
            {
                if (solutionBoardParent.GetChild(j).GetComponent<GameSlot>().GetSlotShape() != null)
                    availableSlots.Add(j);
            }
        }
        
    }
}

public class BoardPreferences
{
    // Shape Variables
    public bool squareEnabled = false, circleEnabled = false, triangleEnabled = false, diamondEnabled = false;
    public bool redEnabled = false, blueEnabled = false, greenEnabled = false, yellowEnabled = false;

    // Board Variables
    public int shapeCount = 1;

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

public class SolutionPreferences
{
    public int amountOfMoves = 1;
    public bool colorDestroyEnabled = true, shapeDestroyEnabled = true;

    public List<GameManager.DestroyMethod> GetAvailableDestroyMethods()
    {
        // Create the tracker list
        List<GameManager.DestroyMethod> methods = new List<GameManager.DestroyMethod>();

        // Adding available methods to tracker list
        if (colorDestroyEnabled) methods.Add(GameManager.DestroyMethod.Color);
        if (shapeDestroyEnabled) methods.Add(GameManager.DestroyMethod.Shape);

        // return the resulting list
        return methods.Count > 0 ? methods : null;
    }

}
