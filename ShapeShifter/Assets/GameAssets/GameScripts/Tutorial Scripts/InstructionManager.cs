using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionManager : Instructions
{
    [Header("Object References")]
    [SerializeField] private MenuSwipeController menuSwipeController = null;

    private SaveDataAccessor saveDataAccessor;

    [Header("Control Variable")]
    [SerializeField] private bool initialTutorial = false;
    [SerializeField] private bool destroyTutorial = false;
    [SerializeField] private bool lockTutorial = false;
    [SerializeField] private bool transformerTutorial = false;

    /// <summary>
    /// Get the arrow of how to panels
    /// </summary>
    public override void Awake()
    {
        saveDataAccessor = new SaveDataAccessor();
        base.Awake();

        // Enabling the tutorial (if needed)
        if (initialTutorial && !saveDataAccessor.GetDataValue<bool>(SaveKeys.INITIAL_TUTORIAL_COMPLETE))
        {
            InvokeInstructions();
            NavigateToBasicInstructions();
        }
        else if (destroyTutorial && !saveDataAccessor.GetDataValue<bool>(SaveKeys.DESTRUCT_TUTORIAL_COMPLETE))
        {
            InvokeInstructions();
            NavigateToDestructInstructions();
        } else if (lockTutorial && !saveDataAccessor.GetDataValue<bool>(SaveKeys.LOCK_TUTORIAL_COMPLETE))
        {
            InvokeInstructions();
            NavigateToLockInstructions();
        }
    }

    public override void DisableInstructions()
    {
        // Disabling the instructions
        instructionsParent.SetActive(false);

        // Marking the tutorial as complete
        if (initialTutorial)
        {
            saveDataAccessor.SetData(SaveKeys.INITIAL_TUTORIAL_COMPLETE, true);
            DataTracker.dataTracker.SaveData();
        }

        if (destroyTutorial)
        {
            saveDataAccessor.SetData(SaveKeys.DESTRUCT_TUTORIAL_COMPLETE, true);
            DataTracker.dataTracker.SaveData();
        }

        if (lockTutorial)
        {
            saveDataAccessor.SetData(SaveKeys.LOCK_TUTORIAL_COMPLETE, true);
            DataTracker.dataTracker.SaveData();
        }

        if (transformerTutorial)
        {
            saveDataAccessor.SetData(SaveKeys.TRANSFORMER_TUTORIAL_COMPLETE, true);
            DataTracker.dataTracker.SaveData();
        }

        GameState.gamePaused = false;
    }

    public void NavigateToBasicInstructions() { menuSwipeController.TransitionToPanel(0); }
    public void NavigateToDestructInstructions() { menuSwipeController.TransitionToPanel(3); }
    public void NavigateToLockInstructions() { menuSwipeController.TransitionToPanel(5); }
    public void NavigateToTransformerInstructions() { menuSwipeController.TransitionToPanel(7); }
}
