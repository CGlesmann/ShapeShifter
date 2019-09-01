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
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameShape))]
public class ShapeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        // Draws the base UI
        base.OnInspectorGUI();

        // Getting the target object
        GameShape shape = (GameShape)target;

        GUILayout.Space(16f);
        GUILayout.Label("Color Setters", EditorStyles.boldLabel);

        // Drawing Color Toggle Buttons
        if (GUILayout.Button("Set Color: Blue"))
        {
            shape.SetShapeColor(shape.manager.GetBlueColor());
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        if (GUILayout.Button("Set Color: Red"))
        {
            shape.SetShapeColor(shape.manager.GetRedColor());
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        if (GUILayout.Button("Set Color: Green"))
        {
            shape.SetShapeColor(shape.manager.GetGreenColor());
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        if (GUILayout.Button("Set Color: Yellow"))
        {
            shape.SetShapeColor(shape.manager.GetYellowColor());
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}
#endif