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

    [Header("Control Variables")]
    [SerializeField] private DestroyMethod currentDestoryMethod = DestroyMethod.Shape;

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
    #endregion

    #region Setter Functions
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
}
