using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimeChallenge : Challenge, IChallenge
{
    public float requiredTime = 0f; // In Seconds
    private float passedTime = 0f;

    public TimeChallenge() { challengeDescription = "Complete the level within 0 seconds"; }

    public void SetUpChallenge() { passedTime = 0f; GameManager.manager.onClockTick += UpdateTrackers; }
    public void UnsubscribeChallenge() { GameManager.manager.onClockTick -= UpdateTrackers; }
    public void UpdateTrackers() { passedTime = GameManager.manager.levelTimer; }
    public bool CheckForCompletedChallenge() { return (passedTime <= requiredTime); }
}

[System.Serializable]
public class MoveChallenge : Challenge, IChallenge
{
    public int requiredMoves = 0;
    private int movesMade = 0;

    public MoveChallenge() { challengeDescription = "Complete the level within 0 moves"; }

    public void SetUpChallenge() { movesMade = 0; BoardManager.boardManager.onShapeSwap += UpdateTrackers; }
    public void UnsubscribeChallenge() { BoardManager.boardManager.onShapeSwap -= UpdateTrackers; }
    public void UpdateTrackers() { movesMade++; }
    public bool CheckForCompletedChallenge() { return (movesMade <= requiredMoves); }
}

[System.Serializable]
public class ModeSwitchChallenge : Challenge, IChallenge
{
    public int maximumModeSwitches = 0;
    private int switchesMade = 0;

    public ModeSwitchChallenge() { challengeDescription = "Switch Destruction Mode 2 times or less"; }

    public void SetUpChallenge() { Debug.Log("Subscribing SwitchChal"); switchesMade = 0; GameManager.manager.onModeSwitch += UpdateTrackers; }
    public void UnsubscribeChallenge() { GameManager.manager.onModeSwitch -= UpdateTrackers; }
    public void UpdateTrackers() { Debug.Log("Updating ModeSwitchChallenge"); switchesMade++; }
    public bool CheckForCompletedChallenge() { return (switchesMade <= maximumModeSwitches); }
}

[System.Serializable]
public class Challenge
{
    public string challengeDescription = "";
    public static int GetChallengeKey(int packIndex, int levelIndex, int challengeIndex) { return (packIndex * 10000) + (levelIndex * 10) + challengeIndex; }
}

public interface IChallenge
{
    void SetUpChallenge();
    void UnsubscribeChallenge();
    void UpdateTrackers();
    bool CheckForCompletedChallenge();
}