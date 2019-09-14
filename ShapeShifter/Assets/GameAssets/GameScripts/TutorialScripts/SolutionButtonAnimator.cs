using UnityEngine;

public class SolutionButtonAnimator : MonoBehaviour
{
    private Animator anim = null;
    
    private void Awake() { anim = GetComponent<Animator>(); }

    public void ToggleBlink(bool toggle) { anim.SetBool("Blinking", toggle); }
}