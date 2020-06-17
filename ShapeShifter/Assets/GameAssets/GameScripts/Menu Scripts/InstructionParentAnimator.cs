using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionParentAnimator : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private InstructionManager instructionManager = null;

    public void DisableInstructions() { instructionManager.DisableInstructions(); }
}
