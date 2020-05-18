using UnityEngine;

public class HTPScreen : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private MenuSwipeController menuSwipeController = null;
    [SerializeField] private Transform gameBoardParent = null;
    [SerializeField] private Transform solutionBoardParent = null;
    private Animator anim = null;
    private int screenIndex = 0;

    private void OnEnable()
    {
        anim = GetComponent<Animator>();
        screenIndex = transform.GetSiblingIndex();

        menuSwipeController.onPanelSwitch += CheckForActiveScreen;
    }

    public void CheckForActiveScreen(int index)
    {
        if (screenIndex != index)
            DeactivateScreen();
        else
            ActivateScreen();
    }

    public void ActivateScreen() { if (anim != null) anim.SetBool("Active", true); }
    public void DeactivateScreen() { if (anim != null) anim.SetBool("Active", false); }
    
    public void HighlightGameboardSlot(int slotIndex) { HighlightSlot(gameBoardParent, slotIndex); }
    public void HighlightSolutionboardSlot(int slotIndex) { HighlightSlot(solutionBoardParent, slotIndex); }

    public void ResetGameboardSlot(int slotIndex) { ResetSlot(gameBoardParent, slotIndex); }
    public void ResetSolutionboardSlot(int slotIndex) { ResetSlot(solutionBoardParent, slotIndex); }

    public void HighlightSlot(Transform boardParent, int slotIndex)
    {
        DynamicGameThemeElement themeElement = boardParent.GetChild(slotIndex).GetComponent<DynamicGameThemeElement>();
        themeElement.SetElementToHighlighted();
    }

    public void ResetSlot(Transform boardParent, int slotIndex)
    {
        DynamicGameThemeElement themeElement = boardParent.GetChild(slotIndex).GetComponent<DynamicGameThemeElement>();
        themeElement.SetElementToNormal();
    }
}

