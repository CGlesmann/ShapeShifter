using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameSlot : GameButton, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Object References")]
    [SerializeField] private SlotLock slotLock = null;
    [SerializeField] private Transform shapeTransform = null;
    [SerializeField] private DynamicGameThemeElement themeElement = null;
    private GameManager manager = GameManager.manager;

    private GameShape slotShape => shapeTransform != null ? shapeTransform.GetComponent<GameShape>() : null;
    private Image slotRenderer => GetComponent<Image>();

    [Header("Component References")]
    [SerializeField] private Animator anim = null;

    [Header("Control Variables")]
    [SerializeField] private bool canSelect = true;
    [SerializeField] private bool pulsing = false;

    private bool canInteract = true;
    [SerializeField] private bool selected = false;
    private bool beganDrag = false;
    private bool highlightedDrag = false;
    private int slotIndex = -1;

    #region Slot State Management Methods
    public void Start() 
    { 
        manager = GameManager.manager;
        if (canSelect)
        {
            BoardManager.boardManager.onSelectGameSlots += EnablePulse;
            BoardManager.boardManager.onDeselectGameSlots += DisablePulse;
        }
    }

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
        if (!DragInputManager.dragInputManager.IsDragging())
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
    }

    public void DisableSlotSelection() { canSelect = false; }
    public void EnableSlotSelection() { canSelect = true; }

    public void LockGameSlot() { DisableSlotSelection(); canInteract = false; }
    public void UnlockGameSlot() { EnableSlotSelection(); canInteract = true; }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameShape gameShape = GetSlotShape();
        ShapeData shapeData = gameShape?.GetShapeData();
        if (canInteract && canSelect && shapeData != null)
        {
            if (!selected)
            {
                GameSlot selectedSlot = GameManager.manager.GetSelectedSlot();
                if (selectedSlot != null && selectedSlot != this)
                    GameManager.manager.DeselectSlot();

                GameManager.manager.SelectSlot(this);
                themeElement.SetElementToHighlighted();
            }

            Color baseColor = gameShape.GetComponent<Image>().color;
            gameShape.GetComponent<Image>().color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a / 2f);

            DragInputManager.dragInputManager.BeginDrag(shapeData, this, gameShape?.GetComponent<Transform>().localScale ?? Vector3.one);
            beganDrag = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (beganDrag)
        {
            Color baseColor = GetSlotShape().GetComponent<Image>().color;
            GetSlotShape().GetComponent<Image>().color = new Color(baseColor.r, baseColor.g, baseColor.b, baseColor.a * 2f);

            themeElement.SetElementToNormal();

            DragInputManager.dragInputManager.FinishDrag();

            selected = false;
            beganDrag = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DragInputManager.dragInputManager.IsDragging())
        {
            ShapeData shapeData = GetSlotShape()?.GetShapeData();
            if (shapeData != null && canInteract && canSelect && DragInputManager.dragInputManager.SetSecondarySlot(this))
            {
                highlightedDrag = true;
                themeElement.SetElementToHighlighted();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DragInputManager.dragInputManager.IsDragging() && highlightedDrag)
        {
            DragInputManager.dragInputManager.ResetSecondarySlot();
            highlightedDrag = false;

            themeElement.SetElementToTertiary();
        }
    }
    #endregion

    #region Animation Methods
    public void DisablePulse()
    {
        if (pulsing)
        {
            themeElement.SetElementToNormal();
            anim.SetTrigger("Stop");

            pulsing = false;
        }
    }

    public void EnablePulse()
    {
        if (!pulsing)
        {
            if (GetSlotShape() != null && GameManager.manager.GetSelectedSlot() != this)
            {
                pulsing = true;

                themeElement.SetElementToTertiary();
                anim.SetTrigger("Start");
            }
        }
    }
    #endregion
}
