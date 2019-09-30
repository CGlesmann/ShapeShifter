using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    private static DebugMenu menu = null;

    [Header("UI References")]
    [SerializeField] private GameObject debugMenuParent = null;

    private void Awake()
    {
        if (menu == null)
        {
            menu = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    
    private void Update()
    {
    #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (debugMenuParent != null)
                debugMenuParent.SetActive(!debugMenuParent.activeSelf);
        }
    #endif

    #if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 4)
        {
            if (debugMenuParent != null)
                debugMenuParent.SetActive(!debugMenuParent.activeSelf);
        }
    #endif
    }
}
