using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragInputManager : MonoBehaviour
{
    public static DragInputManager dragInputManager = null;

    [Header("Object References")]
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
        {
            rendererTransform.position = Input.mousePosition;
        }
    }

    public void StopDisplayingDraggedShape() { shapeRenderer.SetActive(false); dragging = false; startingSlot = null; }
    public void SetNewShapeToDrag(ShapeData newShape, GameSlot slot, Vector3 scale)
    {
        rendererTransform.localScale = scale;
        rendererTransform.position = Input.mousePosition;
        shapeRenderer.SetActive(true);

        gameShape.ConfigureShape(newShape.shapeType, newShape.shapeColor);

        dragging = true;
        startingSlot = slot;
    }

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

    public bool SetSecondarySlot(GameSlot newSlot)
    {
        if (newSlot != null && newSlot != startingSlot)
        {
            highlightedSecondarySlot = newSlot;
            return true;
        }

        return false;
    }

    public void ResetSecondarySlot()
    {
        highlightedSecondarySlot = null;
    }

    public void FinishDrag()
    {
        if (startingSlot != null && highlightedSecondarySlot != null)
        {
            GameManager.manager.SelectSlot(startingSlot);
            GameManager.manager.SelectSlot(highlightedSecondarySlot);
        }

        StopDisplayingDraggedShape();
    }
}
