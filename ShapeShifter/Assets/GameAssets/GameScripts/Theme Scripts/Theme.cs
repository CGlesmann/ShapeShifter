using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Theme : ScriptableObject
{
    public enum GeneralUIThemeKey { Button, HighlightButton, StaticPanel, ExitButton, ExitIcon, SettingsIcon, IndicatorNormal, IndicatorHighlight, EmptyStarIcon, CompletionStarIcon, PauseIcon, UndoIcon, LockIcon }
    public enum GameUIThemeKey { GameboardSlot, SelectedGameBoardSlot, SolutionboardSlot, LockIcon, LockSlot, SwitchLockIcon, DestructLockIcon, Transformer, TransformerFill, TransformerIcon, SolutionTransformer }
    public enum TextUIThemeKey { ButtonText, StaticPanelText, HighlightedText, TransformerText }
    public enum ColorMode { Default, Protanopia, Deuteranopia, Tritanopia }

    [HideInInspector] public ColorDictionary background = new ColorDictionary();

    [HideInInspector] public GeneralUIThemeDictionary generalUIThemeDictionary = new GeneralUIThemeDictionary();
    [HideInInspector] public GameUIThemeDictionary gameUIThemeDictionary = new GameUIThemeDictionary();
    [HideInInspector] public GameShapeThemeDictionary gameShapeThemeDictionary = new GameShapeThemeDictionary();
    [HideInInspector] public TextElementDictionary textElementDictionary = new TextElementDictionary();
}

[System.Serializable]
public class GeneralUIThemeDictionary
{
    [SerializeField] private List<Theme.GeneralUIThemeKey> keys;
    [SerializeField] private List<ThemeElementData> values;

    public void Add(Theme.GeneralUIThemeKey key, ThemeElementData value)
    {
        if (keys == null || values == null)
        {
            keys = new List<Theme.GeneralUIThemeKey>();
            values = new List<ThemeElementData>();

            keys.Add(key);
            values.Add(value);

            return;
        }

        if (!Contains(key, out int index))
        {
            keys.Add(key);
            values.Add(value);
        }

        return;
    }

    public void SetValue(int index, ThemeElementData newData)
    {
        if (values.Count - 1 > index || index < 0)
            return;

        ThemeElementData currentData = values[index];
        currentData.SetColorDictionary(newData.GetDictionary());
        currentData.SetSprite(newData.GetElementSprite());
    }

    public ThemeElementData GetElementData(Theme.GeneralUIThemeKey key)
    {
        if (Contains(key, out int index))
            return values[index];

        return null;
    }

    public ThemeElementData GetElementData(int index) { return values[index]; }

    public bool Contains(Theme.GeneralUIThemeKey targetKey, out int index)
    {
        if (keys == null || values == null)
        {
            index = -1;
            return false;
        }

        for(int i = 0; i < keys.Count; i++)
            if (keys[i] == targetKey)
            {
                index = i;
                return true;
            }

        index = -1;
        return false;
    }
}

[System.Serializable]
public class GameUIThemeDictionary
{
    [SerializeField] private List<Theme.GameUIThemeKey> keys;
    [SerializeField] private List<ThemeElementData> values;

    public void Add(Theme.GameUIThemeKey key, ThemeElementData value)
    {
        if (keys == null || values == null)
        {
            keys = new List<Theme.GameUIThemeKey>();
            values = new List<ThemeElementData>();

            keys.Add(key);
            values.Add(value);

            return;
        }

        if (!Contains(key, out int index))
        {
            keys.Add(key);
            values.Add(value);
        }

        return;
    }

    public void SetValue(int index, ThemeElementData newData)
    {
        if (values.Count - 1 > index || index < 0)
            return;

        ThemeElementData currentData = values[index];
        currentData.SetColorDictionary(newData.GetDictionary());
        currentData.SetSprite(newData.GetElementSprite());
    }

    public ThemeElementData GetElementData(Theme.GameUIThemeKey key)
    {
        if (Contains(key, out int index))
            return values[index];

        return null;
    }

    public ThemeElementData GetElementData(int index) { return values[index]; }

    public bool Contains(Theme.GameUIThemeKey targetKey, out int index)
    {
        if (keys == null || values == null)
        {
            index = -1;
            return false;
        }

        for (int i = 0; i < keys.Count; i++)
            if (keys[i] == targetKey)
            {
                index = i;
                return true;
            }

        index = -1;
        return false;
    }
}

[System.Serializable]
public class GameShapeThemeDictionary
{
    [SerializeField] private List<GameShape.ColorType> colorKeys;
    [SerializeField] private List<ColorDictionary> values;

    [SerializeField] private List<GameShape.ShapeType> shapeKeys;
    [SerializeField] private List<Sprite> shapeValues;

    #region Color Dictionary Methods
    public void Add(GameShape.ColorType key, ColorDictionary value)
    {
        if (colorKeys == null || values == null)
        {
            colorKeys = new List<GameShape.ColorType>();
            values = new List<ColorDictionary>();

            colorKeys.Add(key);
            values.Add(value);

            return;
        }

        if (!Contains(key, out int index))
        {
            colorKeys.Add(key);
            values.Add(value);
        }

        return;
    }

    public void SetValue(int index, ColorDictionary newData)
    {
        if (values.Count - 1 < index || index < 0)
            return;

        values[index] = newData;
    }

    public ColorDictionary GetElementData(GameShape.ColorType key)
    {
        if (Contains(key, out int index))
            return values[index];

        return null;
    }

