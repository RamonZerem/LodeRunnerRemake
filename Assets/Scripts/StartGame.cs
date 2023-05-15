using UnityEngine;

public class StartGame : MonoBehaviour
{
    
    #region MonoBehaviour

    private void Awake()
    {
        MySceneManager.LifeLeft = 5;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return))
            MySceneManager.Load(MySceneManager.Scene.LifeLeft);
    }
    
    #endregion
    
}
