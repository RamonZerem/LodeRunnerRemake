using UnityEngine;
using UnityEngine.UI;

public class LifeLeft : MonoBehaviour
{

    #region Inspector

    [SerializeField]
    private Sprite[] life;
    
    [SerializeField]
    private Image curImage;

    #endregion

    #region MonoBehaviour
    
    private void Awake()
    {
        if (MySceneManager.LifeLeft != 0)
            curImage.sprite = life[MySceneManager.LifeLeft - 1];
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Return))
            MySceneManager.Load(MySceneManager.Scene.MyLevel);
    }
    
    #endregion

}