    public bool Contains(GameShape.ColorType targetKey, out int index)
    {
        if (colorKeys == null || values == null)
        {
            index = -1;
            return false;
        }

        for (int i = 0; i < colorKeys.Count; i++)
            if (colorKeys[i] == targetKey)
            {
                index = i;
                return true;
            }

        index = -1;
        return false;
    }
    #endregion

    #region Sprite Dictionary Methods
    public void Add(GameShape.ShapeType key, Sprite value)
    {
        if (shapeKeys == null || shapeValues == null)
        {
            shapeKeys = new List<GameShape.ShapeType>();
            shapeValues = new List<Sprite>();

            shapeKeys.Add(key);
            shapeValues.Add(value);

            return;
        }

        if (!Contains(key, out int index))
        {
            shapeKeys.Add(key);
            shapeValues.Add(value);
        }

        return;
    }

    public void SetValue(int index, Sprite newData)
    {
        if (shapeValues.Count - 1 < index || index < 0)
        {
            Debug.LogError($"Trying to set a value that doesn't exist at index {index}");
            return;
        }

        shapeValues[index] = newData;
    }

    public Sprite GetElementData(GameShape.ShapeType key)
    {
        if (Contains(key, out int index))
            return shapeValues[index];

        return null;
    }

    public bool Contains(GameShape.ShapeType targetKey, out int index)
    {
        if (shapeKeys == null || shapeValues == null)
        {
            index = -1;
            return false;
        }

        for (int i = 0; i < shapeKeys.Count; i++)
            if (shapeKeys[i] == targetKey)
            {
                index = i;
                return true;
            }

        index = -1;
        return false;
    }
    #endregion
}

[System.Serializable]
public class TextElementDictionary
{
    [SerializeField] private List<Theme.TextUIThemeKey> keys;
    [SerializeField] private List<ColorDictionary> values;

    public void Add(Theme.TextUIThemeKey key, ColorDictionary value)
    {
        if (keys == null || values == null)
        {
            keys = new List<Theme.TextUIThemeKey>();
            values = new List<ColorDictionary>();

            keys.Add(key);
            values.Add(value);

            return;
        }

        if (!Contains(key, out int index))
        {
            keys.Add(key);
            values.Add(value);
        }

        return;
    }

    public void SetValue(int index, ColorDictionary newData)
    {
        if (values.Count - 1 > index || index < 0)
            return;

        values[index] = newData;
    }

    public ColorDictionary GetElementData(Theme.TextUIThemeKey key)
    {
        if (Contains(key, out int index))
            return values[index];

        return null;
    }

    public bool Contains(Theme.TextUIThemeKey targetKey, out int index)
    {
        if (keys == null || values == null)
        {
            index = -1;
            return false;
        }

        for (int i = 0; i < keys.Count; i++)
            if (keys[i] == targetKey)
            {
                index = i;
                return true;
            }

        index = -1;
        return false;
    }
}

[System.Serializable]
public class ThemeElementData
{
    [SerializeField] private Sprite elementSprite = null;
    [SerializeField] private Image.Type imageType = Image.Type.Sliced; 
    [SerializeField] private ColorDictionary elementColors = new ColorDictionary();

    public void SetSprite(Sprite newSprite) { elementSprite = newSprite; }
    public void SetColorDictionary(ColorDictionary newDictionary) { elementColors = new ColorDictionary(newDictionary); }
    public void SetSpriteType(Image.Type type) { imageType = type; }

    public Color GetColorByMode(Theme.ColorMode colorMode) { return elementColors.GetValue(colorMode); }
    public Sprite GetElementSprite() { return elementSprite; }
    public Image.Type GetSpriteType() { return imageType; }

    public ColorDictionary GetDictionary() { return elementColors; }
}

[System.Serializable]
public class ColorDictionary
{
    [SerializeField] private List<Theme.ColorMode> keys;
    [SerializeField] private List<Color> values;

    public ColorDictionary()
    {
        keys = new List<Theme.ColorMode>();
        values = new List<Color>();
    }

    public ColorDictionary(ColorDictionary reference)
    {
        keys = reference.GetKeyList();
        values = reference.GetValueList();
    }

    public void Add(Theme.ColorMode key, Color value)
    {
        if (keys == null || values == null)
        {
            keys = new List<Theme.ColorMode>();
            values = new List<Color>();

            keys.Add(key);
            values.Add(value);

            return;
        }


        if (!Contains(key, out int index))
        {
            keys.Add(key);
            values.Add(value);

            return;
        }
    }

    public Color GetValue(Theme.ColorMode key)
    {
        if (Contains(key, out int index))
            return values[index];

        return Color.white;
    }
    public Color GetValue(int index) { return values[index]; }

    public List<Theme.ColorMode> GetKeyList() { return keys; }
    public List<Color> GetValueList() { return values; }

    public void SetValue(int index, Color value)
    {
        if (keys == null || values == null || values.Count - 1 < index || values.Count == 0)
            return;

        values[index] = value;
        return;
    }

    public bool Contains(Theme.ColorMode key, out int index)
    {
        if (keys == null || keys.Count == 0)
        {
            index = -1;
            return false;
        }

        for(int i = 0; i < keys.Count; i++)
            if (keys[i] == key)
            {
                index = i;
                return true;
            }

        index = -1;
        return false;
    }
}

public abstract class ThemeElementLoader : MonoBehaviour
{
    [SerializeField] protected bool updateDynamically = true;

    public abstract void LoadElement(Theme theme, Theme.ColorMode colorMode);
}