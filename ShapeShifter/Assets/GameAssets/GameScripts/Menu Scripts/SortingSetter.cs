using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingSetter : MonoBehaviour
{
    void Start()
    {
        Canvas canvas = GetComponent<Canvas>();
        canvas.sortingOrder++;
        canvas.sortingOrder--;
    }
}
