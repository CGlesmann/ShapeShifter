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
    [System.Serializable] public enum ColorType { Red, Blue, Green, Yellow}

    [Header("Object References")]
    private GameManager manager = GameManager.manager;

    [Header("Component References")]
    [SerializeField] private Animator anim = null;
    [SerializeField] private Image imageRenderer = null;

    [Header("Shape Variables")]
    [SerializeField] private ShapeType shapeType = ShapeType.Square;
    [SerializeField] public ColorType colorType = ColorType.Red;

    private Vector3 baseSize = Vector3.zero;
    private bool markedForDestruct = false;

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

    public void ConfigureShape(ShapeType type, ColorType newColor) { SetShapeType(type); SetShapeColor(newColor); }
    public void SetShapeType(ShapeType type)
    {
        shapeType = type;
        imageRenderer.sprite = ShapeSettings.GetShapeSprite(type);
    }

    public void SetShapeColor(ColorType type)
    {
        colorType = type;
        imageRenderer.color = ShapeSettings.GetShapeColor(type);
    }

    public void MarkForDestruction() { markedForDestruct = true; }
    public bool IsMarkedForDestruct() { return markedForDestruct; }

    public ShapeType GetShapeType() { return shapeType; }
    public ColorType GetShapeColor() { return colorType; }
    public ShapeData GetShapeData() { return new ShapeData(colorType, shapeType); }

    public void TriggerDestruction() { /*StartCoroutine(DisplayShapeDestruction())*/anim.SetTrigger("Destroy"); }
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
        transform.parent.GetComponent<GameSlot>().SetSlotShapeReference(null);
        Destroy(gameObject);

        BoardManager.boardManager.MarkShapeAsDestroyed();
    }

    public void DestroyShapeImmediate()
    {
        transform.parent.GetComponent<GameSlot>().SetSlotShapeReference(null);
        DestroyImmediate(gameObject);
    }
}

public class ShapeData
{
    public GameShape.ColorType shapeColor;
    public GameShape.ShapeType shapeType;

    public ShapeData(GameShape.ColorType color, GameShape.ShapeType type) { shapeColor = color; shapeType = type; }
    public override string ToString() { return shapeColor.ToString() + shapeType.ToString(); }
    public override int GetHashCode() { return base.GetHashCode(); }

    public override bool Equals(object obj)
    {
        ShapeData other = (ShapeData)obj;
        return other.shapeColor == this.shapeColor && other.shapeType == this.shapeType;
    }
}