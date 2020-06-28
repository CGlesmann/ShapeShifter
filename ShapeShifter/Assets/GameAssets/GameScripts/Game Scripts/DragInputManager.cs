using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DragInputManager : MonoBehaviour
{
    public static DragInputManager dragInputManager = null;

    [Header("Object References")]
    [SerializeField] private GameManager gameManager = null;
    [SerializeField] private BoardManager boardManager = null;

    [Header("Component References")]
    [SerializeField] private Transform rendererTransform = null;
    [SerializeField] private GameObject shapeRenderer = null;
    [SerializeField] private GameShape gameShape = null;

    private bool dragging = false;
    private GameSlot startingSlot = null;
    private GameSlot highlightedSecondarySlot = null;

    private void OnDisable() { dragInputManager = null; }
    private void Awake()
    {
        if (dragInputManager == null)
        {
            dragInputManager = this;
            StopDisplayingDraggedShape();
        }
        else
            GameObject.DestroyImmediate(gameObject);
    }

    private void Update()
    {
        if (dragging)
            rendererTransform.position = Input.mousePosition;
    }

    public void StopDisplayingDraggedShape() { shapeRenderer.SetActive(false); }
    public void BeginDrag(ShapeData newShape, GameSlot slot, Vector3 scale)
    {
        rendererTransform.localScale = scale;
        rendererTransform.position = Input.mousePosition;
        shapeRenderer.SetActive(true);

        gameShape.ConfigureShape(newShape.shapeType, newShape.shapeColor);

        dragging = true;
        startingSlot = slot;

        boardManager.SelectGameSlots();
    }

    public void FinishDrag()
    {
        boardManager.DeselectGameSlots();

        if (startingSlot != null && highlightedSecondarySlot != null)
            gameManager.SelectSlot(highlightedSecondarySlot);
        else
            gameManager.DeselectSlot();

        StopDisplayingDraggedShape();
        dragging = false;
        startingSlot = null;
    }

    public bool IsDragging() { return dragging; }

    public ShapeData GetDraggingShape()
    {
        if (dragging)
            return gameShape.GetShapeData();
        else
            return null;
    }

    public GameSlot GetStartingSlot()
    {
        if (dragging)
            return startingSlot;
        else
            return null;
    }

    public void ResetSecondarySlot() { highlightedSecondarySlot = null; }
    public bool SetSecondarySlot(GameSlot newSlot)
    {
        if (newSlot != null && newSlot != startingSlot)
        {
            highlightedSecondarySlot = newSlot;
            return true;
        }

        return false;
    }
}
