using UnityEngine;

public class TutorialManager : GameManager
{
    [Header("Animator References")]
    [SerializeField] private Animator board1Animator = null;

    private void FixedUpdate() { return; }

    public void TriggerFirstBoardHighlight() { board1Animator.SetBool("Blinking", true); }
    public void StopFirstBoard() { board1Animator.SetBool("Blinking", false); }
}