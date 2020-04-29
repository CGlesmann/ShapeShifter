﻿using System;
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
    [SerializeField] private string mainMenuScene = "";
    [SerializeField] private string nextLevelScene = "";
    private int currentPackIndex = 0;
    private int currentLevelIndex = 0;
    private string currentLevelName = "";

    [Header("Control Variables")]
    [SerializeField] private DestroyMethod currentDestoryMethod = DestroyMethod.Shape;
    public float levelTimer = 0f;

    private GameSlot slot1 = null, slot2 = null;
    private bool switching = false;

    [Header("Object References")]
    public BoardManager boardManager = null;
    public ChallengeManager challengeManager = null;
    public UndoManager undoManager = null;
    public Transform gameBoardParent = null;
    public Transform solutionBoardParent = null;

    [Header("Prefab References")]
    public GameObject shapePrefab = null;

    public delegate void OnClockTick();
    public static event OnClockTick onClockTick;

    public delegate void OnUpdateBoard();
    public static event OnUpdateBoard onUpdateBoard;

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
            shape = solutionBoardParent.GetChild(i).gameObject.GetComponent<GameSlot>().GetSlotShape();

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

    #region Setter Functions
    public void DeselectSlot() { if (!switching) slot1 = null; }
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
                undoManager.ProcessGameBoard(gameBoardParent);
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
    public void MarkSwitchingAsComplete()
    {
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
        if (!CheckForVictory())
            if (!DataTracker.gameData.defeatTutorialComplete)
                CheckForDefeat();

        onUpdateBoard?.Invoke();
    }

    public bool CheckForDefeat()
    {
        Debug.Log("Checking for Defeat");

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
        CompleteLevel();
        return true;
    }

    public void CompleteLevel()
    {
        DisplayVictoryScreen();

        if (DataTracker.gameData.completedLevels.ContainsKey(currentPackIndex))
        {
            if (currentLevelIndex > DataTracker.gameData.completedLevels[currentPackIndex])
                DataTracker.gameData.completedLevels[currentPackIndex] = currentLevelIndex;
        } else {
            DataTracker.gameData.completedLevels.Add(currentPackIndex, currentLevelIndex);
        }

        DataTracker.dataTracker.SaveData();
    }
    #endregion

    #region GUI Functions
    public void PauseGame() { GameState.gamePaused = true; }
    public void ResumeGame() { GameState.gamePaused = false; }

    public void DisplayPauseMenu() { pauseMenuParent.SetActive(true); }
    public void HidePauseMenu() { pauseMenuParent.SetActive(false); }

    public void DisplayVictoryScreen() { GameState.gamePaused = true; victoryMenuParent.SetActive(true); challengeManager.UpdateChallengeEntries(); }
    public void HideVictoryScreen() { victoryMenuParent.SetActive(false); GameState.gamePaused = false; }
    #endregion

    #region Scene Navigation Functions
    public void RestartCurrentLevel() { GameState.gamePaused = false; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void ExitToMainMenu() { GameState.gamePaused = false; SceneManager.LoadScene(mainMenuScene); }
    public void NavigateToNextLevel() { AdManager.CheckForAutomaticAd(GameTime.GetMinuteCount(levelTimer), nextLevelScene); /*HideVictoryScreen();*/ GameState.gamePaused = false; }
    #endregion
}
