using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShapeTransformer : MonoBehaviour, IPointerDownHandler
{
    public enum TransformerType { Shape, Color, None }

    [Header("Object References")]
    [SerializeField] private TextMeshProUGUI transformerCountText = null;
    [SerializeField] private Animator anim = null;

    private GameManager gameManager = null;
    private BoardManager boardManager = null;
    private UndoManager undoManager = null;

    private bool transforming = false;

    [Header("Transformer Settings")]
    [SerializeField] private TransformerData transformerData = null;
    private List<GameShape> shapesToTransform = null;

    private void Awake() { UpdateCounterText(); }
    private void Start()
    {
        gameManager = GameManager.manager;
        boardManager = BoardManager.boardManager;
        undoManager = UndoManager.undoManager;
    }

    public TransformerData GetTransformerData() { return transformerData; }
    public void SetTransformerData(TransformerData data)
    {
        if (data != null)
        {
            transformerData.transformerCounter = data.transformerCounter;
            transformerData.transformerType = data.transformerType;

            UpdateCounterText();
        } else
            transformerData.transformerCounter = 0;
    }

    private void UpdateCounterText() 
    { 
        if (transformerCountText != null && transformerData != null)
            transformerCountText.text = $"{transformerData.transformerCounter}"; 
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!transforming && transformerData.transformerCounter > 0)
            DisplayTransformAnimation();
    }

    public void TransformSurroundingShapes()
    {
        Transform gameBoardParent = gameManager.gameBoardParent;
        List<int> surroundingShapes = boardManager.CheckForSurroundingShapes(transform.GetSiblingIndex(), gameBoardParent);

        if (surroundingShapes != null && surroundingShapes.Count > 0)
        {
            undoManager.PushBoardData(new BoardData(gameBoardParent, new Vector2Int(boardManager.GetBoardWidth(), boardManager.GetBoardHeight())));
            shapesToTransform = new List<GameShape>();

            foreach (int shapeIndex in surroundingShapes)
            {
                GameShape gameShape = boardManager.GetGameSlot(shapeIndex, gameBoardParent)?.GetSlotShape();
                ShapeData shapeData = boardManager.GetBoardShapeData(shapeIndex, gameBoardParent);
                Image shapeImage = gameShape?.GetComponent<Image>();

                if (gameShape != null && shapeData != null && shapeImage != null)
                {
                    shapesToTransform.Add(gameShape);
                    GameShape.ShapeType shapeType = shapeData.shapeType;

                    int newShapeTypeIndex = (int)shapeType + 1;
                    if (newShapeTypeIndex > Enum.GetNames(typeof(GameShape.ShapeType)).Length - 1)
                        newShapeTypeIndex = 1;

                    GameShape.ShapeType newShapeType = (GameShape.ShapeType)newShapeTypeIndex;
                    gameShape.TriggerShapeTransform(newShapeType, gameShape.GetShapeColor());
                }
            }

            transformerData.transformerCounter--;
            UpdateCounterText();

            gameManager.CheckForVictory(gameManager.gameBoardParent, gameManager.solutionBoardParent);
        }

        transforming = false;
    }

    public void DisplayEnterAnimation() { anim.SetTrigger("Enter"); }
    public void DisplayExitAnimation() { anim.SetTrigger("Exit"); }
    public void DestroyTransformer() { GameObject.DestroyImmediate(gameObject); }
    public void DisplayTransformAnimation() { transforming = true; anim.SetTrigger("Transform"); }
}

[System.Serializable]
public class TransformerData
{
    public ShapeTransformer.TransformerType transformerType = ShapeTransformer.TransformerType.Color;
    public int transformerCounter = 0;

    public TransformerData()
    {
        transformerType = ShapeTransformer.TransformerType.Color;
        transformerCounter = 0;
    }

    public TransformerData(TransformerData reference)
    {
        this.transformerType = reference.transformerType;
        this.transformerCounter = reference.transformerCounter;
    }

    public TransformerData(ShapeTransformer.TransformerType type, int counter)
    {
        transformerType = type;
        transformerCounter = counter;
    }
}