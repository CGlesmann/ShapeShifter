using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private static DebugManager debugManager = null;

    [Header("Object References")]
    [SerializeField] private GameObject debugConsole = null;
    private GameObject currentConsole = null;

    private void Awake()
    {
        if (debugManager == null)
        {
            debugManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            GameObject.DestroyImmediate(gameObject);
    }

    private void Update()
    {
        if (Debug.isDebugBuild)
        {
            #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetKeyDown(KeyCode.Escape))
                ToggleConsole();
            #endif

            #if UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount >= 4)
                ToggleConsole();
            #endif
        }
    }

    private void ToggleConsole()
    {
        if (currentConsole == null)
            currentConsole = GameObject.Instantiate(debugConsole);
        else 
        {
            if (!currentConsole.activeSelf)
                currentConsole.SetActive(true);
            else
                currentConsole.SetActive(false);
        }
    }
}
