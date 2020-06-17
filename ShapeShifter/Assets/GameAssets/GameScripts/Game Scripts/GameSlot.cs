using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
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
    private bool highlightedDrag = false;
    private int slotIndex = -1;

    #region Slot State Management Methods
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
    public void DestroyShape() { if (canInteract) GameObject.Destroy(slotShape.gameObject); }
    public void ResetSlotState() { selected = false; themeElement.SetElementToNormal(); }
    #endregion

    #region Slot Input Methods
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

    public void DisableSlotSelection() { canSelect = false; }
    public void EnableSlotSelection() { canSelect = true; }

    public void LockGameSlot() { DisableSlotSelection(); canInteract = false; }
    public void UnlockGameSlot() { EnableSlotSelection(); canInteract = true; }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log($"BeginDrag {gameObject.name}");

        GameShape gameShape = GetSlotShape();
        ShapeData shapeData = gameShape?.GetShapeData();
        if (canInteract && canSelect && shapeData != null)
        {
            DragInputManager.dragInputManager.SetNewShapeToDrag(shapeData, this, gameShape?.GetComponent<Transform>().localScale ?? Vector3.one);
            Color baseColor = gameShape.GetComponent<Image>().color;
            gameShape.GetComponent<Image>().color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a / 2f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"EndDrag {gameObject.name}");

        Color baseColor = GetSlotShape().GetComponent<Image>().color;
        GetSlotShape().GetComponent<Image>().color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * 2f);

        DragInputManager.dragInputManager.FinishDrag();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"PointerEnter {gameObject.name}");

        ShapeData shapeData = GetSlotShape()?.GetShapeData();
        if (shapeData != null && DragInputManager.dragInputManager.SetSecondarySlot(this))
            highlightedDrag = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"PointerExit {gameObject.name}");
        if (highlightedDrag)
        {
            DragInputManager.dragInputManager.ResetSecondarySlot();
            highlightedDrag = false;
        }
    }
    #endregion
}
