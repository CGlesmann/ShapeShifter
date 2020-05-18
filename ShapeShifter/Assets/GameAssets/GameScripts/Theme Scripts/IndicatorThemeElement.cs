using UnityEngine;
using UnityEngine.UI;

public class IndicatorThemeElement : MonoBehaviour
{
    [Header("Element Keys")]
    [SerializeField] private Theme.GeneralUIThemeKey normalKey = Theme.GeneralUIThemeKey.Button;
    [SerializeField] private Theme.GeneralUIThemeKey highlightKey = Theme.GeneralUIThemeKey.HighlightButton;

    [Header("Object Reference")]
    [SerializeField] private Image targetImage = null;

    private MenuSwipeController controller = null;
    private int elementIndex = 0;
    private bool highlighted = false;

    private void OnDisable() { ThemeManager.onThemeSettingsUpdate -= UpdateElement; }
    private void OnEnable() { ThemeManager.onThemeSettingsUpdate += UpdateElement; }

    public void SetIndicator(MenuSwipeController controller, int index, bool highlighted)
    {
        this.controller = controller;
        elementIndex = index;

        controller.onPanelSwitch += CheckForHighlight;

        this.highlighted = highlighted;
        UpdateElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    public void CheckForHighlight(int currentPanel)
    {
        highlighted = (currentPanel == elementIndex);
        UpdateElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    private void UpdateElement(Theme theme, Theme.ColorMode colorMode)
    {
        if (theme != null)
        {
            ThemeElementData data = theme.generalUIThemeDictionary.GetElementData(highlighted ? highlightKey : normalKey);
            if (data != null)
            {
                targetImage.sprite = data.GetElementSprite();
                targetImage.color = data.GetColorByMode(colorMode);
            }
        }
    }
}
