using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ColorModeSelector : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private List<DynamicGeneralThemeElement> buttonThemeElements = null;

    private SaveDataAccessor saveDataAccessor;
    private DynamicGeneralThemeElement currentHighlighedElement;
    private int currentlySelectedModeIndex;

    private void OnEnable()
    {
        saveDataAccessor = new SaveDataAccessor();
        currentlySelectedModeIndex = (int)saveDataAccessor.GetDataValue<Theme.ColorMode>(SaveKeys.SELECTED_COLOR_MODE);

        HighlightButton(currentlySelectedModeIndex);
    }

    public void SetColorMode(int selectedColorMode)
    {
        currentlySelectedModeIndex = selectedColorMode;

        currentHighlighedElement.SetElementToNormal();
        saveDataAccessor.SetData(SaveKeys.SELECTED_COLOR_MODE, (Theme.ColorMode)currentlySelectedModeIndex);
        HighlightButton(currentlySelectedModeIndex);

        DataTracker.dataTracker.SaveData();
        ThemeManager.InvokeUpdateMethod();
    }

    private void HighlightButton(int buttonIndex)
    {
        currentHighlighedElement = buttonThemeElements[buttonIndex];
        currentHighlighedElement.SetElementToHighlighted();
    }
}
