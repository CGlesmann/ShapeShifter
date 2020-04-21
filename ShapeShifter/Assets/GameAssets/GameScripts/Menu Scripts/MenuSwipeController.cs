using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSwipeController : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public delegate void OnPanelSwitch(int panelIndex);
    public event OnPanelSwitch onPanelSwitch;

    [Header("Object References")]
    [SerializeField] private Transform panelParent = null;

    [Header("Drag Settings")]
    [SerializeField] private Vector3 baseMenuOffset = Vector3.zero;
    [Range(0, 1)] [SerializeField] private float dragThreshold = 0f;
    [SerializeField] private float transitionTime = 2f;
    [SerializeField] private float panelSpacing = 0f;

    private Coroutine currentCoroutine = null;
    private Vector3 panelStartPosition = Vector3.zero;
    public int currentPanelIndex = 0;
    private int activePanelCount = 0;
    private float remainingTransitionTime = 0;

    private void Awake()
    {
        panelStartPosition = panelParent.localPosition;

        int activePanelCounter = 0;
        for(int i = 0; i < panelParent.childCount; i++)
            if (panelParent.GetChild(i).gameObject.activeSelf) activePanelCounter++;

        activePanelCount = activePanelCounter;
    }

    public void OnDrag(PointerEventData eventData)
    {
        float pointerXDifference = eventData.pressPosition.x - eventData.position.x;
        panelParent.localPosition = panelStartPosition - new Vector3(pointerXDifference, 0f, 0f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float percentage = (eventData.pressPosition.x - eventData.position.x) / panelSpacing;
        if (Mathf.Abs(percentage) >= dragThreshold)
        {
            Vector3 newLocation = panelStartPosition;
            if (percentage > 0)
            {
                if (currentPanelIndex < activePanelCount - 1)
                {
                    newLocation = new Vector3(++currentPanelIndex * -panelSpacing, 0f, 0f) + baseMenuOffset;

                    currentCoroutine = StartCoroutine(SmoothMove(panelParent.localPosition, newLocation, transitionTime));
                    Debug.Log($"Moving from {panelParent.localPosition} to {newLocation} (Index{currentPanelIndex}) in {transitionTime}s");
                }
                else
                {
                    currentCoroutine = StartCoroutine(SmoothMove(panelParent.localPosition, panelStartPosition, transitionTime));
                    Debug.Log($"Moving from {panelParent.localPosition} to {panelStartPosition} in {transitionTime}s");
                }
            }
            else if (percentage < 0)
            {
                if (currentPanelIndex > 0)
                {
                    newLocation = new Vector3(--currentPanelIndex * -panelSpacing, 0f, 0f) + baseMenuOffset;

                    currentCoroutine = StartCoroutine(SmoothMove(panelParent.localPosition, newLocation, transitionTime));
                    Debug.Log($"Moving from {panelParent.localPosition} to {newLocation} (Index{currentPanelIndex}) in {transitionTime}s");
                }
                else
                {
                    currentCoroutine = StartCoroutine(SmoothMove(panelParent.localPosition, panelStartPosition, transitionTime));
                    Debug.Log($"Moving from {panelParent.localPosition} to {panelStartPosition} in {transitionTime}s");
                }
            }
        }
        else
        {
            currentCoroutine = StartCoroutine(SmoothMove(panelParent.localPosition, panelStartPosition, transitionTime));
            Debug.Log($"Moving from {panelParent.localPosition} to {panelStartPosition} in {transitionTime}s");
        }
    }

    public void BeginRightTransition()
    {
        if (currentPanelIndex < activePanelCount - 1)
        {
            currentPanelIndex++;
            currentCoroutine = StartCoroutine(SmoothMove(panelParent.localPosition, panelParent.localPosition + new Vector3(-panelSpacing, 0f, 0f), transitionTime));
        }
    }

    public void TransitionToPanel(int targetIndex)
    {
        int index = Mathf.Clamp(targetIndex, 0, activePanelCount - 1);
        int indexDiff = targetIndex - currentPanelIndex;
        Vector3 targetPosition = new Vector3(index * -panelSpacing, panelParent.localPosition.y, panelParent.localPosition.z);

        currentPanelIndex = targetIndex;

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        currentCoroutine = StartCoroutine(SmoothMove(panelParent.localPosition, targetPosition, transitionTime * Mathf.Abs(indexDiff)));
    }

    public void SetCurrentPanel(int targetIndex)
    {
        int index = Mathf.Clamp(targetIndex, 0, activePanelCount - 1);
        int indexDiff = targetIndex - currentPanelIndex;

        panelParent.localPosition = new Vector3(panelParent.localPosition.x + (-indexDiff * panelSpacing), panelParent.localPosition.y, panelParent.localPosition.z);
        panelStartPosition = panelParent.localPosition;
        currentPanelIndex = targetIndex;

        onPanelSwitch?.Invoke(currentPanelIndex);
    }

    public float GetRemainingTransitionTime() { return remainingTransitionTime; }
    private IEnumerator SmoothMove(Vector3 startPosition, Vector3 endPosition, float transitionTime)
    {
        float timer = 0f;
        float inc;
        remainingTransitionTime = transitionTime;

        while (timer <= 1f)
        {
            inc = Time.deltaTime / transitionTime;
            timer += inc;
            panelParent.localPosition = Vector3.Lerp(startPosition, endPosition, Mathf.SmoothStep(0f, 1f, timer));

            remainingTransitionTime -= inc;
            yield return null;
        }

        panelParent.localPosition = endPosition;
        panelStartPosition = endPosition;
        remainingTransitionTime = 0f;

        onPanelSwitch?.Invoke(currentPanelIndex);
    }
}
