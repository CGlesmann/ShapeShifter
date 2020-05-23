using System.Collections.Generic;
using UnityEngine;

public class DefeatInstructions : Instructions
{
    public override void DisableInstructions()
    {
        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        saveDataAccessor.SetData(SaveKeys.DEFEAT_TUTORIAL_COMPLETE, true);
        DataTracker.dataTracker.SaveData();

        base.DisableInstructions();
    }
}
