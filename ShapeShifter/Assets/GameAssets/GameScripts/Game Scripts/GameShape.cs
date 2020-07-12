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
    [System.Serializable] public enum ShapeType { None, Square, Circle, Triangle, Diamond };
    [System.Serializable] public enum ColorType { None, Red, Blue, Green, Yellow}

    [Header("Object References")]
    [SerializeField] private ShapeThemeElement shapeThemeElement = null;
    private GameManager manager = GameManager.manager;

    [Header("Component References")]
    [SerializeField] private Animator anim = null;

    [Header("Shape Variables")]
    [SerializeField] private ShapeType shapeType = ShapeType.Square;
    [SerializeField] public ColorType colorType = ColorType.Red;

    private Vector3 baseSize = Vector3.zero;
    private bool markedForDestruct = false;

    private ShapeData queuedTransformData = null;
    private bool transforming = false;

    public void Start() { manager = GameManager.manager; baseSize = transform.localScale; }
    public override string ToString() { return "Shape: " + shapeType.ToString() + " Color: " + colorType.ToString(); }
    public override int GetHashCode() { return base.GetHashCode(); }

    public override bool Equals(object other)
    {
        GameShape otherShape = (GameShape)other;
        if (otherShape != null)
            return (shapeType == otherShape.shapeType && colorType == otherShape.colorType);
        else
            return false;
    }

    public void ConfigureShape(ShapeType type, ColorType newColor)
    {
        shapeType = type;
        colorType = newColor;

        shapeThemeElement.LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    public void SetShapeType(ShapeType type)
    {
        shapeType = type;
        shapeThemeElement.LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    public void SetShapeColor(ColorType type)
    {
        colorType = type;
        shapeThemeElement.LoadElement(ThemeManager.GetCurrentTheme(), ThemeManager.GetCurrentColorMode());
    }

    public void MarkForDestruction() { markedForDestruct = true; }
    public bool IsMarkedForDestruct() { return markedForDestruct; }

    public ShapeType GetShapeType() { return shapeType; }
    public ColorType GetShapeColor() { return colorType; }
    public ShapeData GetShapeData() { return new ShapeData(colorType, shapeType); }

    public void TriggerDestruction() { anim.SetTrigger("Destroy"); }

    public void DestroyShape()
    {
        transform.parent.GetComponent<GameSlot>().SetSlotShapeReference(null);
        Destroy(gameObject);

        BoardManager.boardManager.MarkShapeAsDestroyed();
    }

    public void DestroyShapeImmediate()
    {
        transform.parent.GetComponent<GameSlot>().SetSlotShapeReference(null);
        DestroyImmediate(gameObject);
    }

    public void TriggerShapeTransform(ShapeData newData)
    {
        queuedTransformData = newData;
        transforming = true;

        anim.SetTrigger("Transform");
    }

    public void TriggerShapeTransform(GameShape.ShapeType shapeType, GameShape.ColorType colorType)
    {
        queuedTransformData = new ShapeData(colorType, shapeType);
        transforming = true;

        anim.SetTrigger("Transform");
    }

    public void ExecuteShapeTransform()
    {
        ConfigureShape(queuedTransformData.shapeType, queuedTransformData.shapeColor);
        transforming = false;
    }
}

# if UNITY_EDITOR
[CustomEditor(typeof(GameShape))]
public class ShapeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameShape shape = (GameShape)target;

        if (GUILayout.Button("Update Shape Sprite/Color"))
        {
            shape.ConfigureShape(shape.GetShapeType(), shape.GetShapeColor());
            EditorUtility.SetDirty(shape.GetComponent<Image>());
        }
    }

    [MenuItem("Update Shape Sprite/Color", menuItem = "Level Tools/Update Shape S&C")]
    public static void UpdateAllShapes()
    {
        GameShape[] shapes = GameObject.FindObjectsOfType<GameShape>();
        foreach (GameShape shape in shapes)
        {
            shape.ConfigureShape(shape.GetShapeType(), shape.GetShapeColor());
            EditorUtility.SetDirty(shape.GetComponent<Image>());
        }
    }
}
#endif

[System.Serializable]
public class ShapeData
{
    public GameShape.ColorType shapeColor;
    public GameShape.ShapeType shapeType;

    public ShapeData(GameShape.ColorType color, GameShape.ShapeType type) { shapeColor = color; shapeType = type; }
    public override string ToString() { return $"{shapeColor.ToString()} {shapeType.ToString()}"; }
    public override int GetHashCode() { return base.GetHashCode(); }

    public override bool Equals(object obj)
    {
        ShapeData other = (ShapeData)obj;
        return other.shapeColor == this.shapeColor && other.shapeType == this.shapeType;
    }
}