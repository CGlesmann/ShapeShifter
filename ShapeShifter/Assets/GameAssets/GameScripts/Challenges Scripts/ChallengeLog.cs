using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Challenge Log", menuName = "Global Assets/Challenge Log")]
public class ChallengeLog : ScriptableObject
{
    [HideInInspector][SerializeReference] private List<Challenge> challengeLog = new List<Challenge>();

    public void AddChallenge(Challenge challenge)
    {
        if (challengeLog == null)
            challengeLog = new List<Challenge>();

        challengeLog.Add(challenge);
    }

    public void RemoveChallenge(int index)
    {
        if (index < 0 || index > challengeLog.Count - 1)
            return;

        challengeLog.RemoveAt(index);
    }

    public Challenge GetChallengeData(int challengeIndex)
    {
        if (challengeLog == null || challengeLog.Count - 1 < challengeIndex)
            return null;

        return challengeLog[challengeIndex];
    }

    public int GetChallengeCount()
    {
        if (challengeLog == null)
            return 0;

        return challengeLog.Count;
    }
}