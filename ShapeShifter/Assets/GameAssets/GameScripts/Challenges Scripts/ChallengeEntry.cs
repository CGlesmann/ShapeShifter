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

        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        Dictionary<int, bool> challengeDictionary = saveDataAccessor.GetDataValue<Dictionary<int, bool>>($"CompletedChallenges");
        if (challengeDictionary != null)
        {
            if (challengeDictionary.TryGetValue(Challenge.GetChallengeKey(packIndex, levelIndex, challengeIndex), out bool challengeCompleted))
            {
                if (challengeCompleted)
                {
                    starThemeElement.SetElementToHighlighted();
                    return;
                }
            }
        }

        starThemeElement.SetElementToNormal();
        return;
    }
}
