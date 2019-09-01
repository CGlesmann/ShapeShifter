using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSlot : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private GameManager manager = null;

    [Header("Control Variables")]
    private bool selected = false;

    /// <summary>
    /// Selects the slot if unselected, otherwise unselects the slot
    /// </summary>
    public void ToggleSelect()
    {
        if (selected)
        {
            Debug.Log("De-Selecting");
            selected = false;
        } else {
            Debug.Log("Selecting");
            selected = true;
        }
    }
}
