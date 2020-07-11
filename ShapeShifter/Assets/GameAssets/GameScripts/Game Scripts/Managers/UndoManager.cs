using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UndoManager : MonoBehaviour
{
    public static UndoManager undoManager;

    [Header("Undo Stack Reference")]
    [SerializeField] Stack<BoardData> undoStack = null;

    [Header("Object References")]
    [SerializeField] private LevelConstructor levelConstructor = null;

    [Header("GUI References")]
    [SerializeField] private Button undoButton = null;
    [SerializeField] private DynamicGeneralThemeElement undoThemeElement = null;

    private void Awake()
    {
        undoManager = this;
        DisableUndoButton();
    }

    private void DisableUndoButton()
    {
        undoButton.interactable = false;
        undoThemeElement.SetElementToHighlighted();
    }

    private void EnableUndoButton()
    {
        undoButton.interactable = true;
        undoThemeElement.SetElementToNormal();
    }

    public void ProcessGameBoard(Transform gameBoardParent, Vector2Int boardSize) { PushBoardData(new BoardData(gameBoardParent, boardSize)); }
    public void PushBoardData(BoardData boardData)
    {
        if (undoStack == null)
            undoStack = new Stack<BoardData>();

        undoStack.Push(boardData);
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

    public void ClearUndoStack() { undoStack?.Clear(); DisableUndoButton(); }

    public void RestoreBoardState(BoardData data) { levelConstructor.ConstructGameBoard(data); }
}

[System.Serializable]
public class BoardData
{
    public Vector2Int boardSize;

    public List<ShapeData> shapes;
    public List<LockData> locks;
    public List<TransformerData> transformers;

    public BoardData(Transform gameBoardParent, Vector2Int boardSize)
    {
        this.boardSize = boardSize;

        shapes = new List<ShapeData>();
        locks = new List<LockData>();
        transformers = new List<TransformerData>();

        // Looping through the game board, get all the shapes and store them in the board list
        GameSlot slot;
        GameShape shape;
        SlotLock slotLock;
        ShapeTransformer transformer;

        for (int i = 0; i < gameBoardParent.childCount; i++)
        {
            slot = gameBoardParent.GetChild(i).GetComponent<GameSlot>();
            if (slot != null)
            {
                transformers.Add(new TransformerData(ShapeTransformer.TransformerType.None, 0));
                shape = slot.GetSlotShape();
                slotLock = slot.GetSlotLock();

                ShapeData newShape = shape != null ? new ShapeData(shape.GetShapeColor(), shape.GetShapeType()) : new ShapeData(GameShape.ColorType.None, GameShape.ShapeType.None);
                shapes.Add(newShape);

                LockData newLock = slotLock != null ? new LockData(slotLock.GetLockType(), slotLock.GetLockCounter()) : new LockData(SlotLock.LockType.None, 0);
                locks.Add(newLock);
            } else
            {
                shapes.Add(null);
                locks.Add(null);

                transformer = gameBoardParent.GetChild(i).GetComponent<ShapeTransformer>();
                TransformerData transformerData = transformer?.GetTransformerData();
                if (transformerData != null)
                    transformers.Add(new TransformerData(transformerData));
                else
                    transformers.Add(new TransformerData(ShapeTransformer.TransformerType.None, 0));
            }
        }
    }
}