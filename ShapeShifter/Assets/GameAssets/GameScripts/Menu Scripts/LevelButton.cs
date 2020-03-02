using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private LevelSelectManager manager = null;
    [SerializeField] private int buttonIndex = 0;
    [SerializeField] private Animator anim = null;

    private string levelName => string.Format("Level_{0}", buttonIndex + 1);
    private bool locked = false;

    public void SelectLevel()
    {
        if (!locked)
            manager.DisplayLevelPreview(levelName, buttonIndex);
    }

    public void UnlockLevelButton()
    {
        locked = false;
        GetComponent<Button>().interactable = true;
    }

    private void LockLevelButton()
    {
        locked = true;
        GetComponent<Button>().interactable = false;
    }

    public void DisplayUnlockAnimation() { UnlockLevelButton(); anim.SetTrigger("Unlock"); }
    public void SetLockDisplay() { LockLevelButton(); anim.SetBool("Unlocked", false); }
    public void SetUnlockDisplay() { UnlockLevelButton(); anim.SetBool("Unlocked", true); }
}
