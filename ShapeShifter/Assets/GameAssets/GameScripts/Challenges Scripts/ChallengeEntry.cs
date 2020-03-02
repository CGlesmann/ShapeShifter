using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ChallengeEntry : MonoBehaviour
{
    [Header("Challenge Settings")]
    [SerializeField] private int challengeIndex = 0;

    [Header("GUI References")]
    [SerializeField] private TextMeshProUGUI challengeText = null;
    [SerializeField] private GameObject challengeCheck = null;

    public bool ChallengeSaveCompleted()
    {
        if (DataTracker.gameData.levelChallenges.TryGetValue(SceneManager.GetActiveScene().name, out List<bool> challengeToggles))
        {
            if (challengeToggles == null || challengeToggles.Count - 1 < challengeIndex)
                return false;

            return challengeToggles[challengeIndex];
        }
        else
            return false;
    }

    public void EnableChallengeEntry() { gameObject.SetActive(true); }
    public void DisableChallengeEntry() { gameObject.SetActive(false); }

    public void UpdateChallengeText(string text) { challengeText.text = text; }
    public void MarkAsCompleted() { challengeCheck.SetActive(true); }
}
