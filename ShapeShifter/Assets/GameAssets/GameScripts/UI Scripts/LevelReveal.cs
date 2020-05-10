using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelReveal : MonoBehaviour
{
    [Header("Object Reference")]
    [SerializeField] private GameObject revealer = null;

    public void DeactivateRevealer() { revealer.SetActive(false); }
}
