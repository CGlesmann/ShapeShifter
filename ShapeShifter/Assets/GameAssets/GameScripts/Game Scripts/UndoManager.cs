using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    [Header("Undo Stack Reference")]
    [SerializeField] Stack<BoardData> undoStack = null;

    /// <summary>
    /// Takes in the game board parent and stores the current state in the undoStack
    /// </summary>
    public void ProcessGameBoard(Transform gameBoardParent)
    {
        if (undoStack == null)
            undoStack = new Stack<BoardData>();

        undoStack.Push(new BoardData(gameBoardParent));
    }

    /// <summary>
    /// Pops off the most recent BoardData reference
    /// </summary>
    /// <returns></returns>
    public BoardData GetPreviousBoardState()
    {
        return (undoStack.Count > 0) ? undoStack.Pop() : null;
    }

    /// <summary>
    /// Gets the previous board state and uses the GameManager to restore the state
    /// </summary>
    public void UndoMove()
    {
        BoardData data = GetPreviousBoardState();
        if (data != null)
            GameManager.manager.RestoreBoardState(data);
    }
}

public class BoardData
{
    public List<ShapeData> board;

    public BoardData(Transform gameBoardParent)
    {
        // Creating a new list
        board = new List<ShapeData>();

        // Looping through the game board, get all the shapes and store them in the board list
        GameShape shape;
        for(int i = 0; i < gameBoardParent.childCount; i++)
        {
            shape = gameBoardParent.GetChild(i).GetComponent<GameSlot>().GetSlotShape();

            ShapeData newShape = shape != null ? new ShapeData(shape.GetShapeColor(), shape.GetShapeType()) : null;
            board.Add(newShape);
        }
    }
}