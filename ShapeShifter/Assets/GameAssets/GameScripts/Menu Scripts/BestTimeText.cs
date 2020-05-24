using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BestTimeText : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private TextMeshProUGUI bestTimeText = null;

    public void SetText(int packIndex, int levelIndex)
    {
        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        Dictionary<int, float> bestLevelTimes = saveDataAccessor.GetDataValue<Dictionary<int, float>>(SaveKeys.BEST_LEVEL_TIMES);

        int levelKey = GameManager.GetLevelKey(packIndex + 1, levelIndex + 1);
        if (bestLevelTimes != null && bestLevelTimes.ContainsKey(levelKey))
        {
            string timerText = GameTime.GetGameTimeFormat(bestLevelTimes[levelKey]);
            bestTimeText.text = timerText;
        }
        else
            bestTimeText.text = "--:--.--";
    }
}
