using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Settings Config", menuName = "Global Assets/New Shape Settings", order = 1)]
public class ShapeSettingsConfig : ScriptableObject
{
    [SerializeField, HideInInspector] public ShapeSpriteDictionary shapeSprites = new ShapeSpriteDictionary();
    [SerializeField, HideInInspector] public ShapeColorDictionary shapeColors = new ShapeColorDictionary();
}

[System.Serializable]
public class ShapeColorDictionary
{
    [SerializeField] private List<GameShape.ColorType> keys = new List<GameShape.ColorType>();
    [SerializeField] private List<Color> values = new List<Color>();

    private bool ContainsKey(GameShape.ColorType key, out int index)
    {
        for(int i = 0; i < keys.Count; i++)
        {
            if (keys[i] == key)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    public Color GetValue(GameShape.ColorType key)
    {
        if (ContainsKey(key, out int index))
            return values[index];

        return Color.white;
    }

    public void SetValue(GameShape.ColorType key, Color value)
    {
        if (ContainsKey(key, out int index))
            values[index] = value;
        else
        {
            keys.Add(key);
            values.Add(value);
        }
    }
}

[System.Serializable]
public class ShapeSpriteDictionary
{
    [SerializeField] private List<GameShape.ShapeType> keys = new List<GameShape.ShapeType>();
    [SerializeField] private List<Sprite> values = new List<Sprite>();

    private bool ContainsKey(GameShape.ShapeType key, out int index)
    {
        for (int i = 0; i < keys.Count; i++)
        {
            if (keys[i] == key)
            {
                index = i;
                return true;
            }
        }

        index = -1;
        return false;
    }

    public Sprite GetValue(GameShape.ShapeType key)
    {
        if (ContainsKey(key, out int index))
            return values[index];

        return null;
    }

    public void SetValue(GameShape.ShapeType key, Sprite value)
    {
        if (ContainsKey(key, out int index))
            values[index] = value;
        else
        {
            keys.Add(key);
            values.Add(value);
        }
    }
}