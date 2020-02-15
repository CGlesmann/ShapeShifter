using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Challenge Log", menuName = "Challenge Assets/Challenge Log", order = 1)]
public class ChallengeLog : ScriptableObject
{
    public List<ChallengeList> challengeLog = new List<ChallengeList>();

    public ChallengeList GetLevelChallengeList(int index)
    {
        if (index > challengeLog.Count - 1)
            return null;
        else
            return challengeLog[index];
    }
}

[System.Serializable]
public class ChallengeList
{
    [SerializeReference][HideInInspector] public List<IChallenge> challenges = new List<IChallenge>();
}
