using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class LevelGenerator
{
    public static GameManager gameManager = null;

    public static void GenerateGameboardShapes(GeneratorShapePrefs genPrefs)
    {
        if (gameManager == null)
            GetGameManager();

        Transform gameboardParent = gameManager.gameBoardParent;
        GenerateShapes(gameboardParent, genPrefs);
    }

    private static void GetGameManager() { gameManager = GameObject.FindObjectOfType<GameManager>(); }
    private static void GenerateShapes(Transform boardParent, GeneratorShapePrefs genPrefs)
    {
        // check for invalid input
        if (genPrefs == null || boardParent == null)
            return;

        // Generate list of available slots
        List<int> availableSlots = new List<int>();
        for (int s = 0; s < boardParent.childCount; s++)
            availableSlots.Add(s);

        // Generating the shapes
        List<ShapeData> shapesToCreate = genPrefs.GetShapeDataList();
        GameSlot targetGameSlot = null;
        int targetSlotIndex;

        foreach(ShapeData shapeData in shapesToCreate)
        {
            targetSlotIndex = Random.Range(0, availableSlots.Count - 1);
            targetGameSlot = boardParent.GetChild(availableSlots[targetSlotIndex]).GetComponent<GameSlot>();

            Debug.Log($"Generating {shapeData} at slot {targetSlotIndex}");
            GameSlotTools.CreateSlotShape(targetGameSlot.transform, shapeData.shapeType, shapeData.shapeColor, genPrefs.targetShapeScale);
            EditorUtility.SetDirty(targetGameSlot);

            availableSlots.RemoveAt(targetSlotIndex);
        }
    }
}

public class GeneratorShapePrefs
{
    public Dictionary<GameShape.ShapeType, int> shapes = new Dictionary<GameShape.ShapeType, int>();
    public Dictionary<GameShape.ColorType, int> colors = new Dictionary<GameShape.ColorType, int>();
    public float targetShapeScale = 0f;

    private List<GameShape.ShapeType> GetShapeTypesToGenerate()
    {
        List<GameShape.ShapeType> shapeTypes = new List<GameShape.ShapeType>();
        foreach (KeyValuePair<GameShape.ShapeType, int> shapeTypePair in shapes)
            for (int t = 0; t < shapeTypePair.Value; t++)
                shapeTypes.Add(shapeTypePair.Key);

        return shapeTypes;
    }
    private List<GameShape.ColorType> GetShapeColorsToGenerate()
    {
        List<GameShape.ColorType> shapeColors = new List<GameShape.ColorType>();
        foreach (KeyValuePair<GameShape.ColorType, int> colorPair in colors)
            for (int c = 0; c < colorPair.Value; c++)
                shapeColors.Add(colorPair.Key);

        return shapeColors;
    }

    public List<ShapeData> GetShapeDataList()
    {
        List<ShapeData> shapeDataList = new List<ShapeData>();
        List<GameShape.ShapeType> shapeTypes = GetShapeTypesToGenerate();
        List<GameShape.ColorType> shapeColors = GetShapeColorsToGenerate();

        int amountOfShapes = shapeTypes.Count;
        int targetShapeTypeIndex, targetShapeColorIndex;
        for(int i = 0; i < amountOfShapes; i++)
        {
            targetShapeTypeIndex = Random.Range(0, shapeTypes.Count - 1);
            targetShapeColorIndex = Random.Range(0, shapeColors.Count - 1);

            shapeDataList.Add(new ShapeData(shapeColors[targetShapeColorIndex], shapeTypes[targetShapeTypeIndex]));
            shapeColors.RemoveAt(targetShapeColorIndex);
            shapeTypes.RemoveAt(targetShapeTypeIndex);
        }

        Debug.Log($"Created {shapeDataList.Count} to generate");
        return shapeDataList;
    }

    public int GetRemainingShapes(int boardSize) { return boardSize - GetCurrentShapeCount(); }
    public int GetCurrentShapeCount()
    {
        int counter = 0;
        foreach (KeyValuePair<GameShape.ShapeType, int> pair in shapes)
            counter += pair.Value;

        return counter;
    }

    public int GetRemainingColors(int boardSize) { return boardSize - GetCurrentColorCount(); }
    public int GetCurrentColorCount()
    {
        int counter = 0;
        foreach (KeyValuePair<GameShape.ColorType, int> pair in colors)
            counter += pair.Value;

        return counter;
    }
}
