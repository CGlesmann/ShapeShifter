using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private LevelSelectManager manager = null;
    [SerializeField] private int buttonIndex = 0;

    private string levelName => string.Format("Level_{0}", buttonIndex + 1);
    private bool locked = false;

    [Header("UI References")]
    [SerializeField] private Image buttonImage = null;
    [SerializeField] private GameObject levelText = null;
    [SerializeField] private Sprite unlockedButtonSprite = null;
    [SerializeField] private Sprite lockedButtonSprite = null;

    private void Awake()
    {
        if (buttonIndex == 0 || DataTracker.gameData.highestCompletedLevel >= buttonIndex)
            buttonImage.sprite = unlockedButtonSprite;
        else
        {
            buttonImage.sprite = lockedButtonSprite;
            if (levelText != null)
                levelText.SetActive(false);

            locked = true;
            GetComponent<Button>().interactable = false;
        }
    }

    public void SelectLevel()
    {
        if (!locked)
        {
            Debug.Log(manager.ToString());
            manager.DisplayLevelPreview(levelName);
        }
    }
}
