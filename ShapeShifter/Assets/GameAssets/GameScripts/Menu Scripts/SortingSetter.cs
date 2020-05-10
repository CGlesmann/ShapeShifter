using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SortingSetter : MonoBehaviour
{
    public void Start() { ReorderCanvas(); }
    public void ReorderCanvas()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.sortingOrder++;
        canvas.sortingOrder--;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SortingSetter))]
public class SetterEditor : Editor
{
    [MenuItem("Reorder All Canvas Sorters", menuItem = "Global Tools/Reorder All Canvas Sorters")]
    public static void ReorderAllSorters()
    {
        SortingSetter[] sorters = GameObject.FindObjectsOfType<SortingSetter>();
        foreach (SortingSetter sorter in sorters)
            sorter.ReorderCanvas();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Run Reorder"))
        {
            SortingSetter setter = (SortingSetter)target;
            setter.ReorderCanvas();
        }
    }
}
#endif
