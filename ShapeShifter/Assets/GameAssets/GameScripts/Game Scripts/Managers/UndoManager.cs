using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UndoManager : MonoBehaviour
{
    [Header("Undo Stack Reference")]
    [SerializeField] Stack<BoardData> undoStack = null;

    [Header("Object References")]
    [SerializeField] private BoardManager boardManager = null;

    [Header("GUI References")]
    [SerializeField] private Button undoButton = null;
    [SerializeField] private Image undoImage = null;
    private Color undoImageColor;
    private Color disabledUndoImageColor => new Color(undoImageColor.r, undoImageColor.g, undoImageColor.b, undoImageColor.a * 0.4f);

    private void Awake()
    {
        undoImageColor = undoImage.color;
        DisableUndoButton();
    }

    private void DisableUndoButton()
    {
        undoButton.interactable = false;
        undoImage.color = disabledUndoImageColor;
    }

    private void EnableUndoButton()
    {
        undoButton.interactable = true;
        undoImage.color = undoImageColor;
    }

    public void ProcessGameBoard(Transform gameBoardParent)
    {
        if (undoStack == null)
            undoStack = new Stack<BoardData>();

        undoStack.Push(new BoardData(gameBoardParent));
        EnableUndoButton();
    }

    public BoardData GetPreviousBoardState()
    {
        if (undoStack.Count == 1)
            DisableUndoButton();

        return (undoStack.Count > 0) ? undoStack.Pop() : null;
    }

    public void UndoMove()
    {
        BoardData data = GetPreviousBoardState();
        if (data != null)
            RestoreBoardState(data);
    }

    public void RestoreBoardState(BoardData data)
    {
        // Getting board parents
        GameObject shapePrefab = GameManager.manager.shapePrefab;
        Transform gameBoardParent = GameManager.manager.gameBoardParent;
        float shapeSize = boardManager.shapeSize;

        // Declaring Tracker Variables
        GameSlot slot;
        GameShape shape;
        SlotLock slotLock;

        // Loop through the board and set the board state to the data
        for (int i = 0; i < gameBoardParent.childCount; i++)
        {
            // Getting the Game Slot/Shape Reference
            slot = gameBoardParent.GetChild(i).GetComponent<GameSlot>();
            shape = slot.GetSlotShape();
            slotLock = slot.GetSlotLock();

            #region Generating Slot Shape
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

                    slot.SetSlotShapeReference(shapeRef.transform);
                }
            }
            #endregion

            #region Generating Slot Lock
            if (slotLock != null)
            {
                LockData lockData = data.locks[i];
                if (lockData != null)
                {
                    Debug.Log($"Generating Slot Lock at Index {i} of type {lockData.lockType} with {lockData.lockTimer} move(s) left");

                    // Active
                    if (lockData.lockTimer > 0)
                    {
                        if (slotLock.GetLockCounter() <= 0)
                            slotLock.SetLockToActive();

                        slotLock.SetLockSettings(lockData.lockType, lockData.lockTimer);
                        slot.LockGameSlot();
                    }
                    else if (slotLock.GetLockCounter() > 0)
                        slotLock.SetLockToDestruct();
                }
            }
            #endregion
        }
    }
}

public class BoardData
{
    public List<ShapeData> board;
    public List<LockData> locks;

    public BoardData(Transform gameBoardParent)
    {
        // Creating a new list
        board = new List<ShapeData>();
        locks = new List<LockData>();

        // Looping through the game board, get all the shapes and store them in the board list
        GameSlot slot;
        GameShape shape;
        SlotLock slotLock;
        for (int i = 0; i < gameBoardParent.childCount; i++)
        {
            slot = gameBoardParent.GetChild(i).GetComponent<GameSlot>();
            shape = slot.GetSlotShape();
            slotLock = slot.GetSlotLock();

            ShapeData newShape = shape != null ? new ShapeData(shape.GetShapeColor(), shape.GetShapeType()) : null;
            board.Add(newShape);
                
            LockData newLock = slotLock != null ? new LockData(slotLock.GetLockType(), slotLock.GetLockCounter()) : null;
            locks.Add(newLock);
        }
    }
}