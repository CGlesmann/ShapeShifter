using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ThemeHeader : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TextMeshProUGUI headerText = null;
    [SerializeField] private MenuSwipeController menuSwipeController = null;

    private Theme[] availableThemes;

    private void Awake() { menuSwipeController.onPanelSwitch += UpdateHeaderText; }
    public void UpdateHeaderText(int index) 
    { 
        if (availableThemes == null)
            availableThemes = Resources.LoadAll<Theme>("Themes/");

        headerText.text = $"{availableThemes[index].name}";
    }
}
