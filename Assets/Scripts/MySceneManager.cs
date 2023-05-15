using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MySceneManager 
{

    #region Fields

    public static int LifeLeft { get; set;}

    public enum Scene
    {
        MyLevel,
        LifeLeft,
        StartGame,
        WinningScene,
        GameOver,
    }

    #endregion
    
    public static void Load(Scene scene)
    {
        SceneManager.LoadSceneAsync(scene.ToString());
    }
    
}
