using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionManager : MonoBehaviour
{
    [Header("Control Variable")]
    [SerializeField] private bool initialTutorial = false;
    [SerializeField] private bool destroyTutorial = false;
    [SerializeField] private int startPanel = 0;

    [Header("Slider References")]
    [SerializeField] private Transform screenParent = null;

    private Transform[] panels = null;
    private HTPScreen[] screenControllers = null;

    private Transform currentPanel => panels[currentPanelIndex];

    private bool scrolling = false;
    private int previousPanelIndex = 0;
    private int currentPanelIndex = 0;
    private int activePanelCount = 0;

    [Header("GUI References")]
    [SerializeField] private GameObject instructionsParent = null;
    [SerializeField] private GameObject startButton = null;
    [SerializeField] private GameObject previousButton = null;
    [SerializeField] private GameObject nextButton = null;
    [SerializeField] private TextMeshProUGUI pageCounterText = null;

    [Header("Slider Settings")]
    [SerializeField] private float scrollSpeed = 1f;

    /// <summary>
    /// Get the arrow of how to panels
    /// </summary>
    private void Awake()
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
            startPanel = 5;
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
        nextButton.SetActive(currentPanelIndex != panels.Length - 1);
        if (startButton != null)
            startButton.SetActive(currentPanelIndex == panels.Length - 1);

        GameState.gamePaused = true;
    }

    /// <summary>
    /// Disables the instructions
    /// </summary>
    public void DisableInstructions()
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

    /// <summary>
    /// Begins a left transition
    /// </summary>
    public void BeginLeftTransition()
    {
        // Checking for out of bounds
        if (!scrolling && currentPanelIndex > 0)
        {
            // Getting the two panels and starting the transition
            Transform targetPanel = panels[currentPanelIndex - 1];

            // Check for an active panel
            if (targetPanel.gameObject.activeSelf)
            {
                // Start the screen transition
                StartCoroutine(PanelTransition(currentPanel, targetPanel));

                // Incrementing the counter
                previousPanelIndex = currentPanelIndex;
                currentPanelIndex--;
                screenControllers[currentPanelIndex].ActivateScreen();

                // Checking for the end of the panels
                nextButton.SetActive(true);
                if (startButton != null)
                    startButton.SetActive(false);

                if (currentPanelIndex == 0)
                    previousButton.SetActive(false);
                else
                    previousButton.SetActive(true);

                // Setting the scrolling state 
                scrolling = true;
            }
        }
    }

    /// <summary>
    /// Begins a right transition
    /// </summary>
    public void BeginRightTransition()
    {
        // Checking for out of bounds
        if (!scrolling && currentPanelIndex < panels.Length - 1)
        {
            // Getting the two panels and starting the transition
            Transform targetPanel = panels[currentPanelIndex + 1];

            if (targetPanel.gameObject.activeSelf)
            {
                StartCoroutine(PanelTransition(currentPanel, targetPanel));

                // Incrementing the counter
                previousPanelIndex = currentPanelIndex;
                currentPanelIndex++;
                screenControllers[currentPanelIndex].ActivateScreen();

                // Checking for the end of the panels
                previousButton.SetActive(true);
                if (currentPanelIndex == activePanelCount - 1)
                {
                    nextButton.SetActive(false);

                    if (startButton != null)
                        startButton.SetActive(true);
                }
                else
                {
                    nextButton.SetActive(true);

                    if (startButton != null)
                        startButton.SetActive(false);
                }

                // Setting the scrolling state 
                scrolling = true;
            }
        }
    }

    /// <summary>
    /// Transitions between two panels
    /// </summary>
    /// <param name="currentPanel"></param>
    /// <param name="targetPanel"></param>
    /// <returns></returns>
    private IEnumerator PanelTransition(Transform currentPanel, Transform targetPanel)
    {
        // Getting the distance between the two panels
        float dist = currentPanel.position.x - targetPanel.position.x;

        // Getting the Target Vector2 and current Vector2
        Vector2 currentPos = screenParent.position;
        Vector2 targetPos = new Vector2(currentPos.x + dist, currentPos.y);

        // Declaring the counter variable
        bool needToUpdateText = true;
        float progress = 0f;

        // Loop until progress hits 1
        while (progress < 1f)
        {
            // Incrementing the timer
            progress = Mathf.Clamp(progress + (Time.deltaTime * scrollSpeed), 0f, 1f);

            // Check for text update
            if (needToUpdateText && progress >= 0.5f)
            {
                // Update the text
                UpdatePageCounter();

                // Set the bool
                needToUpdateText = false;
            }

            // Lerping the position
            screenParent.position = Vector2.Lerp(currentPos, targetPos, progress);

            // Delay a frame
            yield return null;
        }

        // Setting the scroll state to false
        scrolling = false;
        screenControllers[previousPanelIndex].DeactivateScreen();
    }

    /// <summary>
    /// Updates the counter text
    /// </summary>
    public void UpdatePageCounter() { pageCounterText.text = (currentPanelIndex + 1).ToString() + "/" + activePanelCount; }
}
