using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Instructions : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] protected GameObject instructionsParent = null;
    [SerializeField] protected Transform screenParent = null;
    [SerializeField] protected Animator anim = null;
    protected HTPScreen[] screenControllers = null;

    public virtual void Awake()
    {
        // Creating a new array
        screenControllers = new HTPScreen[screenParent.childCount];

        // Loop through each child of screenParent
        for (int i = 0; i < screenParent.childCount; i++)
        {
            // Grab a reference to the child's transform and store in array
            screenControllers[i] = screenParent.GetChild(i) != null ? screenParent.GetChild(i).GetComponent<HTPScreen>() : null;

            if (screenControllers[i] != null)
                screenControllers[i].DeactivateScreen();
        }
    }

    public void InvokeInstructions() { instructionsParent.SetActive(true); GameState.gamePaused = true; }
    public void InvokeExitTransition() { anim.SetTrigger("Exit"); GameState.gamePaused = false; }
    public virtual void DisableInstructions() { instructionsParent.SetActive(false); GameState.gamePaused = false; }
}

