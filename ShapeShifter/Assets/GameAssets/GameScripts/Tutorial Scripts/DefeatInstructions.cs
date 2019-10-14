using System.Collections.Generic;
using UnityEngine;

public class DefeatInstructions : Instructions
{
    public override void DisableInstructions()
    {
        DataTracker.gameData.defeatTutorialComplete = true;
        base.DisableInstructions();
    }
}
