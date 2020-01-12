using System.Collections;
using UnityEngine;
using TMPro;

public class HorizontalPanelController : MonoBehaviour
{
    [Header("Control Variables")]
    [SerializeField] protected int startPanel = 0;

    [Header("Slider References")]
    [SerializeField] protected Transform screenParent = null;

    protected Transform[] panels = null;
    protected Transform currentPanel => panels[currentPanelIndex];

    protected bool scrolling = false;
    protected int previousPanelIndex = 0;
    protected int currentPanelIndex = 0;
    protected int activePanelCount = 0;

    [Header("Slider GUI References")]
    [SerializeField] protected GameObject previousButton = null;
    [SerializeField] protected GameObject nextButton = null;
    [SerializeField] protected TextMeshProUGUI pageCounterText = null;

    [Header("Slider Settings")]
    [SerializeField] protected float scrollSpeed = 1f;

    public virtual void Awake()
    {
        // Creating a new array
        panels = new Transform[screenParent.childCount];
        activePanelCount = 0;

        // Loop through each child of screenParent
        for (int i = 0; i < screenParent.childCount; i++)
        {
            // Grab a reference to the child's transform and store in array
            panels[i] = screenParent.GetChild(i);
            activePanelCount += panels[i].gameObject.activeSelf ? 1 : 0;
        }

        // Setting the position to the default panel
        screenParent.localPosition = new Vector3(-panels[startPanel].localPosition.x, screenParent.localPosition.y, screenParent.localPosition.z);
        currentPanelIndex = startPanel;

        // Setting the default UI
        UpdatePageCounter();

        // Disabling the previous button
        previousButton.SetActive(false);
        nextButton.SetActive(true);
    }

    #region UI Transition Methods
    /// <summary>
    /// Begins a left transition
    /// </summary>
    public virtual void BeginLeftTransition()
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

                // Checking for the end of the panels
                nextButton.SetActive(true);
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
    public virtual void BeginRightTransition()
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

                // Checking for the end of the panels
                previousButton.SetActive(true);
                if (currentPanelIndex == activePanelCount - 1)
                    nextButton.SetActive(false);
                else
                    nextButton.SetActive(true);

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
    protected IEnumerator PanelTransition(Transform currentPanel, Transform targetPanel)
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
    }

    /// <summary>
    /// Updates the counter text
    /// </summary>
    public void UpdatePageCounter() { pageCounterText.text = (currentPanelIndex + 1).ToString() + "/" + activePanelCount; }
    #endregion
}
