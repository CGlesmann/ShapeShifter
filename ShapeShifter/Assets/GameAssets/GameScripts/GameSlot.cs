using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSlot : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameManager manager = null;
    private SpriteRenderer slotRenderer => GetComponent<SpriteRenderer>();

    // private properties
    private Transform childShape => transform.childCount > 0 ? transform.GetChild(0) : null;
    private GameShape slotShape => childShape != null ? childShape.GetComponent<GameShape>() : null;

    [Header("Control Variables")]
    [SerializeField] private bool selected = false;
    [SerializeField] private Color deselectedColor = Color.white;
    [SerializeField] private Color selectedColor = Color.white;

    private int slotIndex = -1;

    /// <summary>
    /// Checks for tap input
    /// </summary>
    public void OnMouseDown()
    {
        // Checking for mouse input, toggle select if true
        ToggleSelect();
    }

    /// <summary>
    /// Sets the shape of the current slot
    /// </summary>
    /// <param name="type"></param>
    /// <param name="targetColor"></param>
    public void SetSlotShape(GameShape.ShapeType type, Color targetColor)
    {
        if (slotShape != null)
            slotShape.ConfigureShape(type, targetColor);
    }

    /// <summary>
    /// Sets the index of the current slot
    /// </summary>
    /// <param name="index"></param>
    public void SetSlotIndex(int index) { slotIndex = index; }

    /// <summary>
    /// Returns the current slot index
    /// </summary>
    public int GetSlotIndex() { return slotIndex; }

    /// <summary>
    /// Returns the current shape reference or null if no shape is present
    /// </summary>
    /// <returns></returns>
    public GameShape GetSlotShape()
    {
        if (transform.childCount > 0)
            return transform.GetChild(0).GetComponent<GameShape>();
        else
            return null;
    }

    /// <summary>
    /// Selects the slot if unselected, otherwise unselects the slot
    /// </summary>
    public void ToggleSelect()
    {
        // Slot is selected, therefore deselect the Slot
        if (selected)
        {
            Debug.Log("Deselecting " + name);

            slotRenderer.color = deselectedColor;
            selected = false;
            manager.DeselectSlot();
        } else if (slotShape != null) {
            Debug.Log("Selecting " + name);

            // Slot isn't selected, therefore select the Slot
            slotRenderer.color = selectedColor;
            selected = true;
            manager.SelectSlot(this);
        }
    }

    /// <summary>
    /// Called by the game manager upon switching two shapes
    /// </summary>
    public void DestroyShape() { GameObject.Destroy(slotShape.gameObject); }

    /// <summary>
    /// Sets Selected to false, called when
    /// </summary>
    public void ResetSlotState()
    {
        // Debug Message
        Debug.Log("Resetting" + name);

        // Resetting Slot State
        selected = false;
        slotRenderer.color = deselectedColor;
    }
}
