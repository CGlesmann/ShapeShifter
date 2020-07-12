using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class InstructionManager : Instructions
{
    [Header("Object References")]
    [SerializeField] private MenuSwipeController menuSwipeController = null;

    [Header("Forced Tutorial Settings")]
    [SerializeField] private List<TutorialEntry> tutorialLevels = null;

    public override void Awake()
    {
        base.Awake();

        string currentLevelName = LevelLoader.GetLevelName();
        foreach(TutorialEntry tutorial in tutorialLevels)
        {
            if (tutorial.levelName == currentLevelName)
            {
                InvokeInstructions();
                tutorial.tutorialEvent?.Invoke();
                return;
            }
        }
    }

    public void NavigateToBasicInstructions()
    {
        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        bool tutorialCompleted = saveDataAccessor.GetDataValue<bool>(SaveKeys.INITIAL_TUTORIAL_COMPLETE);

        if (!tutorialCompleted)
        {
            menuSwipeController.TransitionToPanel(0);
            saveDataAccessor.SetData(SaveKeys.INITIAL_TUTORIAL_COMPLETE, true);
            DataTracker.dataTracker.SaveData();
        }
    }

    public void NavigateToDestructInstructions()
    {
        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        bool tutorialCompleted = saveDataAccessor.GetDataValue<bool>(SaveKeys.DESTRUCT_TUTORIAL_COMPLETE);

        if (!tutorialCompleted)
        {
            menuSwipeController.TransitionToPanel(3);
            saveDataAccessor.SetData(SaveKeys.DESTRUCT_TUTORIAL_COMPLETE, true);
            DataTracker.dataTracker.SaveData();
        }
    }

    public void NavigateToLockInstructions()
    {
        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        bool tutorialCompleted = saveDataAccessor.GetDataValue<bool>(SaveKeys.LOCK_TUTORIAL_COMPLETE);

        if (!tutorialCompleted)
        {
            menuSwipeController.TransitionToPanel(5);
            saveDataAccessor.SetData(SaveKeys.LOCK_TUTORIAL_COMPLETE, true);
            DataTracker.dataTracker.SaveData();
        }
    }

    public void NavigateToTransformerInstructions()
    {
        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        bool tutorialCompleted = saveDataAccessor.GetDataValue<bool>(SaveKeys.TRANSFORMER_TUTORIAL_COMPLETE);

        if (!tutorialCompleted)
        {
            menuSwipeController.TransitionToPanel(7);
            saveDataAccessor.SetData(SaveKeys.TRANSFORMER_TUTORIAL_COMPLETE, true);
            DataTracker.dataTracker.SaveData();
        }
    }
}

[System.Serializable]
public class TutorialEntry
{
    public string levelName;
    public UnityEvent tutorialEvent;
}
