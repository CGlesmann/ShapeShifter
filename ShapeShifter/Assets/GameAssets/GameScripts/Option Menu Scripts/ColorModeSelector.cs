using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColorModeSelector : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TextMeshProUGUI selectedColorModeTitleText = null;

    int amountOfAvailableModes;

    Theme.ColorMode currentlySelectedMode;
    int currentlySelectedModeIndex;

    private void OnEnable()
    {
        amountOfAvailableModes = Enum.GetNames(typeof(Theme.ColorMode)).Length;
        currentlySelectedMode = DataTracker.gameData.GetSavedColorMode();
        currentlySelectedModeIndex = (int)currentlySelectedMode;
        selectedColorModeTitleText.text = $"{currentlySelectedMode}";
    }

    public void SelectNextMode()
    {
        currentlySelectedModeIndex++;
        if (currentlySelectedModeIndex > amountOfAvailableModes - 1)
            currentlySelectedModeIndex = 0;

        currentlySelectedMode = (Theme.ColorMode)currentlySelectedModeIndex;
        selectedColorModeTitleText.text = $"{currentlySelectedMode}";
        DataTracker.gameData.SetColorMode(currentlySelectedMode);
        DataTracker.dataTracker.SaveData();
    }

    public void SelectPreviousMode()
    {
        currentlySelectedModeIndex--;
        if (currentlySelectedModeIndex < 0)
            currentlySelectedModeIndex = amountOfAvailableModes - 1;

        currentlySelectedMode = (Theme.ColorMode)currentlySelectedModeIndex;
        selectedColorModeTitleText.text = $"{currentlySelectedMode}";
        DataTracker.gameData.SetColorMode(currentlySelectedMode);
        DataTracker.dataTracker.SaveData();
    }
}
