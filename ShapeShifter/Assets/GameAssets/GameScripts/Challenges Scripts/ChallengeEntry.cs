using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChallengeEntry : MonoBehaviour
{
    [Header("GUI References")]
    [SerializeField] private TextMeshProUGUI challengeText = null;
    [SerializeField] private GameObject challengeCheck = null;

    public void DisableChallengeEntry() { gameObject.SetActive(false); }

    public void UpdateChallengeText(string text) { challengeText.text = text; }
    public void MarkAsCompleted() { challengeCheck.SetActive(true); }
}
