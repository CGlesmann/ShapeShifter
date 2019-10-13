using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionManager : Instructions
{
    [Header("Control Variable")]
    [SerializeField] private bool initialTutorial = false;
    [SerializeField] private bool destroyTutorial = false;

    /// <summary>
    /// Get the arrow of how to panels
    /// </summary>
    public override void Awake()
    {
        // Creating a new array
        panels = new Transform[screenParent.childCount];
        screenControllers = new HTPScreen[screenParent.childCount];
        activePanelCount = 0;

        // Loop through each child of screenParent
        for (int i = 0; i < screenParent.childCount; i++)
        {
            // Grab a reference to the child's transform and store in array
            panels[i] = screenParent.GetChild(i);
            screenControllers[i] = panels[i] != null ? panels[i].GetComponent<HTPScreen>() : null;

            activePanelCount += panels[i].gameObject.activeSelf ? 1 : 0;
            if (screenControllers[i] != null)
                screenControllers[i].DeactivateScreen();
        }

        // Enabling the tutorial (if needed)
        if (initialTutorial && !DataTracker.gameData.initialTutorialComplete)
        {
            startPanel = 0;
            InvokeInstructions();
        }
        else if (destroyTutorial && !DataTracker.gameData.destroyTutorialComplete)
        {
            startPanel = 4;
            InvokeInstructions();
        } else {
            // Setting the position to the default panel
            screenParent.localPosition = new Vector3(-panels[startPanel].localPosition.x, screenParent.localPosition.y, screenParent.localPosition.z);
            currentPanelIndex = startPanel;
        }

        // Setting the default UI
        UpdatePageCounter();

        // Disabling the previous button
        previousButton.SetActive(false);
        nextButton.SetActive(true);

        if (startButton != null)
            startButton.SetActive(false);
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

        startPanel = 0;
        GameState.gamePaused = false;
    }
}
