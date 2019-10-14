using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class GameShape : MonoBehaviour
{
    // Global Shape Type Enum
    [System.Serializable] public enum ShapeType { Square, Circle, Triangle, Diamond };

    [Header("Object References")]
    private GameManager manager = GameManager.manager;

    [Header("Component References")]
    [SerializeField] private SpriteRenderer shapeRenderer = null;
    [SerializeField] private Animator shapeAnimator = null;

    [Header("Shape Variables")]
    [SerializeField] private ShapeType shapeType = ShapeType.Square;
    [HideInInspector] public Color shapeColor => shapeRenderer.color;
    private bool markedForDestruct = false;

    public void Start() { manager = GameManager.manager; }

    public override string ToString()
    {
        return "Shape: " + shapeType.ToString() + " Color: " + shapeColor.ToString();
    }

    public override bool Equals(object other)
    {
        GameShape otherShape = (GameShape)other;
        if (otherShape != null)
            return (shapeType == otherShape.shapeType && shapeColor == otherShape.shapeColor);
        else
            return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// Automatically calls SetShapeType/Color
    /// </summary>
    /// <param name="type"></param>
    /// <param name="newColor"></param>
    public void ConfigureShape(ShapeType type, Color newColor)
    {
        // Setting the Type/Color
        SetShapeType(type);
        SetShapeColor(newColor);
    }

    /// <summary>
    /// Sets the type enum
    /// </summary>
    /// <param name="type"></param>
    public void SetShapeType(ShapeType type)
    {
        // Storing the shape type
        shapeType = type;

        // Setting the shape sprite
        if (shapeRenderer != null)
        {
            // Setting the Renderer Sprite
            switch (shapeType)
            {
                case ShapeType.Square:
                    shapeRenderer.sprite = ShapeSettings.SQUARE_SPRITE;
                    break;
                case ShapeType.Circle:
                    shapeRenderer.sprite = ShapeSettings.CIRCLE_SPRITE;
                    break;
                case ShapeType.Triangle:
                    shapeRenderer.sprite = ShapeSettings.TRIANGLE_SPRITE;
                    break;
                case ShapeType.Diamond:
                    shapeRenderer.sprite = ShapeSettings.DIAMOND_SPRITE;
                    break;
            };
        }
        else
            Debug.LogError("SpriteRenderer not set for " + name);
    }

    /// <summary>
    /// Setting the renderer color to the targetColor
    /// </summary>
    /// <param name="targetColor"></param>
    public void SetShapeColor(Color targetColor)
    {
        // Checking for a null reference
        if (shapeRenderer != null)
        {
            // Setting the Renderer Color
            shapeRenderer.color = targetColor;
        }
        else
            Debug.LogError("SpriteRenderer not set for " + name);
    }

    public void MarkForDestruction() { markedForDestruct = true; }
    public bool IsMarkedForDestruct() { return markedForDestruct; }

    /// <summary>
    /// Gets the current shape type
    /// </summary>
    /// <returns></returns>
    public ShapeType GetShapeType() { return shapeType; }

    /// <summary>
    /// Gets the current color of the shapeRenderer
    /// </summary>
    /// <returns></returns>
    public Color GetShapeColor() { return shapeRenderer.color; }

    /// <summary>
    /// Return the shapes data as a ShapeData struct
    /// </summary>
    /// <returns></returns>
    public ShapeData GetShapeData() { return new ShapeData(shapeColor, shapeType); }

    public void TriggerDestruction() { shapeAnimator.SetTrigger("Destroy"); }

    public void DestroyShape() { manager.shapesBeingDestroyed--;  Destroy(gameObject); }
}

public class ShapeData
{
    public Color shapeColor;
    public GameShape.ShapeType shapeType;

    public ShapeData(Color color, GameShape.ShapeType type)
    {
        shapeColor = color;
        shapeType = type;
    }

    public override string ToString()
    {
        return shapeColor.ToString() + shapeType.ToString();
    }

    public override bool Equals(object obj)
    {
        ShapeData other = (ShapeData)obj;
        return other.shapeColor == this.shapeColor && other.shapeType == this.shapeType;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameShape))]
public class ShapeInspector : Editor
{
    /// <summary>
    /// Draws Helper buttons for setting colors
    /// </summary>
    public override void OnInspectorGUI()
    {
        // Draws the base UI
        base.OnInspectorGUI();

        // Getting the target object
        GameShape shape = (GameShape)target;

        #region Color Setting Functions
        // Makes a section label
        GUILayout.Space(16f);
        GUILayout.Label("Color Setters", EditorStyles.boldLabel);

        // Sets the shape color to Blue according to the global manager reference
        if (GUILayout.Button("Set Color: Blue"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeColor(ShapeSettings.BLUE_COLOR);
            if (!EditorApplication.isPlaying)
                EditorUtility.SetDirty(shape);
        }

        // Sets the shape color to Red according to the global manager reference
        if (GUILayout.Button("Set Color: Red"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeColor(ShapeSettings.RED_COLOR);
            if (!EditorApplication.isPlaying)
                EditorUtility.SetDirty(shape);
        }

        // Sets the shape color to Green according to the global manager reference
        if (GUILayout.Button("Set Color: Green"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeColor(ShapeSettings.GREEN_COLOR);
            if (!EditorApplication.isPlaying)
                EditorUtility.SetDirty(shape);
        }

        // Sets the shape color to Yellow according to the global manager reference
        if (GUILayout.Button("Set Color: Yellow"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeColor(ShapeSettings.YELLOW_COLOR);
            if (!EditorApplication.isPlaying)
                EditorUtility.SetDirty(shape);
        }
        #endregion

        #region Shape Setting Functions
        // Makes a section label
        GUILayout.Space(16f);
        GUILayout.Label("Shape Setters", EditorStyles.boldLabel);

        // Sets the shape to Square according to the global manager reference
        if (GUILayout.Button("Set Shape: Square"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeType(GameShape.ShapeType.Square);
            if (!EditorApplication.isPlaying)
                EditorUtility.SetDirty(shape);
        }

        // Sets the shape to Circle according to the global manager reference
        if (GUILayout.Button("Set Shape: Circle"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeType(GameShape.ShapeType.Circle);
            if (!EditorApplication.isPlaying)
                EditorUtility.SetDirty(shape);
        }

        // Sets the shape to Triangle according to the global manager reference
        if (GUILayout.Button("Set Shape: Triangle"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeType(GameShape.ShapeType.Triangle);
            if (!EditorApplication.isPlaying)
                EditorUtility.SetDirty(shape);
        }

        // Sets the shape to Diamond according to the global manager reference
        if (GUILayout.Button("Set Shape: Diamond"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeType(GameShape.ShapeType.Diamond);
            if (!EditorApplication.isPlaying)
                EditorUtility.SetDirty(shape);
        }
        #endregion
    }
}
#endif