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

    [Header("GUI References")]
    [SerializeField] private TextMeshProUGUI destroyText = null;

    /// <summary>
    /// Setting Default State
    /// </summary>
    private void Awake()
    {
        currentDestoryMethod = DestroyMethod.Shape;
        destroyText.text = "Destroy by Shape";
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

        // Reset each slot
        slot1.ResetSlotState();
        slot2.ResetSlotState();

        // Resetting Slot References
        slot1 = null;
        slot2 = null;
    }
}
