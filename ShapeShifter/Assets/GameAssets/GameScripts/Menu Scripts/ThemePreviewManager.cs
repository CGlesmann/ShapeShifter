using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemePreviewManager : MonoBehaviour
{
    public void PlayExitAnimation() { GetComponent<Animator>().SetTrigger("Exit"); }
    public void DisableThemePreview() { gameObject.SetActive(false); }
}
