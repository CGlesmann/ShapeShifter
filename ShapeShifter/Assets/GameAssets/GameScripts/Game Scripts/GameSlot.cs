using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSlot : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private SlotLock slotLock = null;
    [SerializeField] private Transform shapeTransform = null;
    [SerializeField] private DynamicGameThemeElement themeElement = null;
    private GameManager manager = GameManager.manager;

    private GameShape slotShape => shapeTransform != null ? shapeTransform.GetComponent<GameShape>() : null;
    private Image slotRenderer => GetComponent<Image>();

    [Header("Control Variables")]
    [SerializeField] private bool canSelect = true;
    private bool canInteract = true;
    private bool selected = false;
    private int slotIndex = -1;

    public void Start() { manager = GameManager.manager; }

    public bool CheckCanInteract() { return canInteract; }
    public int GetSlotIndex() { return slotIndex; }

    public SlotLock GetSlotLock() { return slotLock; }
    public Transform GetSlotShapeTransform() { return shapeTransform; }
    public GameShape GetSlotShape()
    {
        if (shapeTransform != null)
            return shapeTransform.GetComponent<GameShape>();
        else
            return null;
    }

    public void SetSlotIndex(int index) { slotIndex = index; }
    public void SetSlotLock(SlotLock l) { slotLock = l; }
    public void SetSlotShapeReference(Transform newShape) { shapeTransform = newShape; }
    public void SetSlotShape(GameShape.ShapeType shapeType, GameShape.ColorType colorType)
    {
        if (slotShape != null)
            slotShape.ConfigureShape(shapeType, colorType);
    }

    public void ToggleSelect()
    {
        if (canInteract && canSelect)
        {
            if (selected)
            {
                selected = false;
                manager.DeselectSlot();
                themeElement.SetElementToNormal();
            }
            else if (slotShape != null)
            {
                selected = manager.SelectSlot(this);
                if (selected)
                    themeElement.SetElementToHighlighted();
            }
        }
    }

    public void DestroyShape() { if (canInteract) GameObject.Destroy(slotShape.gameObject); }
    public void ResetSlotState() { selected = false; themeElement.SetElementToNormal(); }

    public void DisableSlotSelection() { canSelect = false; }
    public void EnableSlotSelection() { canSelect = true; }

    public void LockGameSlot() { DisableSlotSelection(); canInteract = false; }
    public void UnlockGameSlot() { EnableSlotSelection(); canInteract = true; }
}
