using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimeChallenge : Challenge, IChallenge
{
    public float requiredTime = 0f; // In Seconds
    private float passedTime = 0f;

    public void SubscribeToCheck() { GameManager.onClockTick += UpdateTrackers; }
    public void UpdateTrackers() { passedTime = GameManager.manager.levelTimer; }
    public bool CheckForCompletedChallenge() { return (passedTime <= requiredTime); }
}

public class MoveChallenge : Challenge, IChallenge
{
    public int requiredMoves = 0;
    private int movesMade = 0;

    public void SubscribeToCheck() { GameManager.onShapeSwap += UpdateTrackers; }
    public void UpdateTrackers() { movesMade++; }
    public bool CheckForCompletedChallenge() { return (movesMade <= requiredMoves); }
}

public class Challenge
{
    public string challengeDescription = "";
}

public interface IChallenge
{
    void SubscribeToCheck();
    void UpdateTrackers();
    bool CheckForCompletedChallenge();
}