﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPreview : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private LevelSelectManager manager = null;
    [SerializeField] private Animator anim = null;

    public void DisablePanel() { gameObject.SetActive(false); }
    public void ExecuteSceneExitTransition() { manager.BeginSceneExitTransition(); }

    public void HideLevelPreview() { anim.SetTrigger("Dismiss"); }
    public void BeginTransitionToLevel() { anim.SetTrigger("Exit"); }
}
