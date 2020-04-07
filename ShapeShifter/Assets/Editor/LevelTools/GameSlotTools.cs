using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

public static class GameSlotTools
{
    private const float baseLockSize = 384f;

    public static void CreateSlotLock(Transform slot, SlotLock.LockType lockType, int counter, float width)
    {
        GameObject lockPrefab = Resources.Load<GameObject>("Prefabs/SlotLock");

        GameObject newLock = GameObject.Instantiate(lockPrefab, slot);
        newLock.transform.localPosition = Vector3.zero;
        newLock.transform.localScale = new Vector3(width / baseLockSize, width / baseLockSize);

        SlotLock slotLock = newLock.GetComponent<SlotLock>();
        slotLock.SetLockSettings(lockType, counter);

        slot.GetComponent<GameSlot>().SetSlotLock(slotLock);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    public static void EditSlotLock(Transform slot, SlotLock.LockType lockType, int counter, float width)
    {
        SlotLock slotLock = slot.GetComponent<GameSlot>().GetSlotLock();

        slotLock.SetLockSettings(lockType, counter);
        slotLock.transform.localScale = new Vector3(width / baseLockSize, width / baseLockSize);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    public static void CreateSlotShape(Transform slot, GameShape.ShapeType shapeType, Color shapeColor, float shapeSize)
    {
        GameObject shapePrefab = Resources.Load<GameObject>("Prefabs/GameShape");

        GameObject newShape = GameObject.Instantiate(shapePrefab, slot);
        newShape.transform.localPosition = Vector3.zero;
        newShape.transform.localScale = new Vector3(shapeSize, shapeSize);

        GameShape shapeRef = newShape.GetComponent<GameShape>();
        shapeRef.SetShapeColor(shapeColor);
        shapeRef.SetShapeType(shapeType);

        GameSlot slotRef = slot.GetComponent<GameSlot>();
        slotRef.SetSlotShapeReference(newShape.transform);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    public static void EditSlotShape(Transform slot, GameShape.ShapeType shapeType, Color shapeColor, float shapeSize)
    {
        GameShape newShape = slot.GetComponent<GameSlot>().GetSlotShapeTransform().GetComponent<GameShape>();
        newShape.transform.localScale = new Vector3(shapeSize, shapeSize);

        GameShape shapeRef = newShape.GetComponent<GameShape>();
        shapeRef.SetShapeColor(shapeColor);
        shapeRef.SetShapeType(shapeType);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
