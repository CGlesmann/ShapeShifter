using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class GameboardTools
{
    private static GameManager gameManager = null;
    private static List<GameObject> slotLocks = null;

    private static bool locksToggled = true;

    private static void GetGameManager() { gameManager = GameObject.Find("LevelManager").GetComponent<GameManager>(); }

    private static void ClearBoard(Transform boardParent, bool deleteShapes, bool deleteLocks)
    {
        if (boardParent != null)
        {
            Transform shape;
            GameSlot gameSlot;
            SlotLock slotLock;
            for (int i = 0; i < boardParent.childCount; i++)
            {
                gameSlot = boardParent.GetChild(i).GetComponent<GameSlot>();

                if (gameSlot != null)
                {
                    if (deleteShapes)
                    {
                        shape = gameSlot.GetSlotShapeTransform();
                        if (shape != null)
                            GameObject.DestroyImmediate(shape.gameObject);
                    }

                    if (deleteLocks)
                    {
                        slotLock = gameSlot.GetSlotLock();
                        if (slotLock != null)
                            GameObject.DestroyImmediate(slotLock.gameObject);
                    }
                }
            }
        }
    }

    public static void ClearGameboard(bool deleteShapes, bool deleteLocks)
    {
        if (gameManager == null)
            GetGameManager();

        Transform gameBoardParent = gameManager.gameBoardParent;
        ClearBoard(gameBoardParent, deleteShapes, deleteLocks);
    }

    public static void ClearSolutionboard(bool deleteShapes, bool deleteLocks)
    {
        if (gameManager == null)
            GetGameManager();

        Transform solutionBoardParent = gameManager.solutionBoardParent;
        ClearBoard(solutionBoardParent, deleteShapes, deleteLocks);
    }

    public static void SetAllGameSlotReferences(Transform boardParent)
    {
        GameSlot slot;
        for(int i = 0; i < boardParent.childCount; i++)
        {
            slot = boardParent.GetChild(i).GetComponent<GameSlot>();
            for(int j = 0; j < slot.transform.childCount; j++)
            {
                Transform currentSlotChild = slot.transform.GetChild(j);

                GameShape gameShape = currentSlotChild.GetComponent<GameShape>();
                if (gameShape != null)
                {
                    slot.SetSlotShapeReference(currentSlotChild);
                    EditorUtility.SetDirty(slot);
                    continue;
                }

                SlotLock slotLock = currentSlotChild.GetComponent<SlotLock>();
                if (slotLock != null)
                {
                    slot.SetSlotLock(slotLock);
                    EditorUtility.SetDirty(slot);
                    continue;
                }
            }
        }
    }

    #region Lock Manipulation
    public static void ToggleLocks()
    {
        locksToggled = !locksToggled;
        if (locksToggled)
            EnableLocks();
        else
            DisableLocks();
    }

    public static void DisableLocks()
    {
        if (gameManager == null)
            GetGameManager();

        Transform gameboardParent = gameManager.gameBoardParent;
        Transform currentSlot = null;

        if (slotLocks == null)
            slotLocks = new List<GameObject>();

        for(int i = 0; i < gameboardParent.childCount; i++)
        {
            currentSlot = gameboardParent.GetChild(i);
            if (currentSlot.childCount > 0)
            {
                SlotLock slotLock;
                for(int j = 0; j < currentSlot.childCount; j++)
                {
                    slotLock = currentSlot.GetChild(j).GetComponent<SlotLock>();
                    if (slotLock != null)
                    {
                        Debug.Log("Found Lock");

                        slotLocks.Add(slotLock.gameObject);
                        slotLock.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public static void EnableLocks()
    {
        foreach(GameObject l in slotLocks)
            l.SetActive(true);

        slotLocks.Clear();
    }
    #endregion

    #region Shape Manipulation
    public static void ResizeGameShapes(float newSize)
    {
        if (gameManager == null)
            GetGameManager();

        Transform gameBoard = gameManager.gameBoardParent;
        Transform shape = null;
        for(int i = 0; i < gameBoard.childCount; i++)
        {
            shape = gameBoard.GetChild(i).GetComponent<GameSlot>().GetSlotShapeTransform();
            if (shape != null)
                shape.localScale = new Vector3(newSize, newSize);
        }
    }
    #endregion
}
