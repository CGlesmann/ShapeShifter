using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelTools
{
    [MenuItem("Clear All Boards", menuItem = "Level Tools/Clear All Board")]
    public static void ClearAllBoards()
    {
        ClearGameBoard();
        ClearSolutionBoard();
    }

    [MenuItem("Clear GameBoard", menuItem = "Level Tools/Clear GameBoard")]
    public static void ClearGameBoard()
    {
        // Searching for a GameManager
        GameManager manager = (GameManager)Object.FindObjectOfType(typeof(GameManager));

        // Execute algo if manager available
        if (manager != null)
        {
            // Getting the Game Board Reference
            Transform gameBoard = manager.gameBoardParent;
            Transform slot;

            // Getting each slot, removing the child
            for(int i = 0; i < gameBoard.childCount; i++)
            {
                // Getting the slot
                slot = gameBoard.GetChild(i);

                // Removing the child from the slot
                if (slot.childCount > 0)
                    Object.DestroyImmediate(slot.GetChild(0).gameObject);
            }
        }
    }

    [MenuItem("Clear SolutionBoard", menuItem = "Level Tools/Clear SolutionBoard")]
    public static void ClearSolutionBoard()
    {
        // Searching for a GameManager
        GameManager manager = (GameManager)Object.FindObjectOfType(typeof(GameManager));

        // Execute algo if manager available
        if (manager != null)
        {
            // Getting the Game Board Reference
            Transform solutionBoard = manager.solutionBoardParent;
            Transform slot;

            // Getting each slot, removing the child
            for (int i = 0; i < solutionBoard.childCount; i++)
            {
                // Getting the slot
                slot = solutionBoard.GetChild(i);

                // Removing the child from the slot
                if (slot.childCount > 0)
                    Object.DestroyImmediate(slot.GetChild(0).gameObject);
            }
        }
    }
}
