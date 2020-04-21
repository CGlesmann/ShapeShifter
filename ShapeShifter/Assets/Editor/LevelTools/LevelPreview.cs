using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelPreview : EditorWindow
{
    private LevelPreview levelPreviewWindow = null;
    private List<BoardData> generatedBoards = null;
    int width, height;

    private void GetLevelPreviewWindow() { levelPreviewWindow = GetWindow<LevelPreview>(); }
    public void DisplayLevelPreviews(List<BoardData> generatedBoards, int boardWidth, int boardHeight)
    {
        this.generatedBoards = new List<BoardData>(generatedBoards);
        width = boardWidth;
        height = boardHeight;

        if (levelPreviewWindow == null)
            GetLevelPreviewWindow();

        levelPreviewWindow.Show();
    }

    private void OnGUI()
    {
        foreach (BoardData board in generatedBoards)
            DrawLevelPreview(board, width, height);
    }

    private void DrawLevelPreview(BoardData data, int boardWidth, int boardHeight)
    {
        for(int i = 0; i < boardHeight; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for(int j = 0; j < boardWidth; j++)
            {
                if (data.board[i] != null)
                {
                    GUIStyle shapeStyle = new GUIStyle();
                    shapeStyle.normal.background = AssetPreview.GetAssetPreview(ShapeSettings.GetShapeSprite(data.board[i].shapeType));
                }
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}
