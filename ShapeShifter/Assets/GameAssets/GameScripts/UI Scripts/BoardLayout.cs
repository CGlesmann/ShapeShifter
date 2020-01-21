using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoardLayout : MonoBehaviour
{
    [Header("Board Settings")]
    [SerializeField] private int rowLength = 3;
    [SerializeField] private int rowHeight = 3;
    [SerializeField] private float horizontalSpaceSize = 8f;
    [SerializeField] private float verticalSpaceSize = 8f;

    private void Awake() { AlignBoard(); }

    public void AlignBoard()
    {
        float leftBoarder = transform.position.x, rightBoarder = GetComponent<RectTransform>().rect.width;
        float topBoarder = transform.position.y, bottomBoarder = GetComponent<RectTransform>().rect.height;
        Vector2 currentPosition = new Vector2(leftBoarder, topBoarder);

        float width = ((rightBoarder - leftBoarder) - (horizontalSpaceSize * (rowLength - 1)) / rowLength), horizontalInc = width + horizontalSpaceSize;
        float height = ((bottomBoarder - topBoarder) - (verticalSpaceSize * (rowHeight - 1)) / rowHeight), verticalInc = height + verticalSpaceSize;

        for (int i = 0; i < transform.childCount; i++)
        {
            if ((i + 1) % rowLength == 0)
            {
                currentPosition.x = leftBoarder;
                currentPosition.y += verticalInc;
            }

            transform.GetChild(i).position = new Vector2(currentPosition.x, currentPosition.y);
            transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            currentPosition.x += horizontalInc;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BoardLayout))]
public class BoardLayoutInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BoardLayout board = (BoardLayout)target;

        EditorGUILayout.Separator();
        if (GUILayout.Button("Align Board"))
            board.AlignBoard();
    }
}
#endif
