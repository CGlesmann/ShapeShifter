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

    private void Start()
    {
        availableThemes = Resources.LoadAll<Theme>("Themes/");

        UpdateHeaderText(0);
        menuSwipeController.onPanelSwitch += UpdateHeaderText;
    }

    public void UpdateHeaderText(int index) { headerText.text = $"{availableThemes[index].name}";}
}
