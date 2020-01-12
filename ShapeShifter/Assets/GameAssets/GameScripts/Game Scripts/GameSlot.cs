using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GameSlot : MonoBehaviour
{
    [Header("Object References")]
    private GameManager manager = GameManager.manager;
    private SpriteRenderer slotRenderer => GetComponent<SpriteRenderer>();

    // private properties
    private Transform childShape => GetSlotShapeTransform();
    private GameShape slotShape => childShape != null ? childShape.GetComponent<GameShape>() : null;

    [Header("Control Variables")]
    [SerializeField] private bool canSelect = true;
    [SerializeField] private Color deselectedColor = Color.white;
    [SerializeField] private Color selectedColor = Color.white;
    private bool selected = false;
    private int slotIndex = -1;

    /// <summary>
    /// Setting the static manager reference
    /// </summary>
    public void Start()
    {
        manager = GameManager.manager;
    }

    #if UNITY_EDITOR || UNITY_STANDALONE_WIN
    /// <summary>
    /// Checks for tap input
    /// </summary>
    private void OnMouseDown()
    {
        // Checking whether or not the slot can be selected
        if (!GameState.gamePaused && canSelect)
        {
            // Checking for mouse input, toggle select if true
            ToggleSelect();
        }
    }
    #endif

    #if UNITY_IOS || UNITY_ANDROID
    /// <summary>
    /// Checking for touch input on mobile devices
    /// </summary>
    private void Update()
    {
        // Checking for touch input
        if (!GameState.gamePaused && canSelect && Input.touchCount == 1)
        {
            // Getting Touch Data
            Touch inputData = Input.GetTouch(0);

            // Checking for the appropiate touch phase
            if (inputData.phase == TouchPhase.Ended)
            {
                // Getting the touch point
                Vector2 wp = Camera.main.ScreenToWorldPoint(inputData.position);

                // Checking for a collision
                if (GetComponent<Collider2D>() == Physics2D.OverlapPoint(wp))
                    ToggleSelect();
            }
        }
    }
    #endif

    public Transform GetSlotShapeTransform()
    {
        Transform t;
        for(int i = 0; i < transform.childCount; i++)
        {
            t = transform.GetChild(0);
            if (t.GetComponent<GameShape>() != null)
            {
                return t;
            }
        }

        return null;
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
        Transform t = GetSlotShapeTransform();
        if (t != null)
        {
            Debug.Log("Found shape");
            return t.GetComponent<GameShape>();
        }
        else
        {
            Debug.Log("Couldn't Find shape");
            return null;
        }
    }

    /// <summary>
    /// Selects the slot if unselected, otherwise unselects the slot
    /// </summary>
    public void ToggleSelect()
    {
        // Slot is selected, therefore deselect the Slot
        if (selected)
        {
            slotRenderer.color = deselectedColor;
            selected = false;
            manager.DeselectSlot();
        }
        else if (slotShape != null) {
            // Slot isn't selected, therefore select the Slot
            selected = manager.SelectSlot(this);
            if (selected)
                slotRenderer.color = selectedColor;
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
        // Resetting Slot State
        selected = false;
        slotRenderer.color = deselectedColor;
    }

    public void DisableSlotSelection() { canSelect = false; }
    public void EnableSlotSelection() { canSelect = true; }
}
