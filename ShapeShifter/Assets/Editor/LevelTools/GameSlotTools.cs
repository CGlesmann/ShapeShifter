using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class GameSlotTools
{
    private const float baseLockSize = 384f;

    public static void CreateSlotLock(Transform slot, SlotLock.LockType lockType, int counter, float width)
    {
        GameObject lockPrefab = Resources.Load<GameObject>("Prefabs/SlotLock");

        GameObject newLock = PrefabUtility.InstantiatePrefab(lockPrefab, slot) as GameObject;
        newLock.transform.localPosition = Vector3.zero;
        newLock.transform.localScale = new Vector3(width / baseLockSize, width / baseLockSize);

        SlotLock slotLock = newLock.GetComponent<SlotLock>();
        slotLock.SetLockSettings(lockType, counter);

        slot.GetComponent<GameSlot>().SetSlotLock(slotLock);
        EditorUtility.SetDirty(slot.GetComponent<GameSlot>());
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    public static void EditSlotLock(Transform slot, SlotLock.LockType lockType, int counter, float width)
    {
        SlotLock slotLock = slot.GetComponent<GameSlot>().GetSlotLock();

        slotLock.SetLockSettings(lockType, counter);
        slotLock.transform.localScale = new Vector3(width / baseLockSize, width / baseLockSize);

        EditorUtility.SetDirty(slotLock);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    public static void CreateSlotShape(Transform slot, GameShape.ShapeType shapeType, GameShape.ColorType shapeColor, float shapeSize)
    {
        Debug.Log("Creating a shape");
        GameObject shapePrefab = Resources.Load<GameObject>("Prefabs/GameShape");

        GameObject newShape = PrefabUtility.InstantiatePrefab(shapePrefab, slot) as GameObject;
        newShape.transform.localPosition = Vector3.zero;
        newShape.transform.localScale = new Vector3(shapeSize, shapeSize);

        GameShape shapeRef = newShape.GetComponent<GameShape>();
        shapeRef.SetShapeColor(shapeColor);
        shapeRef.SetShapeType(shapeType);

        GameSlot slotRef = slot.GetComponent<GameSlot>();
        slotRef.SetSlotShapeReference(newShape.transform);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    public static void EditSlotShape(Transform slot, GameShape.ShapeType shapeType, GameShape.ColorType shapeColor, float shapeSize)
    {
        GameShape newShape = slot.GetComponent<GameSlot>().GetSlotShapeTransform().GetComponent<GameShape>();
        newShape.transform.localScale = new Vector3(shapeSize, shapeSize);

        GameShape shapeRef = newShape.GetComponent<GameShape>();
        shapeRef.SetShapeColor(shapeColor);
        shapeRef.SetShapeType(shapeType);
        EditorUtility.SetDirty(shapeRef);

        EditorUtility.SetDirty(shapeRef.GetComponent<Image>());
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
