using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShaker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 basePosition = Vector3.zero;
    [SerializeField] private float shakeStrength = 0f;
    [SerializeField] private Transform objectTransformer = null;

    private Vector3 startPosition = Vector3.zero;
    private bool shaking = false;

    private void Update()
    {
        if (shaking)
            objectTransformer.localPosition = basePosition + (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * shakeStrength);
    }

    private void StartShaking() { startPosition = objectTransformer.localPosition; shaking = true; }
    private void EndShaking() { shaking = false; objectTransformer.localPosition = startPosition; }
}
