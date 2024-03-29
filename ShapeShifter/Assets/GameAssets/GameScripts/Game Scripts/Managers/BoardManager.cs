﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager boardManager = null;

    [Header("Shape Settings")]
    public float shapeSize = 1f;

    [Header("Board Settings")]
    [SerializeField] private int boardWidth = 0;
    [SerializeField] private int boardHeight = 0;
    private int shapesBeingDestroyed = 0;
    private int boardSize => boardWidth * boardHeight;

    [Header("Board References")]
    [SerializeField] private Transform gameBoardParent = null;
    [SerializeField] private Transform solutionBoardParent = null;

    [Header("Object References")]
    [SerializeField] private GameManager gameManager = null;

    public delegate void OnShapeDestroy(int count);
    public event OnShapeDestroy onShapeDestroy;

    public delegate void OnShapeSwap();
    public event OnShapeSwap onShapeSwap;

    public delegate void OnToggleGameSlots();
    public event OnToggleGameSlots onSelectGameSlots;
    public event OnToggleGameSlots onDeselectGameSlots;

    private int locksAnimating = 0;

    public void OnEnable() { boardManager = this; }

    public void SelectGameSlots() { onSelectGameSlots?.Invoke(); }
    public void DeselectGameSlots() { onDeselectGameSlots?.Invoke(); }

    public void SetBoardSize(int width, int height) { boardWidth = width; boardHeight = height; }
    public int GetBoardSize() { return boardWidth * boardHeight; }
    public int GetBoardWidth() { return boardWidth; }
    public int GetBoardHeight() { return boardHeight; }
    public int GetShapesBeingDestroyed() { return shapesBeingDestroyed; }
    public void MarkShapeAsDestroyed() { shapesBeingDestroyed--; }

    public GameSlot GetGameSlot(int slotIndex, Transform boardParent) { return boardParent.GetChild(slotIndex).GetComponent<GameSlot>(); }
    public ShapeData GetBoardShapeData(int slotIndex, Transform boardParent)
    {
        GameSlot slot = GetGameSlot(slotIndex, boardParent);

        if (slot.CheckCanInteract())
        {
            GameShape shape = slot.GetSlotShape();
            if (shape != null)
                return shape.GetShapeData() ?? null;
            else
                return null;
        }
        else
            return null;
    }

    public IEnumerator MoveShapes(GameSlot slot1, GameSlot slot2)
    {
        // Declaring temporary storeage variables
        Transform shape1P = slot1.GetSlotShapeTransform(), shape2P = slot2.GetSlotShapeTransform();

        Vector3 shape1StartPosition = shape1P.position, shape2StartPosition = shape2P.position;
        float mult = 1f / (10f / 24f);
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

        StartCoroutine(SwitchShapes(slot1, slot2));
    }

    public IEnumerator SwitchShapes(GameSlot slot1, GameSlot slot2)
    {
        // Setting the parents
        Transform shape1 = slot1.GetSlotShapeTransform();
        Transform shape2 = slot2.GetSlotShapeTransform();

        shape1.SetParent(slot2.transform);
        shape2.SetParent(slot1.transform);

        slot1.SetSlotShapeReference(shape2);
        slot2.SetSlotShapeReference(shape1);

        onShapeSwap?.Invoke();
        while (locksAnimating > 0)
            yield return null;

        // Triggering Shape Destruction(s)
        int destroyedShapes = 0;
        destroyedShapes += TriggerShapeDestruction(slot1.GetSlotIndex(), gameBoardParent);
        destroyedShapes += TriggerShapeDestruction(slot2.GetSlotIndex(), gameBoardParent);

        while (shapesBeingDestroyed > 0)
            yield return null;

        onShapeDestroy?.Invoke(destroyedShapes);
        while (locksAnimating > 0)
            yield return null;

        // Reset each slot
        slot1.ResetSlotState();
        slot2.ResetSlotState();

        if (shapesBeingDestroyed == 0)
            gameManager.CheckGameState();

        gameManager.MarkSwitchingAsComplete();
    }

    public Action MarkLockAsAnimating()
    {
        locksAnimating++;
        return MarkLockAnimationFinished;
    }
    public void MarkLockAnimationFinished() { locksAnimating--; }

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
            // slot.GetSlotShape()
            if (GetBoardShapeData(i, boardParent) != null)
                indexes.Add(i);
        }

        // Top Center Slot
        i = (index - (boardWidth + 0));
        if (index >= boardWidth && i >= 0 && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (GetBoardShapeData(i, boardParent) != null)
                indexes.Add(i);
        }

        // Top Right Slot
        i = index - (boardWidth - 1);
        if ((index + 1) % boardWidth != 0 && i >= 0 && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (GetBoardShapeData(i, boardParent) != null)
                indexes.Add(i);
        }

        // Middle Left Slot
        i = index - 1;
        if (index % boardWidth != 0 && i >= 0 && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (GetBoardShapeData(i, boardParent) != null)
                indexes.Add(i);
        }

        // Middle Right Slot
        i = index + 1;
        if ((index + 1) % boardWidth != 0 && i >= 0 && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (GetBoardShapeData(i, boardParent) != null)
                indexes.Add(i);
        }

        // Bottom left slot
        i = index + (boardWidth - 1);
        if (index % boardWidth != 0 && i < boardSize && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (GetBoardShapeData(i, boardParent) != null)
                indexes.Add(i);
        }

        // Bottom Center Slot
        i = index + boardWidth;
        if (index <= (boardSize - boardWidth) && i < boardSize && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (GetBoardShapeData(i, boardParent) != null)
                indexes.Add(i);
        }

        // Bottom Right Slot
        i = index + (boardWidth + 1);
        if ((index + 1) % boardWidth != 0 && i < boardSize && GetGameSlot(i, boardParent) != null)
        {
            // Getting Slot Reference
            slot = GetGameSlot(i, boardParent);

            // Checking for destorying the shape
            if (GetBoardShapeData(i, boardParent) != null)
                indexes.Add(i);
        }

        return indexes;
    }

    public int TriggerShapeDestruction(int index, Transform boardParent)
    {
        // Getting the current slot
        GameShape centerShape = boardParent.GetChild(index).GetComponent<GameSlot>().GetSlotShape();
        if (centerShape == null)
        {
            Debug.LogError("Couldn't find shape when triggering destruction at slot " + index);
            return -1;
        }

        // Declaring temp storage variable
        List<int> targetIndexes = CheckForSurroundingShapes(index, boardParent);
        GameSlot slot;
        GameShape shape = null;
        bool destoryCurrentSlot = false;
        int newShapesBeingDestroyed = 0;

        if (targetIndexes != null)
        {
            foreach (int i in targetIndexes)
            {
                slot = boardParent.GetChild(i).GetComponent<GameSlot>();
                shape = slot.GetSlotShape();

                if (gameManager.CheckForMatch(centerShape, shape))
                {
                    if (!shape.IsMarkedForDestruct())
                    {
                        shape.TriggerDestruction();
                        shape.MarkForDestruction();
                        newShapesBeingDestroyed++;
                        shapesBeingDestroyed++;
                    }

                    destoryCurrentSlot = true;
                }
            }
        }

        if (destoryCurrentSlot)
        {
            if (!centerShape.IsMarkedForDestruct())
            {
                centerShape.TriggerDestruction();
                centerShape.MarkForDestruction();
                newShapesBeingDestroyed++;
                shapesBeingDestroyed++;
            }
        }

        //onShapeDestroy?.Invoke(newShapesBeingDestroyed);
        return newShapesBeingDestroyed;
    }
}
