using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum DestroyMethod { Shape, Color };

    [Header("Global Color Constants")]
    [SerializeField] private Color BLUE_COLOR = Color.blue;
    [SerializeField] private Color RED_COLOR = Color.red;
    [SerializeField] private Color GREEN_COLOR = Color.green;
    [SerializeField] private Color YELLOW_COLOR = Color.yellow;

    [Header("Global Shape Constants")]
    [SerializeField] private Sprite SQUARE_SPRITE = null;
    [SerializeField] private Sprite CIRCLE_SPRITE = null;
    [SerializeField] private Sprite TRIANGLE_SPRITE = null;
    [SerializeField] private Sprite DIAMOND_SPRITE = null;

    [Header("Control Variables")]
    [SerializeField] private DestroyMethod currentDestoryMethod = DestroyMethod.Shape;
    private GameSlot slot1 = null, slot2 = null; // References for selected slots

    [Header("Board Settings")]
    [SerializeField] private int boardWidth = 4;
    [SerializeField] private int boardHeight = 4;

    [Header("Object References")]
    [SerializeField] private Transform gameBoardParent = null;
    [SerializeField] private Transform solutionBoardParent = null;

    [Header("GUI References")]
    [SerializeField] private TextMeshProUGUI destroyText = null;

    /// <summary>
    /// Setting Default State
    /// </summary>
    private void Awake()
    {
        currentDestoryMethod = DestroyMethod.Shape;
        destroyText.text = "Destroy by Shape";

        SetGameSlotIndexes();
    }

    #region Getter Functions
    /// <summary>
    /// Returns the Global Blue Color
    /// </summary>
    /// <returns></returns>
    public Color GetBlueColor() { return BLUE_COLOR; }

    /// <summary>
    /// Returns the Global Red Color
    /// </summary>
    /// <returns></returns>
    public Color GetRedColor() { return RED_COLOR; }

    /// <summary>
    /// Returns the Global Green Color
    /// </summary>
    /// <returns></returns>
    public Color GetGreenColor() { return GREEN_COLOR; }

    /// <summary>
    /// Returns the Global Yellow Color
    /// </summary>
    /// <returns></returns>
    public Color GetYellowColor() { return YELLOW_COLOR; }

    /// <summary>
    /// Returns the global square sprite
    /// </summary>
    /// <returns></returns>
    public Sprite GetSqureSprite() { return SQUARE_SPRITE; }

    /// <summary>
    /// Returns the global circle sprite
    /// </summary>
    /// <returns></returns>
    public Sprite GetCircleSprite() { return CIRCLE_SPRITE; }

    /// <summary>
    /// Returns the global triangle sprite
    /// </summary>
    /// <returns></returns>
    public Sprite GetTriangleSprite() { return TRIANGLE_SPRITE; }

    /// <summary>
    /// Returns the global diamond sprite
    /// </summary>
    /// <returns></returns>
    public Sprite GetDiamondSprite() { return DIAMOND_SPRITE; }

    public GameSlot GetGameBoardslot(int slotIndex) { return gameBoardParent.GetChild(slotIndex).GetComponent<GameSlot>(); }
    #endregion

    #region Setter Functions
    /// <summary>
    /// Selects a slot, switches the shapes if two slots are selected
    /// </summary>
    /// <param name="targetSlot"></param>
    public void SelectSlot(GameSlot targetSlot)
    {
        // Checking for which reference
        if (slot1 == null)
            slot1 = targetSlot;
        else if (slot2 == null)
        {
            // Two Slots Selected, switch the shapes
            slot2 = targetSlot;
            SwitchShapes();
        }
    }

    /// <summary>
    /// Deselects the currently selected shape
    /// This can only be called when one shape is selected
    /// Therefore this always reset shape1 reference
    /// </summary>
    public void DeselectSlot() { slot1 = null; }

    /// <summary>
    /// Switches the current destroy method to the opposite method
    /// Used for toggle button
    /// </summary>
    public void ToggleDestoryMethod()
    {
        switch(currentDestoryMethod)
        {
            case DestroyMethod.Shape:
                currentDestoryMethod = DestroyMethod.Color;
                destroyText.text = "Destroy by Color";
                break;
            case DestroyMethod.Color:
                currentDestoryMethod = DestroyMethod.Shape;
                destroyText.text = "Destroy by Shape";
                break;
            default:
                destroyText.text = "Unknown Destroy Method";
                break;
        }
    }
    #endregion

    /// <summary>
    /// Sets all of the slot indexes
    /// </summary>
    public void SetGameSlotIndexes()
    {
        // Declaring the temp store variable
        GameSlot slot = null;

        // Setting the indexes on the game board
        if (gameBoardParent != null)
        {
            for (int i = 0; i < gameBoardParent.childCount; i++)
            {
                slot = gameBoardParent.GetChild(i).GetComponent<GameSlot>();

                Debug.Log("Setting " + slot.name + " to " + i);
                slot.SetSlotIndex(i);
            }
        }

        // Setting the indexes on the solution board
        if (solutionBoardParent != null)
        {
            for (int i = 0; i < solutionBoardParent.childCount; i++)
            {
                slot = solutionBoardParent.GetChild(i).GetComponent<GameSlot>();
                slot.SetSlotIndex(i);
            }
        }
    }

    /// <summary>
    /// Switches the two slot shapes and resets the slot states
    /// </summary>
    public void SwitchShapes()
    {
        Debug.Log("switching " + slot1.name + " and " + slot2.name);

        // Get the shape references and store in temp veriables
        GameShape.ShapeType shape1Type = slot1.GetSlotShape().GetShapeType(), shape2Type = slot2.GetSlotShape().GetShapeType();
        Color shape1Color = slot1.GetSlotShape().GetShapeColor(), shape2Color = slot2.GetSlotShape().GetShapeColor();

        // Sets each of the slots shapes
        slot1.SetSlotShape(shape2Type, shape2Color);
        slot2.SetSlotShape(shape1Type, shape1Color);

        // Triggering Shape Destruction(s)
        TriggerShapeDestruction(slot1.GetSlotIndex());
        TriggerShapeDestruction(slot2.GetSlotIndex());

        // Reset each slot
        slot1.ResetSlotState();
        slot2.ResetSlotState();

        // Resetting Slot References
        slot1 = null;
        slot2 = null;
    }

    /// <summary>
    /// Checking for a valid match
    /// </summary>
    /// <param name="shape1"></param>
    /// <param name="shape2"></param>
    /// <returns></returns>
    public bool CheckForMatch(GameShape shape1, GameShape shape2)
    {
        Debug.Log("Shape 1: " + shape1 + " Shape2: " + shape2);
        switch(currentDestoryMethod)
        {
            case DestroyMethod.Shape:
                return (shape1.GetShapeType() == shape2.GetShapeType());
            case DestroyMethod.Color:
                return (shape1.GetShapeColor() == shape2.GetShapeColor());
            default:
                return false;
        }
    }

    /// <summary>
    /// Triggers a destruction of shapes around the given index
    /// </summary>
    /// <param name="index"></param>
    public void TriggerShapeDestruction(int index)
    {
        // Getting the current slot
        GameSlot centerSlot = gameBoardParent.GetChild(index).GetComponent<GameSlot>();
        if (centerSlot == null)
        {
            Debug.LogError("Couldn't find slot when triggering destruction at slot " + index);
            return;
        }

        // Declaring temp storage variable
        bool destoryCurrentSlot = false;
        GameSlot slot;
        
        // Top left slot
        if (index % boardWidth != 0 && index - (boardWidth + 1) >= 0 && GetGameBoardslot(index - (boardWidth + 1)) != null)
        {
            // Getting Slot Reference
            slot = GetGameBoardslot(index - (boardWidth + 1));

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null && CheckForMatch(centerSlot.GetSlotShape(), slot.GetSlotShape()))
            {
                slot.DestroyShape();
                destoryCurrentSlot = true;
            }
        }

        // Top Center Slot
        if (index >= boardWidth && index - (boardWidth + 0) >= 0 && GetGameBoardslot(index - (boardWidth + 0)) != null)
        {
            // Getting Slot Reference
            slot = GetGameBoardslot(index - (boardWidth + 0));

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null && CheckForMatch(centerSlot.GetSlotShape(), slot.GetSlotShape()))
            {
                slot.DestroyShape();
                destoryCurrentSlot = true;
            }
        }

        // Top Right Slot
        if (index + 1 % boardWidth != 0 && index - (boardWidth - 1) >= 0 && GetGameBoardslot(index - (boardWidth - 1)) != null)
        {
            // Getting Slot Reference
            slot = GetGameBoardslot(index - (boardWidth - 1));

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null && CheckForMatch(centerSlot.GetSlotShape(), slot.GetSlotShape()))
            {
                slot.DestroyShape();
                destoryCurrentSlot = true;
            }
        }

        // Middle Left Slot (NOT COMPLETE)
        if (index + 1 % boardWidth != 0 && index - (boardWidth - 1) >= 0 && GetGameBoardslot(index - (boardWidth - 1)) != null)
        {
            // Getting Slot Reference
            slot = GetGameBoardslot(index - (boardWidth - 1));

            // Checking for destorying the shape
            if (slot.GetSlotShape() != null && CheckForMatch(centerSlot.GetSlotShape(), slot.GetSlotShape()))
            {
                slot.DestroyShape();
                destoryCurrentSlot = true;
            }
        }

        // Center Slot
        if (destoryCurrentSlot)
            GameObject.Destroy(centerSlot.GetSlotShape().gameObject);
    }
}
