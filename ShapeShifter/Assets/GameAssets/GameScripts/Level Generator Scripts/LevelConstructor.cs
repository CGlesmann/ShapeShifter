using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelConstructor : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private BoardManager boardManager = null;
    [SerializeField] private Transform gameBoardParent = null;
    [SerializeField] private Transform solutionBoardParent = null;

    [SerializeField] private GridLayoutGroup gameBoardLayoutGroup = null;
    [SerializeField] private GridLayoutGroup solutionBoardLayoutGroup = null;

    [Header("Prefeb References")]
    [SerializeField] private GameObject shapePrefab = null;
    [SerializeField] private GameObject slotLockPrefab = null;
    [SerializeField] private GameObject gameBoardTransformerPrefab = null;
    [SerializeField] private GameObject solutionBoardTransformerPrefab = null;
    [SerializeField] private GameObject gameBoardSlotPrefab = null;
    [SerializeField] private GameObject solutionBoardSlotPrefab = null;

    private void OnEnable()
    {
        LevelData levelData = LevelLoader.GetLevelToLoad();
        if (levelData == null)
        {
            Debug.LogError("No LevelData is present to load");
            return;
        }

        ConstructLevel(levelData);
    }

    public void ConstructLevel(LevelData levelData)
    {
        GenerateLevelSlots(levelData, levelData.gameBoardData.boardSize, levelData.solutionBoardData.boardSize);
        ConstructGameBoard(levelData.GetGameBoardData());
        ConstructSolutionBoard(levelData.GetSolutionBoardData());

        DisplayEntranceAnimation();
    }

    public void GenerateLevelSlots(LevelData levelData, Vector2Int gameBoardSize, Vector2Int solutionBoardSize)
    {
        GenerateBoardSlots(levelData.gameBoardData, gameBoardLayoutGroup, gameBoardSlotPrefab, gameBoardTransformerPrefab, gameBoardParent, gameBoardSize.x, gameBoardSize.y); //Gameboard Slots
        GenerateBoardSlots(levelData.solutionBoardData, solutionBoardLayoutGroup, solutionBoardSlotPrefab, solutionBoardTransformerPrefab, solutionBoardParent, solutionBoardSize.x, solutionBoardSize.y); //Solutionboard Slots
    }

    private void GenerateBoardSlots(BoardData boardData, GridLayoutGroup group, GameObject slotPrefab, GameObject transformerPrefab, Transform boardParent, int width, int height)
    {
        RectTransform boardTransform = boardParent.GetComponent<RectTransform>();
        float baseBoardSize = Mathf.Min(boardTransform.rect.width, boardTransform.rect.height);
        float cellGap = 64f * (3f / (float)width);

        float cellWidth = (baseBoardSize - ((width - 1) * cellGap)) / width;
        float cellHeight = (baseBoardSize - ((height - 1) * cellGap)) / height;

        group.cellSize = new Vector2(cellWidth, cellHeight);
        group.spacing = new Vector2(cellGap, cellGap);

        for (int i = 0; i < width * height; i++)
        {
            if (boardData.transformers[i] == null || boardData.transformers[i].transformerType == ShapeTransformer.TransformerType.None)
            {
                GameSlot slot = Instantiate(slotPrefab, boardParent).GetComponent<GameSlot>();
                if (Application.isPlaying)
                    slot.transform.localScale = Vector3.zero;
                slot.SetSlotIndex(i);
            }
            else
            {
                ShapeTransformer newTransformer = Instantiate(transformerPrefab, boardParent).GetComponent<ShapeTransformer>();
                newTransformer.SetTransformerData(boardData.transformers[i]);
            }
        }

        boardManager.SetBoardSize(width, height);
    }

    public void ConstructGameBoard(BoardData gameBoardData) { ConstructBoard(gameBoardData, gameBoardParent); }
    public void ConstructSolutionBoard(BoardData solutionBoardData) { ConstructBoard(solutionBoardData, solutionBoardParent); }
    private void ConstructBoard(BoardData data, Transform boardParent)
    {
        // Getting board parents
        //GameObject shapePrefab = GameManager.manager.shapePrefab;
        float cellSize = boardParent.GetComponent<GridLayoutGroup>().cellSize.y;
        float shapeSize = 2.25f * (cellSize / 384);
        int boardCount = data.boardSize.x * data.boardSize.y;
        boardManager.shapeSize = shapeSize;

        // Declaring Tracker Variables
        GameSlot slot;
        GameShape shape;
        SlotLock slotLock;
        ShapeTransformer shapeTransformer;

        // Loop through the board and set the board state to the data
        for (int i = 0; i < boardCount; i++)
        {
            // Getting the Game Slot/Shape Reference
            slot = boardParent.GetChild(i).GetComponent<GameSlot>();
            shape = slot?.GetSlotShape();
            slotLock = slot?.GetSlotLock();
            shapeTransformer = boardParent.GetChild(i).GetComponent<ShapeTransformer>();

            #region Generating Slot Shape
            if (shape != null)
            {
                // Checking if a shape doesn't exist on the passed in data
                if (data.shapes[i] == null || data.shapes[i].shapeType == GameShape.ShapeType.None)
                    Destroy(shape.gameObject);
                else if (shape.GetShapeData() != data.shapes[i])
                {
                    // Shape does exist but doesn't match the shape in the data, set them to be equivalent
                    shape.SetShapeColor(data.shapes[i].shapeColor);
                    shape.SetShapeType(data.shapes[i].shapeType);
                }
            }
            else
            {
                // Checking to see if a shape exists in the data at the given spot, if so create a new one
                if (data.shapes[i] != null && data.shapes[i].shapeType != GameShape.ShapeType.None)
                {
                    // Creating the shape
                    GameObject newShape = Instantiate(shapePrefab, slot.transform);
                    newShape.transform.localPosition = new Vector3(0.5f, -0.5f, 0f);
                    newShape.transform.localScale = new Vector3(shapeSize, shapeSize, 0f);

                    // Getting the GameShape reference and setting it equal to the shape in the data
                    GameShape shapeRef = newShape.GetComponent<GameShape>();
                    shapeRef.SetShapeColor(data.shapes[i].shapeColor);
                    shapeRef.SetShapeType(data.shapes[i].shapeType);

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
            } else
            {
                LockData lockData = data.locks[i];
                if (lockData != null && lockData.lockType != SlotLock.LockType.None)
                {
                    GameObject newLock = Instantiate(slotLockPrefab, slot.transform);
                    newLock.transform.localPosition = new Vector3(0f, -cellSize / 2f, 0f);
                    newLock.transform.localScale = new Vector3(cellSize / 384f, cellSize / 384f);

                    slotLock = newLock.GetComponent<SlotLock>();
                    slotLock.SetLockSettings(lockData.lockType, lockData.lockTimer);

                    slot.GetComponent<GameSlot>().SetSlotLock(slotLock);
                }
            }
            #endregion

            #region Generate Shape Transformers
            if (shapeTransformer != null)
                shapeTransformer.SetTransformerData(data.transformers[i]);
            #endregion
        }
    }

    #region Animation Methods
    public void DisplayEntranceAnimation()
    {
        Queue<Transform> boards = new Queue<Transform>();
        boards.Enqueue(gameBoardParent);
        boards.Enqueue(solutionBoardParent);

        StartCoroutine(PlayEntranceAnimations(boards));
    }

    private IEnumerator PlayEntranceAnimations(Queue<Transform> boards)
    {
        yield return new WaitForSeconds(0.3f);

        Transform currentBoard;
        GameSlot currentSlot;
        ShapeTransformer currentTransformer;

        while (boards.Count > 0)
        {
            currentBoard = boards.Dequeue();
            for (int i = 0; i < currentBoard.childCount; i++)
            {
                currentSlot = currentBoard.GetChild(i).GetComponent<GameSlot>();
                if (currentSlot != null)
                    currentSlot.DisplayEnterAnimation();
                else
                {
                    currentTransformer = currentBoard.GetChild(i).GetComponent<ShapeTransformer>();
                    if (currentTransformer != null)
                        currentTransformer.DisplayEnterAnimation();
                }

                yield return null;
            }
        }
    }

    public void DisplayDestroyAnimation()
    {
        int gameboardCount = gameBoardParent.childCount;
        for (int i = 0; i < gameboardCount; i++)
            GameObject.DestroyImmediate(gameBoardParent.GetChild(0).gameObject);

        int solutionboardCount = solutionBoardParent.childCount;
        for (int i = 0; i < solutionboardCount; i++)
            GameObject.DestroyImmediate(solutionBoardParent.GetChild(0).gameObject);
    }
    #endregion

    public void DestroyLevel()
    {
        int gameBoardCount = gameBoardParent.childCount;
        for (int i = 0; i < gameBoardCount; i++)
            GameObject.DestroyImmediate(gameBoardParent.GetChild(0).gameObject);

        int solutionBoardCount = solutionBoardParent.childCount;
        for (int j = 0; j < solutionBoardCount; j++)
            GameObject.DestroyImmediate(solutionBoardParent.GetChild(0).gameObject);
    }
}

[System.Serializable]
public class LevelData
{
    public BoardData gameBoardData = null;
    public BoardData solutionBoardData = null;

    public LevelData(BoardData gameBoardData, BoardData solutionBoardData)
    {
        this.gameBoardData = gameBoardData;
        this.solutionBoardData = solutionBoardData;
    }

    public BoardData GetGameBoardData() { return gameBoardData; }
    public BoardData GetSolutionBoardData() { return solutionBoardData; }
}