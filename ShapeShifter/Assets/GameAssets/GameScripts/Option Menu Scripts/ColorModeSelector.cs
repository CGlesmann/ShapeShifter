using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColorModeSelector : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TextMeshProUGUI selectedColorModeTitleText = null;

    SaveDataAccessor saveDataAccessor;
    int amountOfAvailableModes;

    Theme.ColorMode currentlySelectedMode;
    int currentlySelectedModeIndex;

    private void OnEnable()
    {
        saveDataAccessor = new SaveDataAccessor();

        amountOfAvailableModes = Enum.GetNames(typeof(Theme.ColorMode)).Length;
        currentlySelectedMode = saveDataAccessor.GetDataValue<Theme.ColorMode>(SaveKeys.SELECTED_COLOR_MODE);
        currentlySelectedModeIndex = (int)currentlySelectedMode;
        selectedColorModeTitleText.text = $"{currentlySelectedMode}";
    }

    public void SelectNextMode()
    {
        currentlySelectedModeIndex++;
        if (currentlySelectedModeIndex > amountOfAvailableModes - 1)
            currentlySelectedModeIndex = 0;

        SetColorMode();
    }

    public void SelectPreviousMode()
    {
        currentlySelectedModeIndex--;
        if (currentlySelectedModeIndex < 0)
            currentlySelectedModeIndex = amountOfAvailableModes - 1;

        SetColorMode();
    }

    private void SetColorMode()
    {
        currentlySelectedMode = (Theme.ColorMode)currentlySelectedModeIndex;
        selectedColorModeTitleText.text = $"{currentlySelectedMode}";

        saveDataAccessor.SetData(SaveKeys.SELECTED_COLOR_MODE, currentlySelectedMode);
        DataTracker.dataTracker.SaveData();

        ThemeManager.InvokeUpdateMethod();
    }
}
