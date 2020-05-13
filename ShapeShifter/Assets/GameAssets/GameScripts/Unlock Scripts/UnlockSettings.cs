using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unlock Settings", menuName = "Global Assets/Unlock Settings")]
public class UnlockSettings : ScriptableObject
{
    [HideInInspector] [SerializeReference] public List<Unlock> unlocks = new List<Unlock>();
}

public interface IUnlock
{
    bool CheckForCompletion(int packIndex, int levelIndex);
}

[System.Serializable]
public class Unlock
{
    public string unlockDescription = "";
    public GameObject unlockPrefab = null;
}

[System.Serializable]
public class PackUnlock : Unlock, IUnlock
{
    public int packIndex, requiredLevel;

    public bool CheckForCompletion(int pi, int li) { return (pi == packIndex && li == requiredLevel); }
}
