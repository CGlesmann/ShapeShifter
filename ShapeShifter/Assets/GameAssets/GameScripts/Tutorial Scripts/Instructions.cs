using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Instructions : HorizontalPanelController
{
    [Header("Slider References")]
    protected HTPScreen[] screenControllers = null;

    [Header("GUI References")]
    [SerializeField] protected GameObject instructionsParent = null;
    [SerializeField] protected GameObject startButton = null;

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

        // Setting the position to the default panel
        screenParent.localPosition = new Vector3(-panels[startPanel].localPosition.x, screenParent.localPosition.y, screenParent.localPosition.z);
        currentPanelIndex = startPanel;

        // Setting the default UI
        UpdatePageCounter();

        // Disabling the previous button
        previousButton.SetActive(false);
        nextButton.SetActive(true);

        if (startButton != null)
            startButton.SetActive(false);
    }

    /// <summary>
    /// Enables the instructions and sets the default UI state
    /// </summary>
    public void InvokeInstructions()
    {
        // Enabling the instructions
        instructionsParent.SetActive(true);

        // Setting the default UI State
        screenParent.localPosition = new Vector3(-panels[startPanel].localPosition.x, screenParent.localPosition.y, screenParent.localPosition.z);
        currentPanelIndex = startPanel;

        screenControllers[currentPanelIndex].ActivateScreen();
        UpdatePageCounter();

        previousButton.SetActive(currentPanelIndex != 0);
        nextButton.SetActive(currentPanelIndex != activePanelCount - 1);
        if (startButton != null)
            startButton.SetActive(currentPanelIndex == activePanelCount - 1);

        GameState.gamePaused = true;
    }

    /// <summary>
    /// Disables the instructions
    /// </summary>
    public virtual void DisableInstructions()
    {
        // Disabling the instructions
        instructionsParent.SetActive(false);
        startPanel = 0;
        GameState.gamePaused = false;
    }

    public override void BeginLeftTransition()
    {
        panels[currentPanelIndex].GetComponent<HTPScreen>().DeactivateScreen();
        base.BeginLeftTransition();
        panels[currentPanelIndex].GetComponent<HTPScreen>().ActivateScreen();

        if (startButton != null)
            startButton.SetActive(currentPanelIndex == activePanelCount - 1);
    }

    public override void BeginRightTransition()
    {
        panels[currentPanelIndex].GetComponent<HTPScreen>().DeactivateScreen();
        base.BeginRightTransition();
        panels[currentPanelIndex].GetComponent<HTPScreen>().ActivateScreen();

        if (startButton != null)
            startButton.SetActive(currentPanelIndex == activePanelCount - 1);
    }
}

