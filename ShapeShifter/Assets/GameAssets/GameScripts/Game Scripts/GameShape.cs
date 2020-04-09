using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Image imageRenderer = null;

    [Header("Shape Variables")]
    [SerializeField] private ShapeType shapeType = ShapeType.Square;
    [HideInInspector] public Color shapeColor => imageRenderer.color;

    private Vector3 baseSize = Vector3.zero;
    private bool markedForDestruct = false;

    public void Start() { manager = GameManager.manager; baseSize = transform.localScale; Debug.Log("BaseSize: " + baseSize.ToString()); }
    public override string ToString() { return "Shape: " + shapeType.ToString() + " Color: " + shapeColor.ToString(); }
    public override int GetHashCode() { return base.GetHashCode(); }

    public override bool Equals(object other)
    {
        GameShape otherShape = (GameShape)other;
        if (otherShape != null)
            return (shapeType == otherShape.shapeType && shapeColor == otherShape.shapeColor);
        else
            return false;
    }

    public void ConfigureShape(ShapeType type, Color newColor)
    {
        // Setting the Type/Color
        SetShapeType(type);
        SetShapeColor(newColor);
    }

    public void SetShapeType(ShapeType type)
    {
        shapeType = type;

        if (imageRenderer != null)
        {
            switch (shapeType)
            {
                case ShapeType.Square:
                    imageRenderer.sprite = ShapeSettings.SQUARE_SPRITE;
                    break;
                case ShapeType.Circle:
                    imageRenderer.sprite = ShapeSettings.CIRCLE_SPRITE;
                    break;
                case ShapeType.Triangle:
                    imageRenderer.sprite = ShapeSettings.TRIANGLE_SPRITE;
                    break;
                case ShapeType.Diamond:
                    imageRenderer.sprite = ShapeSettings.DIAMOND_SPRITE;
                    break;
            };
        }
    }

    public void SetShapeColor(Color targetColor)
    {
        if (imageRenderer != null)
            imageRenderer.color = targetColor;
    }

    public void MarkForDestruction() { markedForDestruct = true; }
    public bool IsMarkedForDestruct() { return markedForDestruct; }

    public ShapeType GetShapeType() { return shapeType; }
    public Color GetShapeColor() { return imageRenderer.color; }
    public ShapeData GetShapeData() { return new ShapeData(shapeColor, shapeType); }

    public void TriggerDestruction() { StartCoroutine(DisplayShapeDestruction());}
    private IEnumerator DisplayShapeDestruction()
    {
        float counter = 0;
        while (counter < 1)
        {
            counter += Time.deltaTime * 2f;

            transform.localScale = Vector3.Lerp(baseSize, Vector3.zero, counter);
            yield return null;
        }

        DestroyShape();
    }

    public void DestroyShape()
    {
        if (manager != null)
            manager.shapesBeingDestroyed--;

        transform.parent.GetComponent<GameSlot>().SetSlotShapeReference(null);
        DestroyImmediate(gameObject);
    }

    public void DestroyShapeImmediate()
    {
        transform.parent.GetComponent<GameSlot>().SetSlotShapeReference(null);
        DestroyImmediate(gameObject);
    }
}

public class ShapeData
{
    public Color shapeColor;
    public GameShape.ShapeType shapeType;

    public ShapeData(Color color, GameShape.ShapeType type) { shapeColor = color; shapeType = type; }
    public override string ToString() { return shapeColor.ToString() + shapeType.ToString(); }
    public override int GetHashCode() { return base.GetHashCode(); }

    public override bool Equals(object obj)
    {
        ShapeData other = (ShapeData)obj;
        return other.shapeColor == this.shapeColor && other.shapeType == this.shapeType;
    }
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
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // Sets the shape color to Red according to the global manager reference
        if (GUILayout.Button("Set Color: Red"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeColor(ShapeSettings.RED_COLOR);
            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // Sets the shape color to Green according to the global manager reference
        if (GUILayout.Button("Set Color: Green"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeColor(ShapeSettings.GREEN_COLOR);
            if (!EditorApplication.isPlaying)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        // Sets the shape color to Yellow according to the global manager reference
        if (GUILayout.Button("Set Color: Yellow"))
        {
            // Sets the color, makrs the scene as dirty for save
            shape.SetShapeColor(ShapeSettings.YELLOW_COLOR);
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