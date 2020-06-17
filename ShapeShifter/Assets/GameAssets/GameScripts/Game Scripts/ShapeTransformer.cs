using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShapeTransformer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum TransformerType { Shape, Color }

    [Header("Object References")]
    [SerializeField] private Image transformerIconFill = null;
    [SerializeField] private TextMeshProUGUI transformerCountText = null;
    private GameManager gameManager = null;
    private BoardManager boardManager = null;
    private UndoManager undoManager = null;

    [Header("Transformer Settings")]
    [SerializeField] private TransformerData transformerData = null;

    private List<GameShape> shapesToTransform = null;

    [Header("Input Settings")]
    [SerializeField] private float inputTime = 1f;
    [SerializeField] private bool isHoldingDown = false;

    private BoardData cachedBoardData = null;
    private bool previewingTransformation = false;
    private bool bypassEndHolding = false;
    private float inputTimer;

    private void Awake() { UpdateCounterText(); }
    private void Start()
    {
        gameManager = GameManager.manager;
        boardManager = BoardManager.boardManager;
        undoManager = UndoManager.undoManager;
    }

    private void Update()
    {
        if (isHoldingDown)
            ExecuteHold();
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

    private void UpdateCounterText() { transformerCountText.text = $"{transformerData.transformerCounter}"; }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (transformerData.transformerCounter > 0)
        {
            if (!previewingTransformation)
            {
                DisplayShapePreviews();
                bypassEndHolding = true;
            }
            else
                BeginHolding();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (bypassEndHolding)
        {
            bypassEndHolding = false;
            return;
        }

        EndHolding();
    }

    private void BeginHolding()
    {
        isHoldingDown = true;
        inputTimer = 0;

        //DisplayShapePreviews();
    }

    private void ExecuteHold()
    {
        if (isHoldingDown)
        {
            inputTimer = Mathf.Clamp(inputTimer + Time.deltaTime, 0, inputTime);
            transformerIconFill.fillAmount = (inputTimer / inputTime);
        }
    }

    private void EndHolding()
    {
        if (isHoldingDown)
        {
            isHoldingDown = false;
            transformerIconFill.fillAmount = 0;

            if (inputTimer >= inputTime)
                FinalizeShapeTransformations();
            else
                RevertShapeTransformations();
        }
    }

    private void DisplayShapePreviews()
    {
        Transform gameBoardParent = gameManager.gameBoardParent;
        List<int> surroundingShapes = boardManager.CheckForSurroundingShapes(transform.GetSiblingIndex(), gameBoardParent);
        if (surroundingShapes != null && surroundingShapes.Count > 0)
        {
            cachedBoardData = new BoardData(gameBoardParent);
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
                        newShapeTypeIndex = 0;

                    GameShape.ShapeType newShapeType = (GameShape.ShapeType)newShapeTypeIndex;
                    gameShape.SetShapeType(newShapeType);

                    shapeImage.color = new Color(shapeImage.color.r, shapeImage.color.g, shapeImage.color.b, shapeImage.color.a * 0.5f);
                }
            }

            previewingTransformation = true;
        }
    }

    private void FinalizeShapeTransformations()
    {
        if (shapesToTransform != null && shapesToTransform.Count > 0)
        {
            undoManager.PushBoardData(cachedBoardData);
            cachedBoardData = null;

            foreach (GameShape shape in shapesToTransform)
            {
                Image shapeImage = shape?.GetComponent<Image>();
                if (shapeImage != null)
                    shapeImage.color = new Color(shapeImage.color.r, shapeImage.color.g, shapeImage.color.b, 1f);
            }

            transformerData.transformerCounter--;
            UpdateCounterText();

            previewingTransformation = false;
            gameManager.CheckForVictory(gameManager.gameBoardParent, gameManager.solutionBoardParent);
        }
    }

    private void ResetShapePreviewAlpha()
    {
        if (shapesToTransform != null && shapesToTransform.Count > 0)
        {
            foreach (GameShape shape in shapesToTransform)
            {
                Image shapeImage = shape?.GetComponent<Image>();
                if (shapeImage != null)
                    shapeImage.color = new Color(shapeImage.color.r, shapeImage.color.g, shapeImage.color.b, 1f);
            }
        }
    }

    private void RevertShapeTransformations()
    {
        if (shapesToTransform != null && shapesToTransform.Count > 0)
        {
            foreach (GameShape shape in shapesToTransform)
            {
                ShapeData shapeData = shape?.GetShapeData();
                Image shapeImage = shape?.GetComponent<Image>();
                if (shape != null && shapeData != null && shapeImage != null)
                {
                    GameShape.ShapeType shapeType = shapeData.shapeType;

                    int newShapeTypeIndex = (int)shapeType - 1;
                    if (newShapeTypeIndex < 0)
                        newShapeTypeIndex = Enum.GetNames(typeof(GameShape.ShapeType)).Length - 1;

                    GameShape.ShapeType newShapeType = (GameShape.ShapeType)newShapeTypeIndex;
                    shape.SetShapeType(newShapeType);
                }
            }

            previewingTransformation = false;
        }
    }
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
}