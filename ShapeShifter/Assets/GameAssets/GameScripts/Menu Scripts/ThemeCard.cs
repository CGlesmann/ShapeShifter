using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeCard : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Animator cardAnim = null;
    [SerializeReference] private List<ThemeElementLoader> themeElements = new List<ThemeElementLoader>();

    [Header("Card Settings")]
    [SerializeField] private Theme themeToPreview = null;

    private void OnEnable()
    {
        foreach (ThemeElementLoader loader in themeElements)
            loader.LoadElement(themeToPreview, ThemeManager.GetCurrentColorMode());
    }

    public void PlayPressAnimation() { cardAnim.SetTrigger("Pressed"); }
    public void SetTheme() 
    {
        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();

        saveDataAccessor.SetData(SaveKeys.SELECTED_THEME_KEY, themeToPreview.name);
        DataTracker.dataTracker.SaveData();

        ThemeManager.SetTheme(themeToPreview);
        ThemeManager.InvokeUpdateMethod();
    }
}
