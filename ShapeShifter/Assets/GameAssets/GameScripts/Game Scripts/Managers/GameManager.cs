using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Declaring a singleton
    public static GameManager manager = null;
    
    // Destroy Control Toggle
    public enum DestroyMethod { Shape, Color };

    [Header("Scene Navigation Variables")]
    [SerializeField] private string mainMenuScene = "";
    [SerializeField] private string nextLevelScene = "";
    private int currentPackIndex = 0;
    private int currentLevelIndex = 0;
    private string currentLevelName = "";

    [Header("Control Variables")]
    [SerializeField] private DestroyMethod currentDestoryMethod = DestroyMethod.Shape;
    public float levelTimer = 0f;

    private BoardData processedBoardData = null;
    private GameSlot slot1 = null, slot2 = null;
    private bool switching = false;

    [Header("Object References")]
    public BoardManager boardManager = null;
    public UndoManager undoManager = null;
    public Transform gameBoardParent = null;
    public Transform solutionBoardParent = null;

    [Header("Prefab References")]
    public GameObject shapePrefab = null;

    public delegate void OnClockTick();
    public event OnClockTick onClockTick;

    public delegate void OnUpdateBoard();
    public event OnUpdateBoard onUpdateBoard;

    public delegate void OnVictory();
    public event OnVictory onVictory;

    public delegate void OnModeSwitch();
    public event OnModeSwitch onModeSwitch;

    [Header("GUI References")]
    [SerializeField] private GameObject pauseMenuParent = null;
    [SerializeField] private GameObject victoryMenuParent = null;
    [SerializeField] private TextMeshProUGUI destroyText = null;
    [SerializeField] private TextMeshProUGUI gameTimerText = null;

    [Header("Defeat UI References")]
    [SerializeField] private DefeatInstructions defeatInstructions = null;
    private Dictionary<int, ShapeData> requiredShapes;

    #region Unity Functions
    private void OnEnable() { manager = this; }
    private void Awake()
    {
        currentLevelName = SceneManager.GetActiveScene().name.Split('_')[1];

        string[] levelNumberSplit = currentLevelName.Split('-');

        currentPackIndex = Int32.Parse(levelNumberSplit[0]);
        currentLevelIndex = Int32.Parse(levelNumberSplit[1]);
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
            shape = solutionBoardParent.GetChild(i).gameObject.GetComponent<GameSlot>()?.GetSlotShape();

            // Adding shape data if a shape exists
            if (shape != null)
                requiredShapes.Add(i, shape.GetShapeData());
        }

        // Setting the gameboard/solution board
        boardManager.SetGameSlotIndexes();
    }

    private void Update()
    {
        // Checking for paused state
        if (!GameState.gamePaused)
        {
            // Updating Game Timer
            levelTimer += Time.deltaTime;
            onClockTick?.Invoke();

            if (gameTimerText != null)
                gameTimerText.text = string.Format("Level {0} : {1}", currentLevelName, GameTime.GetGameTimeFormat(levelTimer));
        }
    }

    private void OnDestroy() { manager = null; }
    #endregion

    #region Getter Functions
    public int GetPackIndex() { return currentPackIndex; }
    public int GetLevelIndex() { return currentLevelIndex; } 
    public GameSlot GetSelectedSlot() { return slot1; }
    #endregion

    #region Setter Functions
    public void DeselectSlot() 
    {
        if (!switching && slot1 != null)
        {
            boardManager.DeselectGameSlots();
            slot1 = null;
        }
    }
    public bool SelectSlot(GameSlot targetSlot)
    {
        // Check for two shapes switching around
        if (!switching)
        {
            // Checking for which reference
            if (slot1 == null)
            {
                slot1 = targetSlot;
                boardManager.SelectGameSlots();

                return true;
            }
            else if (slot2 == null)
            {
                // Two Slots Selected, switch the shapes
                slot2 = targetSlot;

                boardManager.DeselectGameSlots();

                processedBoardData = new BoardData(gameBoardParent);
                //undoManager.ProcessGameBoard(gameBoardParent);
                StartCoroutine(boardManager.MoveShapes(slot1, slot2));
                return true;
            }
        }

        // Returning the default case
        return false;
    }

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

        onModeSwitch?.Invoke();
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

        onModeSwitch?.Invoke();
    }
    #endregion

    #region In Game Functions
    public void MarkSwitchingAsComplete()
    {
        undoManager.PushBoardData(processedBoardData);
        processedBoardData = null;

        switching = false;
        slot1 = null;
        slot2 = null;
    }

    public bool CheckForMatch(GameShape shape1, GameShape shape2)
    {
        switch (currentDestoryMethod)
        {
            case DestroyMethod.Shape:
                return (shape1.GetShapeType() == shape2.GetShapeType());
            case DestroyMethod.Color:
                return (shape1.GetShapeColor() == shape2.GetShapeColor());
            default:
                return false;
        }
    }
    #endregion

    #region State Check Functions
    public void CheckGameState()
    {
        if (!CheckForVictory(gameBoardParent, solutionBoardParent))
        {
            SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
            if (!saveDataAccessor.GetDataValue<bool>(SaveKeys.DEFEAT_TUTORIAL_COMPLETE))
                CheckForDefeat(gameBoardParent, solutionBoardParent);
        }

        onUpdateBoard?.Invoke();
    }

    public bool CheckForDefeat(Transform gameBoardParent, Transform solutionBoardParent)
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
                defeatInstructions.InvokeInstructions();
                return true;
            }
        }

        // All cases pass, return true
        return false;
    }

    public bool CheckForVictory(Transform gameBoardParent, Transform solutionBoardParent)
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
                return false;
            }
            else if (shape1 != null && shape2 != null)
                if (!slot1.GetSlotShape().Equals(slot2.GetSlotShape()))
                {
                    return false;
                }
        }

        // All the slots evaluate as true, trigger Complete Level
        CompleteLevel();
        return true;
    }

    public void CompleteLevel()
    {
        onVictory?.Invoke();
        DisplayVictoryScreen();

        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();

        #region Save Completed Level Index
        Dictionary<int, int> completedLevels = saveDataAccessor.GetDataValue<Dictionary<int, int>>(SaveKeys.COMPLETED_LEVELS_SAVE_KEY);

        if (completedLevels == null)
        {
            completedLevels = new Dictionary<int, int>();
            completedLevels.Add(currentPackIndex, currentLevelIndex);

            saveDataAccessor.SetData(SaveKeys.COMPLETED_LEVELS_SAVE_KEY, completedLevels);
            DataTracker.dataTracker.SaveData();
        }
        else if (!completedLevels.ContainsKey(currentPackIndex))
        {
            completedLevels.Add(currentPackIndex, currentLevelIndex);

            saveDataAccessor.SetData(SaveKeys.COMPLETED_LEVELS_SAVE_KEY, completedLevels);
            DataTracker.dataTracker.SaveData();
        }
        else
        {
            int highestCompletedLevel = completedLevels[currentPackIndex];
            if (currentLevelIndex > highestCompletedLevel)
            {
                completedLevels[currentPackIndex] = currentLevelIndex;
                saveDataAccessor.SetData(SaveKeys.COMPLETED_LEVELS_SAVE_KEY, completedLevels);
                DataTracker.dataTracker.SaveData();
            }
        }
        #endregion

        #region Save Level Time
        Dictionary<int, float> bestLevelTimes = saveDataAccessor.GetDataValue<Dictionary<int, float>>(SaveKeys.BEST_LEVEL_TIMES);
        int levelKey = GetLevelKey(currentPackIndex, currentLevelIndex);
        if (bestLevelTimes == null)
        {
            bestLevelTimes = new Dictionary<int, float>();
            bestLevelTimes.Add(levelKey, levelTimer);

            saveDataAccessor.SetData(SaveKeys.BEST_LEVEL_TIMES, bestLevelTimes);
            DataTracker.dataTracker.SaveData();
        } else if (!bestLevelTimes.ContainsKey(levelKey))
        {
            bestLevelTimes.Add(levelKey, levelTimer);

            saveDataAccessor.SetData(SaveKeys.BEST_LEVEL_TIMES, bestLevelTimes);
            DataTracker.dataTracker.SaveData();
        } else if (bestLevelTimes[levelKey] > levelTimer)
        {
            bestLevelTimes[levelKey] = levelTimer;
            saveDataAccessor.SetData(SaveKeys.BEST_LEVEL_TIMES, bestLevelTimes);
            DataTracker.dataTracker.SaveData();
        }
        #endregion
    }
    #endregion

    #region GUI Functions
    public void PauseGame() { GameState.gamePaused = true; }
    public void ResumeGame() { GameState.gamePaused = false; }

    public void DisplayPauseMenu() { pauseMenuParent.SetActive(true); }
    public void HidePauseMenu() { pauseMenuParent.SetActive(false); }

    public void HideVictoryScreen() { victoryMenuParent.SetActive(false); GameState.gamePaused = false; }
    public void DisplayVictoryScreen()
    {
        GameState.gamePaused = true;
        victoryMenuParent.SetActive(true);
    }
    #endregion

    #region Scene Navigation Functions
    public void RestartCurrentLevel() { GameState.gamePaused = false; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void ExitToMainMenu() { GameState.gamePaused = false; SceneManager.LoadScene(mainMenuScene); }
    public void NavigateToNextLevel() { AdManager.CheckForAutomaticAd(GameTime.GetMinuteCount(levelTimer), nextLevelScene); /*HideVictoryScreen();*/ GameState.gamePaused = false; }
    #endregion

    #region Helper Functions
    public static int GetLevelKey(int packIndex, int levelIndex)
    {
        return (packIndex * 10000) + levelIndex + 1;
    }
    #endregion
}
