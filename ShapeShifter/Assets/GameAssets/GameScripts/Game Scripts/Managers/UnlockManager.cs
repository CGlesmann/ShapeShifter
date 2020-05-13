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
        notificationsToCreate = new Queue<Unlock>();
        int counter = 0;

        for(int i = 0; i < unlockSettings.unlocks.Count; i++)
        {
            IUnlock unlockInterface = unlockSettings.unlocks[i] as IUnlock;
            if (unlockInterface.CheckForCompletion(packIndex, levelIndex) && !DataTracker.gameData.GetUnlockDisplayed(i))
            {
                AddNotificationToQueue(unlockSettings.unlocks[i]);
                DataTracker.gameData.MarkUnlockAsDisplayed(i);
                DataTracker.dataTracker.SaveData();
                counter++;
            }
        }

        return counter > 0;
    }

    public void AddNotificationToQueue(Unlock unlock)
    {
        Debug.Log("Adding Notification");
        notificationsToCreate.Enqueue(unlock);
    }

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
        Debug.Log("Creating a notification");
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