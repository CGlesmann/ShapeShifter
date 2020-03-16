using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionManager : Instructions
{
    [Header("Object References")]
    [SerializeField] private MenuSwipeController menuSwipeController = null;

    [Header("Control Variable")]
    [SerializeField] private bool initialTutorial = false;
    [SerializeField] private bool destroyTutorial = false;

    /// <summary>
    /// Get the arrow of how to panels
    /// </summary>
    public override void Awake()
    {
        base.Awake();

        // Enabling the tutorial (if needed)
        if (initialTutorial && !DataTracker.gameData.initialTutorialComplete)
        {
            InvokeInstructions();
            menuSwipeController.TransitionToPanel(0);
        }
        else if (destroyTutorial && !DataTracker.gameData.destroyTutorialComplete)
        {
            InvokeInstructions();
            menuSwipeController.TransitionToPanel(3);
        }
    }

    public override void DisableInstructions()
    {
        // Disabling the instructions
        instructionsParent.SetActive(false);

        // Marking the tutorial as complete
        if (initialTutorial)
        {
            DataTracker.gameData.initialTutorialComplete = true;
            DataTracker.dataTracker.SaveData();
        }

        if (destroyTutorial)
        {
            DataTracker.gameData.destroyTutorialComplete = true;
            DataTracker.dataTracker.SaveData();
        }

        GameState.gamePaused = false;
    }
}
