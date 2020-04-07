using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelTracker : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject indicatorPrefab = null;
    [SerializeField] private Transform indicatorParent = null;

    [Header("Object References")]
    [SerializeField] private MenuSwipeController menuSwipeController = null;
    [SerializeField] private Transform panelParent = null;

    [Header("Highlight Settings")]
    [SerializeField] private Color highlightColor = Color.white;
    private Color startingColor;
    private Image currentSelectedIndicator;

    private void Awake()
    {
        for (int i = 0; i < panelParent.childCount; i++)
            if (panelParent.GetChild(i).gameObject.activeSelf)
                AddPanelIndicator();

        UpdateCurrentPanel(menuSwipeController.currentPanelIndex);
        menuSwipeController.onPanelSwitch += UpdateCurrentPanel;
    }

    public void AddPanelIndicator() { Instantiate(indicatorPrefab, indicatorParent); }
    public void UpdateCurrentPanel(int index)
    {
        if (currentSelectedIndicator != null)
            currentSelectedIndicator.color = startingColor;

        currentSelectedIndicator = indicatorParent.GetChild(index).GetComponent<Image>();
        startingColor = currentSelectedIndicator.color;
        currentSelectedIndicator.color = highlightColor;
    }
}
