using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Declaring a singleton
    public static GameManager manager = null;
    
    // Destroy Control Toggle
    public enum DestroyMethod { Shape, Color };

    [Header("Scene Navigation Variables")]
    public int levelIndex = 0;
    [SerializeField] private string mainMenuScene = "";
    [SerializeField] private string nextLevelScene = "";
    private string currentLevelName = "";

    [Header("Control Variables")]
    [SerializeField] private DestroyMethod currentDestoryMethod = DestroyMethod.Shape;
    public float levelTimer = 0f;

    private GameSlot slot1 = null, slot2 = null; // References for selected slots
    private bool switching = false;
    private bool checkVictory = false;

    [Header("Board Settings")]
    [SerializeField] public float shapeSize = 2.25f;
    [SerializeField] private int boardWidth = 4;
    [SerializeField] private int boardHeight = 4;
    [HideInInspector] public int shapesBeingDestroyed = 0;
    public int boardSize => boardWidth * boardHeight;

    [Header("Solution Board Settings")]
    [SerializeField] private float solutionDisplayTime = 10f; // In Seconds
    private float solutionTimer = 0;

    [Header("Object References")]
    public ChallengeManager challengeManager = null;
    public UndoManager undoManager = null;
    public Transform gameBoardParent = null;
    public Transform solutionBoardParent = null;

    [Header("Prefab References")]
    public GameObject shapePrefab = null;

    public delegate void OnClockTick();
    public static event OnClockTick onClockTick;

    public delegate void OnShapeSwap();
    public static event OnShapeSwap onShapeSwap;

    public delegate void OnUpdateBoard();
    public static event OnUpdateBoard onUpdateBoard;

    [Header("GUI References")]
    [SerializeField] private GameObject pauseMenuParent = null;
    [SerializeField] private GameObject victoryMenuParent = null;
    [SerializeField] private TextMeshProUGUI destroyText = null;
    [SerializeField] private TextMeshProUGUI solutionText = null;
    [SerializeField] private TextMeshProUGUI gameTimerText = null;
    [SerializeField] private Image solutionTimerUI = null;

    [Header("Defeat UI References")]
    [SerializeField] private GameObject defeatScreenParent = null;
    [SerializeField] private DefeatInstructions defeatInstructions = null;
    [SerializeField] private TextMeshProUGUI failureText = null;
    Dictionary<int, ShapeData> requiredShapes;

    #region Unity Functions
    private void OnEnable() { manager = this; }

    /// <summary>
    /// Setting Default State
    /// </summary>
    private void Awake()
    {
        currentLevelName = SceneManager.GetActiveScene().name.Split('_')[1];
        gameTimerText.text = $"Level {currentLevelName} : 00:00";

        // Setting the default destroy settings
        currentDestoryMethod = DestroyMethod.Shape;
        destroyText.text = "Destroy by Shape";

        // Getting the required shapes
        GameShape shape;
        requiredShapes = new Dictionary<int, ShapeData>();

        // Getting the required Shapes/Slots
        for(int i = 0; i < solutionBoardParent.childCount; i++)
        {
            // Attempting to get the shape at the given index
            shape = solutionBoardParent.GetChild(i).gameObject.GetComponent<GameSlot>().GetSlotShape();

            // Adding shape data if a shape exists
            if (shape != null)
                requiredShapes.Add(i, shape.GetShapeData());
        }

        // Setting the gameboard/solution board
        SetGameSlotIndexes();
        shapesBeingDestroyed = 0;
    }

    /// <summary>
    /// Managing the Solution board timer
    /// </summary>
    private void Update()
    {
        // Checking for paused state
        if (!GameState.gamePaused)
        {
            // Checking for an active timer
            if (solutionTimer > 0f)
            {
                // Decrementing the timer
                solutionTimer -= Time.deltaTime;

                // Checking for solution board toggle
                if (solutionTimer <= 0f)
                    HideSolutionBoard();
                else
                {
                    // Updating the solution timer element
                    solutionTimerUI.fillAmount = (solutionTimer / solutionDisplayTime);
                }
            }

            // Updating Game Timer
            levelTimer += Time.deltaTime;
            onClockTick?.Invoke();

            if (gameTimerText != null)
                gameTimerText.text = string.Format("Level {0} : {1}", currentLevelName, GameTime.GetGameTimeFormat(levelTimer));
        }
    }

    /// <summary>
    /// Checking for victory (as needed)
    /// </summary>
    private void FixedUpdate()
    {
        // Checking for paused state
        if (!GameState.gamePaused)
        {
            // Checking for victory (if nessecary)
            if (checkVictory && shapesBeingDestroyed <= 0)
            {
                // Check for level completion
                if (!CheckForVictory())
                    if (!DataTracker.gameData.defeatTutorialComplete)
                        CheckForDefeat();

                // Mark as checked
                checkVictory = false;

                // Update Board
                onUpdateBoard?.Invoke();
            }
        }
    }

    /// <summary>
    /// Resets the Static reference
    /// </summary>
    private void OnDestroy() { manager = null; }
    #endregion

    #region Getter Functions
    public GameSlot GetGameSlot(int slotIndex, Transform boardParent) { return boardParent.GetChild(slotIndex).GetComponent<GameSlot>(); }
    public ShapeData GetBoardShapeData(int slotIndex, Transform boardParent) { return GetGameSlot(slotIndex, boardParent)?.GetSlotShape().GetShapeData() ?? null; }
    #endregion

    #region Setter Functions
    /// <summary>
    /// Selects a slot, switches the shapes if two slots are selected
    /// </summary>
    /// <param name="targetSlot"></param>
    public bool SelectSlot(GameSlot targetSlot)
    {
        // Check for two shapes switching around
        if (!switching)
        {
            // Checking for which reference
            if (slot1 == null)
            {
                slot1 = targetSlot;
                return true;
            }
            else if (slot2 == null)
            {
                // Two Slots Selected, switch the shapes
                slot2 = targetSlot;
                StartCoroutine(MoveShapes());

                onShapeSwap?.Invoke();
                return true;
            }
        }

        // Returning the default case
        return false;
    }

    /// <summary>
    /// Deselects the currently selected shape
    /// This can only be called when one shape is selected
    /// Therefore this always reset shape1 reference
    /// </summary>
    public void DeselectSlot() { if (!switching) slot1 = null; }

    /// <summary>
    /// Switches the current destroy method to the opposite method
    /// Used for toggle button
    /// </summary>
    public void ToggleDestoryMethod()
    {
        switch(currentDestoryMethod)
        {
            case DestroyMethod.Shape:
                currentDestoryMethod = DestroyMethod.Color;
                destroyText.text = "Destroy by Color";
                break;
            case DestroyMethod.Color:
                currentDestoryMethod = DestroyMethod.Shape;
                destroyText.text = "Destroy by Shape";
                break;
            default:
                destroyText.text = "Unknown Destroy Method";
                break;
        }
    }

    public void ToggleDestoryMethod(DestroyMethod method)
    {
        switch (method)
        {
            case DestroyMethod.Shape:
                currentDestoryMethod = DestroyMethod.Shape;
                destroyText.text = "Destroy by Shape";
                break;
            case DestroyMethod.Color:
                currentDestoryMethod = DestroyMethod.Color;
                destroyText.text = "Destroy by Color";
                break;
            default:
                destroyText.text = "Unknown Destroy Method";
                break;
        }
    }
    #endregion

    #region In Game Functions
    public void RestoreBoardState(BoardData data)
    {
        // Declaring Tracker Variables
        GameSlot slot;
        GameShape shape;

        // Loop through the board and set the board state to the data
        for (int i = 0; i < gameBoardParent.childCount; i++)
        {
            // Getting the Game Slot/Shape Reference
            slot = gameBoardParent.GetChild(i).GetComponent<GameSlot>();
            shape = slot.GetSlotShape();

            // Checking if a shape exists in the given slot
            if (shape != null)
            {
                // Checking if a shape doesn't exist on the passed in data
                if (data.board[i] == null)
                    Destroy(shape.gameObject);
                else if (shape.GetShapeData() != data.board[i])
                {
                    // Shape does exist but doesn't match the shape in the data, set them to be equivalent
                    shape.SetShapeColor(data.board[i].shapeColor);
                    shape.SetShapeType(data.board[i].shapeType);
                }
            }
            else
            {
                // Checking to see if a shape exists in the data at the given spot, if so create a new one
                if (data.board[i] != null)
                {
                    // Creating the shape
                    GameObject newShape = Instantiate(shapePrefab, slot.transform);
                    newShape.transform.localPosition = new Vector3(0.5f, -0.5f, 0f);
                    newShape.transform.localScale = new Vector3(shapeSize, shapeSize, 0f);

                    // Getting the GameShape reference and setting it equal to the shape in the data
                    GameShape shapeRef = newShape.GetComponent<GameShape>();
                    shapeRef.SetShapeColor(data.board[i].shapeColor);
                    shapeRef.SetShapeType(data.board[i].shapeType);
                }
            }
        }
    }

    /// <summary>
    /// Toggles the solution boards visibility
    /// </summary>
    public void ToggleSolutionBoard()
    {
        // Checks if two shapes are being switched
        if (!switching)
        {
            // Checking for an active game board
            if (gameBoardParent.gameObject.activeSelf)
            {
                ShowSolutionBoard();
                if (solutionText != null)
                    solutionText.text = "View GameBoard";
            }
            else
            {
                // Checking for an active solution board
                HideSolutionBoard();
                if (solutionText != null)
                    solutionText.text = "View SolutionBoard";
            }
        }
    }

    /// <summary>
    /// Disables the game board and enables the solution board
    /// </summary>
    public void ShowSolutionBoard()
    {
        // Checking for an active timer
        if (solutionTimer <= 0f)
        {
            // Setting the Solution Timer
            // solutionTimer = solutionDisplayTime;

            // Enabling the timer element
            // solutionTimerUI.gameObject.SetActive(true);
            solutionTimerUI.fillAmount = 1;

            // Toggling the Solution board / Gameboard
            gameBoardParent.gameObject.SetActive(false);
            solutionBoardParent.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Hides and solution board and enables the game board
    /// </summary>
    private void HideSolutionBoard()
    {
        // Disabling the timer element
        solutionTimerUI.gameObject.SetActive(false);

        // Toggling the Solution board / Gameboard
        gameBoardParent.gameObject.SetActive(true);
        solutionBoardParent.gameObject.SetActive(false);
    }

    /// <summary>
    /// Sets all of the slot indexes
    /// </summary>
    public void SetGameSlotIndexes()
    {
        // Declaring the temp store variable
        GameSlot slot = null;

        // Setting the indexes on the solution board
        if (solutionBoardParent != null)
        {
            for (int i = 0; i < solutionBoardParent.childCount; i++)
            {
                slot = solutionBoardParent.GetChild(i).GetComponent<GameSlot>();
                slot.SetSlotIndex(i);
            }
        }

        // Setting the indexes on the game board
        if (gameBoardParent != null)
        {
            for (int i = 0; i < gameBoardParent.childCount; i++)
            {
                slot = gameBoardParent.GetChild(i).GetComponent<GameSlot>();
                slot.SetSlotIndex(i);
            }
        }
    }

    /// <summary>
    /// Switches the two slot shapes and resets the slot states
    /// </summary>
    public void SwitchShapes()
    {
        // Storing Board state in the UndoManager
        undoManager.ProcessGameBoard(gameBoardParent);

        // Setting the parents
        slot1.GetSlotShape().transform.SetParent(slot2.transform);
        slot2.GetSlotShape().transform.SetParent(slot1.transform);

        // Triggering Shape Destruction(s)
        TriggerShapeDestruction(slot1.GetSlotIndex(), gameBoardParent);
        TriggerShapeDestruction(slot2.GetSlotIndex(), gameBoardParent);

        // Reset each slot
        slot1.ResetSlotState();
        slot2.ResetSlotState();

        // Resetting Slot References
        slot1 = null;
        slot2 = null;

        // Flag Victory Check
        checkVictory = true;
    }

    public void SwitchSolutionShapes(GameSlot s1, int s1Index, GameSlot s2, int s2Index)
    {
        // Setting the parents
        GameShape s = s1.GetSlotShape();
        s.transform.SetParent(s2.transform);
        s.transform.localPosition = new Vector3(0.5f, -0.5f);

        s = s2.GetSlotShape();
        s.transform.SetParent(s1.transform);
        s.transform.localPosition = new Vector3(0.5f, -0.5f);

        // Triggering Shape Destruction(s)
        TriggerShapeDestruction(s1Index, solutionBoardParent);
        TriggerShapeDestruction(s2Index, solutionBoardParent);
    }

    private IEnumerator MoveShapes()
    {
        // Setting the control variable
        switching = true;

        // Declaring temporary storeage variables
        Transform shape1P = slot1.GetSlotShape().transform, shape2P = slot2.GetSlotShape().transform;

        Vector3 shape1StartPosition = shape1P.position, shape2StartPosition = shape2P.position;
        float mult = 1f / (10f/24f);
        float progress = 0;

        while (progress < 1f)
        {
            // Incrementing the progress
            progress += Mathf.Clamp(Time.deltaTime * mult, 0f, 1f);

            // Moving the shapes
            shape1P.position = Vector3.Lerp(shape1StartPosition, shape2StartPosition, progress);
            shape2P.position = Vector3.Lerp(shape2StartPosition, shape1StartPosition, progress);

            // Frame delay
            yield return null;
        }

        SwitchShapes();
        switching = false;
    }

    /// <summary>
    /// Checking for a valid match
    /// </summary>
    /// <param name="shape1"></param>
    /// <param name="shape2"></param>
    /// <returns></returns>
    public bool CheckForMatch(GameShape shape1, GameShape shape2)
    {
        switch(currentDestoryMethod)
        {
            case DestroyMethod.Shape:
                return (shape1.GetShapeType() == shape2.GetShapeType());
            case DestroyMethod.Color:
                return (shape1.GetShapeColor() == shape2.GetShapeColor());
            default:
                return false;
        }
    }

    /// <summary>
    /// Returns a list of indexes of slots that contain shapes which also surround the center index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public List<int> CheckForSurroundingShapes(int index, Transform boardParent)
    {
        // Declaring temp storage variable
        List<int> indexes = new List<int>();
        int i;

        GameSlot slot;

        // Top left slot
        i = index - (boardWidth + 1);
        if (index % boardWidth != 0 && i >= 0 && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null)
                indexes.Add(i);
        }

        // Top Center Slot
        i = (index - (boardWidth + 0));
        if (index >= boardWidth && i >= 0 && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null)
                indexes.Add(i);
        }

        // Top Right Slot
        i = index - (boardWidth - 1);
        if ((index + 1) % boardWidth != 0 && i >= 0 && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null)
                indexes.Add(i);
        }

        // Middle Left Slot
        i = index - 1;
        if (index % boardWidth != 0 && i >= 0 && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null)
                indexes.Add(i);
        }

        // Middle Right Slot
        i = index + 1;
        if ((index + 1) % boardWidth != 0 && i >= 0 && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null)
                indexes.Add(i);
        }

        // Bottom left slot
        i = index + (boardWidth - 1);
        if (index % boardWidth != 0 && i < boardSize && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null)
                indexes.Add(i);
        }

        // Bottom Center Slot
        i = index + boardWidth;
        if (index <= (boardSize - boardWidth) && i < boardSize && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null)
                indexes.Add(i);
        }

        // Bottom Right Slot
        i = index + (boardWidth + 1);
        if ((index + 1) % boardWidth != 0 && i < boardSize && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null)
                indexes.Add(i);
        }

        return indexes;
    }

    /// <summary>
    /// Triggers a destruction of shapes around the given index
    /// </summary>
    /// <param name="index"></param>
    public void TriggerShapeDestruction(int index, Transform boardParent)
    {
        // Getting the current slot
        GameShape centerShape = boardParent.GetChild(index).GetComponent<GameSlot>().GetSlotShape();
        if (centerShape == null)
        {
            Debug.LogError("Couldn't find shape when triggering destruction at slot " + index);
            return;
        }

        // Declaring temp storage variable
        List<int> targetIndexes = CheckForSurroundingShapes(index, boardParent);
        bool destoryCurrentSlot = false;
        GameSlot slot;
        GameShape shape = null;

        if (targetIndexes != null)
        {
            foreach (int i in targetIndexes)
            {
                slot = boardParent.GetChild(i).GetComponent<GameSlot>();
                shape = slot.GetSlotShape();

                if (CheckForMatch(centerShape, shape))
                {
                    // Check to make sure the shape is marked for destruction
                    if (!shape.IsMarkedForDestruct())
                    {
                        shape.TriggerDestruction();
                        shape.MarkForDestruction();
                        shapesBeingDestroyed++;
                    }

                    // Marking the center shape for destruction
                    destoryCurrentSlot = true;
                }
            }
        }

        // Center Slot
        if (destoryCurrentSlot)
        {
            if (!centerShape.IsMarkedForDestruct())
            {
                centerShape.TriggerDestruction();
                centerShape.MarkForDestruction();
                shapesBeingDestroyed++;
            }
        }
    }
    #endregion

    #region State Check Functions
    public bool CheckForDefeat()
    {
        // Declaring tracker variables
        GameShape shape;
        List<ShapeData> neededShapes = new List<ShapeData>();
        foreach (KeyValuePair<int, ShapeData> pair in requiredShapes)
            neededShapes.Add(pair.Value); 

        for (int i = 0; i < gameBoardParent.childCount; i++)
        {
            // Case 1: Critical slots are empty
            if (requiredShapes.Count > 0)
            {
                if (requiredShapes.TryGetValue(i, out ShapeData data))
                {
                    // Getting the corresponding shape on the game board
                    shape = gameBoardParent.GetChild(i).GetComponent<GameSlot>().GetSlotShape();

                    // If a shape isn't on the gameboard, critical slot is empty
                    if (shape == null)
                    {
                        Debug.Log("Defeat based on case 1");
                        defeatInstructions.InvokeInstructions();
                        return true;
                    }
                }
            }

            // Case 2: Not Enough shapes remain
            shape = gameBoardParent.GetChild(i).GetComponent<GameSlot>().GetSlotShape();
            if (shape != null && neededShapes.Contains(shape.GetShapeData()))
                neededShapes.Remove(shape.GetShapeData());

            if (i == gameBoardParent.childCount - 1 && neededShapes.Count > 0)
            {
                Debug.Log("Defeat based on case 2");
                foreach (ShapeData s in neededShapes)
                    Debug.Log(s);

                defeatInstructions.InvokeInstructions();
                return true;
            }
        }

        // All cases pass, return true
        return false;
    }

    public bool CheckForVictory()
    {
        // Setting the storage vars
        GameSlot slot1 = null, slot2 = null;
        GameShape shape1 = null, shape2 = null;

        // Check each square
        for(int i = 0; i < gameBoardParent.childCount; i++)
        {
            // Getting each slot reference
            slot1 = gameBoardParent.GetChild(i).GetComponent<GameSlot>();
            slot2 = solutionBoardParent.GetChild(i).GetComponent<GameSlot>();

            // Getting Shape References
            shape1 = slot1 != null ? slot1.GetSlotShape() : null;
            shape2 = slot2 != null ? slot2.GetSlotShape() : null;

            // Comparing the two shapes from each slot, return false if shapes aren't equal
            if ((shape1 == null && shape2 != null) || (shape1 != null && shape2 == null))
            {
                Debug.Log("Victory failed at index " + i.ToString() + "\nResults were...\n" + "Slot1: " + slot1.GetSlotShape() + "\nSlot2: " + slot2.GetSlotShape());
                return false;
            }
            else if (shape1 != null && shape2 != null)
                if (!slot1.GetSlotShape().Equals(slot2.GetSlotShape()))
                {
                    Debug.Log("Victory failed at index " + i.ToString() + "\nResults were...\n" + "Slot1: " + slot1.GetSlotShape() + "\nSlot2: " + slot2.GetSlotShape());
                    return false;
                }
        }

        // All the slots evaluate as true, trigger Complete Level
        Debug.Log("Level Completed");
        CompleteLevel();
        return true;
    }

    public void CompleteLevel()
    {
        shapesBeingDestroyed = 0;
        DisplayVictoryScreen();

        if (Int32.TryParse(SceneManager.GetActiveScene().name.Split('-')[1] ?? "", out int completedLevel))
        {
            if (completedLevel > DataTracker.gameData.highestCompletedLevel)
                DataTracker.gameData.highestCompletedLevel = completedLevel;

            DataTracker.dataTracker.SaveData();
        }
    }
    #endregion

    #region GUI Functions
    public void PauseGame() { GameState.gamePaused = true; }
    public void ResumeGame() { GameState.gamePaused = false; }

    public void DisplayPauseMenu() { pauseMenuParent.SetActive(true); }
    public void HidePauseMenu() { pauseMenuParent.SetActive(false); }

    public void DisplayVictoryScreen() { GameState.gamePaused = true; checkVictory = false; victoryMenuParent.SetActive(true); challengeManager.UpdateChallengeEntries(); }
    public void HideVictoryScreen() { victoryMenuParent.SetActive(false); GameState.gamePaused = false; }

    public void DisplayDefeatScreen(string defeatText) { defeatScreenParent.SetActive(true); failureText.text = defeatText; GameState.gamePaused = true; }
    #endregion

    #region Scene Navigation Functions
    public void RestartCurrentLevel() { GameState.gamePaused = false; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void ExitToMainMenu() { GameState.gamePaused = false; SceneManager.LoadScene(mainMenuScene); }
    public void NavigateToNextLevel() { AdManager.CheckForAutomaticAd(GameTime.GetMinuteCount(levelTimer), nextLevelScene); /*HideVictoryScreen();*/ GameState.gamePaused = false; }
    #endregion
}
