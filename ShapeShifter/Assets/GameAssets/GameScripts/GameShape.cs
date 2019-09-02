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
    public enum ShapeType { Square, Circle, Triangle, Diamond };

    [Header("Object References")]
    public GameManager manager = null;

    [Header("Component References")]
    [SerializeField] private SpriteRenderer shapeRenderer = null;

    [Header("Shape Variables")]
    [SerializeField] private ShapeType shapeType = ShapeType.Square;
    [HideInInspector] public Color shapeColor => shapeRenderer.color;

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
                    shapeRenderer.sprite = manager.GetSqureSprite();
                    break;
                case ShapeType.Circle:
                    shapeRenderer.sprite = manager.GetCircleSprite();
                    break;
                case ShapeType.Triangle:
                    shapeRenderer.sprite = manager.GetTriangleSprite();
                    break;
                case ShapeType.Diamond:
                    shapeRenderer.sprite = manager.GetDiamondSprite();
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
            shape.SetShapeColor(shape.manager.GetBlueColor());
            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // Sets the shape color to Red according to the global manager reference
        if (GUILayout.Button("Set Color: Red"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeColor(shape.manager.GetRedColor());
            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // Sets the shape color to Green according to the global manager reference
        if (GUILayout.Button("Set Color: Green"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeColor(shape.manager.GetGreenColor());
            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // Sets the shape color to Yellow according to the global manager reference
        if (GUILayout.Button("Set Color: Yellow"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeColor(shape.manager.GetYellowColor());
            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
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
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // Sets the shape to Circle according to the global manager reference
        if (GUILayout.Button("Set Shape: Circle"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeType(GameShape.ShapeType.Circle);
            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // Sets the shape to Triangle according to the global manager reference
        if (GUILayout.Button("Set Shape: Triangle"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeType(GameShape.ShapeType.Triangle);
            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // Sets the shape to Diamond according to the global manager reference
        if (GUILayout.Button("Set Shape: Diamond"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeType(GameShape.ShapeType.Diamond);
            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        #endregion
    }
}
#endif