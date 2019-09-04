using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Declaring Time Constants
    private const int DAY_IN_SECOND = 86400, HOUR_IN_SECOND = 3600, MINUTE_IN_SECOND = 60;
    
    // Destroy Control Toggle
    public enum DestroyMethod { Shape, Color };

    [Header("Global Color Constants")]
    [SerializeField] private Color BLUE_COLOR = Color.blue;
    [SerializeField] private Color RED_COLOR = Color.red;
    [SerializeField] private Color GREEN_COLOR = Color.green;
    [SerializeField] private Color YELLOW_COLOR = Color.yellow;

    [Header("Global Shape Constants")]
    [SerializeField] private Sprite SQUARE_SPRITE = null;
    [SerializeField] private Sprite CIRCLE_SPRITE = null;
    [SerializeField] private Sprite TRIANGLE_SPRITE = null;
    [SerializeField] private Sprite DIAMOND_SPRITE = null;

    [Header("Scene Navigation Variables")]
    [SerializeField] private string mainMenuScene = "";
    [SerializeField] private string nextLevelScene = "";

    [Header("Control Variables")]
    [SerializeField] private DestroyMethod currentDestoryMethod = DestroyMethod.Shape;
    public float levelTimer = 0f;

    private GameSlot slot1 = null, slot2 = null; // References for selected slots
    private bool checkVictory = false;

    [Header("Board Settings")]
    [SerializeField] private int boardWidth = 4;
    [SerializeField] private int boardHeight = 4;
    private int boardSize => boardWidth * boardHeight;

    [Header("Solution Board Settings")]
    [SerializeField] private float solutionDisplayTime = 10f; // In Seconds
    private float solutionTimer = 0;

    [Header("Object References")]
    [SerializeField] private Transform gameBoardParent = null;
    [SerializeField] private Transform solutionBoardParent = null;

    [Header("GUI References")]
    [SerializeField] private GameObject pauseMenuParent = null;
    [SerializeField] private GameObject endLevelMenuParent = null;
    [SerializeField] private TextMeshProUGUI destroyText = null;
    [SerializeField] private TextMeshProUGUI gameTimerText = null;
    [SerializeField] private Image solutionTimerUI = null;

    #region Unity Functions
    /// <summary>
    /// Setting Default State
    /// </summary>
    private void Awake()
    {
        Debug.Log("Target Frame rate: " + Application.targetFrameRate);

        currentDestoryMethod = DestroyMethod.Shape;
        destroyText.text = "Destroy by Shape";

        SetGameSlotIndexes();
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
            gameTimerText.text = GetGameTime();
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
            if (Input.GetKeyDown(KeyCode.Space) || checkVictory)
            {
                // Check for level completion
                CheckForVictory();

                // Mark as checked
                checkVictory = false;
            }
        }
    }
    #endregion

    #region Getter Functions
    /// <summary>
    /// Returns the Global Blue Color
    /// </summary>
    /// <returns></returns>
    public Color GetBlueColor() { return BLUE_COLOR; }

    /// <summary>
    /// Returns the Global Red Color
    /// </summary>
    /// <returns></returns>
    public Color GetRedColor() { return RED_COLOR; }

    /// <summary>
    /// Returns the Global Green Color
    /// </summary>
    /// <returns></returns>
    public Color GetGreenColor() { return GREEN_COLOR; }

    /// <summary>
    /// Returns the Global Yellow Color
    /// </summary>
    /// <returns></returns>
    public Color GetYellowColor() { return YELLOW_COLOR; }

    /// <summary>
    /// Returns the global square sprite
    /// </summary>
    /// <returns></returns>
    public Sprite GetSqureSprite() { return SQUARE_SPRITE; }

    /// <summary>
    /// Returns the global circle sprite
    /// </summary>
    /// <returns></returns>
    public Sprite GetCircleSprite() { return CIRCLE_SPRITE; }

    /// <summary>
    /// Returns the global triangle sprite
    /// </summary>
    /// <returns></returns>
    public Sprite GetTriangleSprite() { return TRIANGLE_SPRITE; }

    /// <summary>
    /// Returns the global diamond sprite
    /// </summary>
    /// <returns></returns>
    public Sprite GetDiamondSprite() { return DIAMOND_SPRITE; }

    public GameSlot GetGameBoardslot(int slotIndex) { return gameBoardParent.GetChild(slotIndex).GetComponent<GameSlot>(); }

    public string GetGameTime()
    {
        // Declaring the store variable
        string time = "";
        int seconds = Mathf.RoundToInt(levelTimer);

        // Checking days
        int dayCount = Mathf.FloorToInt(seconds / DAY_IN_SECOND);

        // Checking hours
        seconds -= (dayCount * DAY_IN_SECOND);
        int hourCount = Mathf.FloorToInt(seconds / HOUR_IN_SECOND);

        // Checking Minutes
        seconds -= (hourCount * HOUR_IN_SECOND);
        int minuteCount = Mathf.FloorToInt(seconds / MINUTE_IN_SECOND);

        // Removing minutes from seconds timer
        seconds -= (minuteCount * 60);

        // Formatting the time string
        if (dayCount > 0)
            time += dayCount < 10 ? "0" + dayCount + ":" : dayCount + ":";
        if (hourCount > 0)
            time += hourCount < 10 ? "0" + hourCount + ":" : hourCount + ":";

        time += minuteCount < 10 ? "0" + minuteCount + ":" : minuteCount + ":";
        time += seconds < 10 ? "0" + seconds : seconds.ToString();

        // Returning the resulting time
        return time;
    }
    #endregion

    #region Setter Functions
    /// <summary>
    /// Selects a slot, switches the shapes if two slots are selected
    /// </summary>
    /// <param name="targetSlot"></param>
    public void SelectSlot(GameSlot targetSlot)
    {
        // Checking for which reference
        if (slot1 == null)
            slot1 = targetSlot;
        else if (slot2 == null)
        {
            // Two Slots Selected, switch the shapes
            slot2 = targetSlot;
            SwitchShapes();
        }
    }

    /// <summary>
    /// Deselects the currently selected shape
    /// This can only be called when one shape is selected
    /// Therefore this always reset shape1 reference
    /// </summary>
    public void DeselectSlot() { slot1 = null; }

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
    #endregion

    #region In Game Functions
    /// <summary>
    /// Disables the game board and enables the solution board
    /// </summary>
    public void ShowSolutionBoard()
    {
        // Checking for an active timer
        if (solutionTimer <= 0f)
        {
            // Setting the Solution Timer
            solutionTimer = solutionDisplayTime;

            // Enabling the timer element
            solutionTimerUI.gameObject.SetActive(true);
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

        // Setting the indexes on the game board
        if (gameBoardParent != null)
        {
            for (int i = 0; i < gameBoardParent.childCount; i++)
            {
                slot = gameBoardParent.GetChild(i).GetComponent<GameSlot>();

                Debug.Log("Setting " + slot.name + " to " + i);
                slot.SetSlotIndex(i);
            }
        }

        // Setting the indexes on the solution board
        if (solutionBoardParent != null)
        {
            for (int i = 0; i < solutionBoardParent.childCount; i++)
            {
                slot = solutionBoardParent.GetChild(i).GetComponent<GameSlot>();
                slot.SetSlotIndex(i);
            }
        }
    }

    /// <summary>
    /// Switches the two slot shapes and resets the slot states
    /// </summary>
    public void SwitchShapes()
    {
        Debug.Log("switching " + slot1.name + " and " + slot2.name);

        // Get the shape references and store in temp veriables
        GameShape.ShapeType shape1Type = slot1.GetSlotShape().GetShapeType(), shape2Type = slot2.GetSlotShape().GetShapeType();
        Color shape1Color = slot1.GetSlotShape().GetShapeColor(), shape2Color = slot2.GetSlotShape().GetShapeColor();

        // Sets each of the slots shapes
        slot1.SetSlotShape(shape2Type, shape2Color);
        slot2.SetSlotShape(shape1Type, shape1Color);

        // Triggering Shape Destruction(s)
        TriggerShapeDestruction(slot1.GetSlotIndex());
        TriggerShapeDestruction(slot2.GetSlotIndex());

        // Reset each slot
        slot1.ResetSlotState();
        slot2.ResetSlotState();

        // Resetting Slot References
        slot1 = null;
        slot2 = null;

        // Flag Victory Check
        checkVictory = true;
    }

    /// <summary>
    /// Checking for a valid match
    /// </summary>
    /// <param name="shape1"></param>
    /// <param name="shape2"></param>
    /// <returns></returns>
    public bool CheckForMatch(GameShape shape1, GameShape shape2)
    {
        Debug.Log("Shape 1: " + shape1 + " Shape2: " + shape2);
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
    /// Triggers a destruction of shapes around the given index
    /// </summary>
    /// <param name="index"></param>
    public void TriggerShapeDestruction(int index)
    {
        // Getting the current slot
        GameSlot centerSlot = gameBoardParent.GetChild(index).GetComponent<GameSlot>();
        if (centerSlot == null)
        {
            Debug.LogError("Couldn't find slot when triggering destruction at slot " + index);
            return;
        }

        // Declaring temp storage variable
        bool destoryCurrentSlot = false;
        GameSlot slot;
        
        // Top left slot
        if (index % boardWidth != 0 && index - (boardWidth + 1) >= 0 && GetGameBoardslot(index - (boardWidth + 1)) != null)
        {
            // Getting Slot Reference
            slot = GetGameBoardslot(index - (boardWidth + 1));

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null && CheckForMatch(centerSlot.GetSlotShape(), slot.GetSlotShape()))
            {
                slot.DestroyShape();
                destoryCurrentSlot = true;
            }
        }

        // Top Center Slot
        if (index >= boardWidth && index - (boardWidth + 0) >= 0 && GetGameBoardslot(index - (boardWidth + 0)) != null)
        {
            // Getting Slot Reference
            slot = GetGameBoardslot(index - (boardWidth + 0));

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null && CheckForMatch(centerSlot.GetSlotShape(), slot.GetSlotShape()))
            {
                slot.DestroyShape();
                destoryCurrentSlot = true;
            }
        }

        // Top Right Slot
        if ((index + 1) % boardWidth != 0 && index - (boardWidth - 1) >= 0 && GetGameBoardslot(index - (boardWidth - 1)) != null)
        {
            // Getting Slot Reference
            slot = GetGameBoardslot(index - (boardWidth - 1));

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null && CheckForMatch(centerSlot.GetSlotShape(), slot.GetSlotShape()))
            {
                slot.DestroyShape();
                destoryCurrentSlot = true;
            }
        }

        // Middle Left Slot
        if (index % boardWidth != 0 && index - 1 >= 0 && GetGameBoardslot(index - 1) != null)
        {
            // Getting Slot Reference
            slot = GetGameBoardslot(index - 1);

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null && CheckForMatch(centerSlot.GetSlotShape(), slot.GetSlotShape()))
            {
                slot.DestroyShape();
                destoryCurrentSlot = true;
            }
        }
        
        // Middle Right Slot
        if ((index + 1) % boardWidth != 0 && index + 1 >= 0 && GetGameBoardslot(index + 1) != null)
        {
            // Getting Slot Reference
            slot = GetGameBoardslot(index + 1);

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null && CheckForMatch(centerSlot.GetSlotShape(), slot.GetSlotShape()))
            {
                slot.DestroyShape();
                destoryCurrentSlot = true;
            }
        }

        // Bottom left slot
        if (index % boardWidth != 0 && index + (boardWidth - 1) < boardSize && GetGameBoardslot(index + (boardWidth - 1)) != null)
        {
            // Getting Slot Reference
            slot = GetGameBoardslot(index + (boardWidth - 1));

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null && CheckForMatch(centerSlot.GetSlotShape(), slot.GetSlotShape()))
            {
                slot.DestroyShape();
                destoryCurrentSlot = true;
            }
        }

        // Bottom Center Slot
        if (index <= (boardSize - boardWidth) && index + boardWidth < boardSize && GetGameBoardslot(index + boardWidth) != null)
        {
            // Getting Slot Reference
            slot = GetGameBoardslot(index + boardWidth);

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null && CheckForMatch(centerSlot.GetSlotShape(), slot.GetSlotShape()))
            {
                slot.DestroyShape();
                destoryCurrentSlot = true;
            }
        }

        // Bottom Right Slot
        if ((index + 1) % boardWidth != 0 && index + (boardWidth + 1) < boardSize && GetGameBoardslot(index + (boardWidth + 1)) != null)
        {
            // Getting Slot Reference
            slot = GetGameBoardslot(index + (boardWidth + 1));

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null && CheckForMatch(centerSlot.GetSlotShape(), slot.GetSlotShape()))
            {
                slot.DestroyShape();
                destoryCurrentSlot = true;
            }
        }

        // Center Slot
        if (destoryCurrentSlot)
            GameObject.Destroy(centerSlot.GetSlotShape().gameObject);
    }
    #endregion

    #region State Check Functions
    /// <summary>
    /// Triggered after switching two shapes
    /// Compares the gameboard to the solution board
    /// Triggers CompleteLevel if evaluates as true
    /// </summary>
    public void CheckForVictory()
    {
        Debug.Log("Checking for Victory");

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
                return;
            }
            else if (shape1 != null && shape2 != null)
                if (!slot1.GetSlotShape().Equals(slot2.GetSlotShape()))
                {
                    Debug.Log("Victory failed at index " + i.ToString() + "\nResults were...\n" + "Slot1: " + slot1.GetSlotShape() + "\nSlot2: " + slot2.GetSlotShape());
                    return;
                }
        }

        // All the slots evaluate as true, trigger Complete Level
        CompleteLevel();
    }

    /// <summary>
    /// Triggers end of level animation and menu
    /// </summary>
    public void CompleteLevel() { DisplayEndLevelMenu(); }
    #endregion

    #region GUI Functions
    /// <summary>
    /// Displays the Pause menu and sets the game state to paused
    /// </summary>
    public void PauseGame()
    {
        // Displaying the the pause menu
        DisplayPauseMenu();

        // Setting the game state to paused
        GameState.gamePaused = true;
    }

    /// <summary>
    /// Unpauses the game state and hides the pause menu
    /// </summary>
    public void ResumeGame()
    {
        // Hide the pause menu
        HidePauseMenu();

        // Setting the game state to unpaused
        GameState.gamePaused = false;
    }

    /// <summary>
    /// Displaying the pause menu
    /// </summary>
    public void DisplayPauseMenu() { pauseMenuParent.SetActive(true); }

    /// <summary>
    /// Hides the pause menu
    /// </summary>
    public void HidePauseMenu() { pauseMenuParent.SetActive(false); }

    /// <summary>
    /// Displays the end level menu, also pauses the game to restrict user control
    /// </summary>
    public void DisplayEndLevelMenu() { endLevelMenuParent.SetActive(true); GameState.gamePaused = true; }
    #endregion

    #region Scene Navigation Functions
    /// <summary>
    /// Invokes navigation to the main menu
    /// </summary>
    public void ExitToMainMenu() { GameState.gamePaused = false; SceneManager.LoadScene(mainMenuScene); }

    /// <summary>
    /// Navigates to the declared nextLevel
    /// </summary>
    public void NavigateToNextLevel() { SceneManager.LoadScene(nextLevelScene); }
    #endregion
}
