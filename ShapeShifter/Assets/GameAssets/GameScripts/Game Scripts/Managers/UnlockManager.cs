using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    [Header("Unlock References")]
    [SerializeField] private UnlockSettings unlockSettings = null;

    private Queue<Unlock> notificationsToCreate;
    private bool paused = false;

    public bool CheckForCompletedUnlocks(int packIndex, int levelIndex)
    {
        SaveDataAccessor saveDataAccessor = new SaveDataAccessor();
        Dictionary<int, bool> unlockDictionary = saveDataAccessor.GetDataValue<Dictionary<int, bool>>(SaveKeys.UNLOCK_SAVE_KEY);

        notificationsToCreate = new Queue<Unlock>();
        int counter = 0;

        for(int i = 0; i < unlockSettings.unlocks.Count; i++)
        {
            IUnlock unlockInterface = unlockSettings.unlocks[i] as IUnlock;
            if (unlockInterface.CheckForCompletion(packIndex, levelIndex))
            {
                if (unlockDictionary == null)
                {
                    unlockDictionary = new Dictionary<int, bool>();
                    unlockDictionary.Add(i, true);

                    saveDataAccessor.SetData(SaveKeys.UNLOCK_SAVE_KEY, unlockDictionary);
                    DataTracker.dataTracker.SaveData();

                    AddNotificationToQueue(unlockSettings.unlocks[i]);
                    counter++;
                }
                else if (!unlockDictionary.ContainsKey(i))
                {
                    unlockDictionary[i] = true;
                    saveDataAccessor.SetData(SaveKeys.UNLOCK_SAVE_KEY, unlockDictionary);
                    DataTracker.dataTracker.SaveData();

                    AddNotificationToQueue(unlockSettings.unlocks[i]);
                    counter++;
                }
            }
        }

        return counter > 0;
    }

    public void AddNotificationToQueue(Unlock unlock) { notificationsToCreate.Enqueue(unlock); }
    public void StartNotificationDisplay()
    {
        if (notificationsToCreate != null && notificationsToCreate.Count > 0)
            StartCoroutine(DisplayUnlocks());
    }

    private IEnumerator DisplayUnlocks()
    {
        Unlock unlockToDisplay;
        while (notificationsToCreate.Count > 0)
        {
            unlockToDisplay = notificationsToCreate.Dequeue();
            if (unlockToDisplay != null)
            {
                CreateNotification(unlockToDisplay);
                paused = true;
            }

            while (paused)
                yield return null;
        }
    }

    private void CreateNotification(Unlock unlockData)
    {
        GameObject newNotification = Instantiate(unlockData.unlockPrefab);
        UnlockNotification unlockNotification = newNotification.GetComponent<UnlockNotification>();

        unlockNotification.UpdateNotificationText(unlockData.unlockDescription);
        unlockNotification.SubscribeEventToDestroy(ProgressDisplayRoutine);
    }

    public void ProgressDisplayRoutine()
    {
        paused = false;
    }
}