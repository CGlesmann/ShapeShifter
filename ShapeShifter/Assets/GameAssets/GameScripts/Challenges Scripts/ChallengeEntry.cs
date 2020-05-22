using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ChallengeEntry : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private DynamicGeneralThemeElement starThemeElement = null;
    [SerializeField] private TextMeshProUGUI challengeText = null;

    [Header("Entry Settings")]
    [SerializeField] private int challengeIndex = 0;

    public void UpdateChallengeEntry(Challenge challengeData, int packIndex, int levelIndex)
    {
        challengeText.text = $"- {challengeData.challengeDescription}";
        bool challengeCompleted = DataTracker.gameData.GetChallengeResult(Challenge.GetChallengeKey(packIndex, levelIndex, challengeIndex));

        if (challengeCompleted)
            starThemeElement.SetElementToHighlighted();
        else
            starThemeElement.SetElementToNormal();
    }
}
