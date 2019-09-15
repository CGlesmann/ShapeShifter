using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionManager : MonoBehaviour
{
    [Header("Slider References")]
    [SerializeField] private Transform screenParent = null;

    private Transform[] panels = null;
    private Transform currentPanel => panels[currentPanelIndex];

    private bool scrolling = false;
    private int currentPanelIndex = 0;

    [Header("GUI References")]
    [SerializeField] private GameObject instructionsParent = null;
    [SerializeField] private GameObject startButton = null;
    [SerializeField] private TextMeshProUGUI pageCounterText = null;

    [Header("Slider Settings")]
    [SerializeField] private float scrollSpeed = 1f;

    /// <summary>
    /// Get the arrow of how to panels
    /// </summary>
    private void Awake()
    {
        // Enabling the tutorial (if needed)
        instructionsParent.SetActive(!GameState.forcedTutorialCompleted);

        // Creating a new array
        panels = new Transform[screenParent.childCount];

        // Loop through each child of screenParent
        for (int i = 0; i < screenParent.childCount; i++)
        {
            // Grab a reference to the child's transform and store in array
            panels[i] = screenParent.GetChild(i);
        }

        // Setting the default UI
        UpdatePageCounter();
    }

    /// <summary>
    /// Enables the instructions and sets the default UI state
    /// </summary>
    public void InvokeInstructions()
    {
        // Enabling the instructions
        instructionsParent.SetActive(true);

        // Setting the default UI State
        screenParent.localPosition = new Vector3(0f, screenParent.localPosition.y, screenParent.localPosition.z);
        currentPanelIndex = 0;
        UpdatePageCounter();
        startButton.SetActive(false);
    }

    /// <summary>
    /// Disables the instructions
    /// </summary>
    public void DisableInstructions()
    {
        // Disabling the instructions
        instructionsParent.SetActive(false);

        // Marking the tutorial as complete
        GameState.forcedTutorialCompleted = true;
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
            StartCoroutine(PanelTransition(currentPanel, targetPanel));

            // Incrementing the counter
            currentPanelIndex--;

            // Setting the scrolling state 
            scrolling = true;

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
            StartCoroutine(PanelTransition(currentPanel, targetPanel));

            // Incrementing the counter
            currentPanelIndex++;

            // Setting the scrolling state 
            scrolling = true;
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

        // Checking for the end of the panels
        if (currentPanelIndex == panels.Length - 1)
            startButton.SetActive(true);
        else
            startButton.SetActive(false);
    }

    /// <summary>
    /// Updates the counter text
    /// </summary>
    public void UpdatePageCounter() { pageCounterText.text = (currentPanelIndex + 1).ToString() + "/" + panels.Length; }
}
