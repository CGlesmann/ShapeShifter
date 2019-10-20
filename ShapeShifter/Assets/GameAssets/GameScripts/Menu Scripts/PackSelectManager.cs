using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PackSelectManager : LevelSelectManager
{
    public void NavigateToLevelSelect(string packScreen)
    {
        SceneManager.LoadScene(packScreen);
    }
}
