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

    private void Awake()
    {
        int startingPanel = menuSwipeController.currentPanelIndex;
        for (int i = 0; i < panelParent.childCount; i++)
        {
            if (panelParent.GetChild(i).gameObject.activeSelf)
            {
                AddPanelIndicator(i, (startingPanel == i));
            }
        }
    }

    public void AddPanelIndicator(int index, bool highlighted)
    {
        IndicatorThemeElement newElement = Instantiate(indicatorPrefab, indicatorParent).GetComponent<IndicatorThemeElement>();
        newElement.SetIndicator(menuSwipeController, index, highlighted);
    }
}
