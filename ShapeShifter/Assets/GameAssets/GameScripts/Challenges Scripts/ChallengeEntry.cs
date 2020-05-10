using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ChallengeEntry : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TextMeshProUGUI challengeText = null;
    [SerializeField] private Image starIcon = null;

    [Header("Sprite References")]
    [SerializeField] private Sprite emptyStarIcon = null;
    [SerializeField] private Sprite filledStarIcon = null;

    [Header("Entry Settings")]
    [SerializeField] private int challengeIndex = 0;

    public void UpdateChallengeEntry(Challenge challengeData, int packIndex, int levelIndex)
    {
        challengeText.text = $"- {challengeData.challengeDescription}";
        bool challengeCompleted = DataTracker.gameData.GetChallengeResult(Challenge.GetChallengeKey(packIndex, levelIndex, challengeIndex));

        if (challengeCompleted)
            starIcon.sprite = filledStarIcon;
        else
            starIcon.sprite = emptyStarIcon;
    }
}
