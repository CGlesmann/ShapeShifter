using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataAccessor
{
    public void SetData(string key, object data) { DataTracker.gameData.SetData(key, data); }
    public T GetDataValue<T>(string key) { return DataTracker.gameData.GetDataValue<T>(key); }
}
