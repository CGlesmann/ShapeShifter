using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ChallengePreview : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameManager gameManager = null;
    [SerializeField] private TextMeshProUGUI noChallengeText = null;

    [Header("Challenge References")]
    [SerializeField] private GameObject challengeParent = null;
    [SerializeField] private List<ChallengeEntry> challengeEntries = new List<ChallengeEntry>();

    private int packIndex = -1;
    private int levelIndex = -1;

    private void Awake()
    {
        if (gameManager != null)
            gameManager.onVictory += ResetChallengePreview;
    }

    public void SetLevelChallengePreview(ChallengeLog challengeLog, string levelName)
    {
        int challengeCount = challengeLog?.GetChallengeCount() ?? 0;

        if (challengeCount <= 0)
        {
            noChallengeText.gameObject.SetActive(true);
            challengeParent.SetActive(false);
        }
        else
        {
            noChallengeText.gameObject.SetActive(false);
            challengeParent.SetActive(true);

            if (packIndex == -1 || levelIndex == -1)
            {
                Vector2 indexes = LevelParser.GetLevelPackLevelIndexes(levelName);
                packIndex = (int)indexes.x;
                levelIndex = (int)indexes.y;
            }

            for (int i = 0; i < challengeEntries.Count; i++)
            {
                if (i <= challengeCount - 1)
                {
                    challengeEntries[i].gameObject.SetActive(true);
                    challengeEntries[i].UpdateChallengeEntry(challengeLog.GetChallengeData(i), packIndex, levelIndex);
                }
                else
                    challengeEntries[i].gameObject.SetActive(false);
            }
        }
    }

    public void ResetChallengePreview()
    {
        packIndex = -1;
        levelIndex = -1;
    }
}
