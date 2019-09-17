using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShapeSettings
{
    [Header("Shape Sprites")]
    public static Sprite SQUARE_SPRITE = Resources.Load<Sprite>("ShapeSprites/Square");
    public static Sprite CIRCLE_SPRITE = Resources.Load<Sprite>("ShapeSprites/Circle");
    public static Sprite TRIANGLE_SPRITE = Resources.Load<Sprite>("ShapeSprites/Triangle");
    public static Sprite DIAMOND_SPRITE = Resources.Load<Sprite>("ShapeSprites/Diamond");

    [Header("Shape Colors")]
    public static Color RED_COLOR = Color.red;
    public static Color BLUE_COLOR = Color.blue;
    public static Color YELLOW_COLOR = Color.yellow;
    public static Color GREEN_COLOR = Color.green;
}
