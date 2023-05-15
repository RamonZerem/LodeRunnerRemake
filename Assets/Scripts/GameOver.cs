using UnityEngine;

public class GameOver : MonoBehaviour
{
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            MySceneManager.Load(MySceneManager.Scene.StartGame);
        }
    }
    
}
