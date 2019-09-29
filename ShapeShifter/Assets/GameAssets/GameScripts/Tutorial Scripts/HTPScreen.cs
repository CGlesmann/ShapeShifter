using UnityEngine;

public class HTPScreen : MonoBehaviour
{
    private Animator anim = null;

    private void OnEnable() { anim = GetComponent<Animator>(); }

    public void ActivateScreen() { if (anim != null) anim.SetBool("Active", true); }
    public void DeactivateScreen() { if (anim != null) anim.SetBool("Active", false); }
}

