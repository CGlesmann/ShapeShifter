using UnityEngine;

public class HTPScreen : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private MenuSwipeController menuSwipeController = null;
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
}

