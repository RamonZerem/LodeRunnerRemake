using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public class GameManager : MonoBehaviour
{
    
    #region Inspector
    
    [SerializeField]
    private CinemachineVirtualCamera[] vCams;
    
    [SerializeField]
    private GameObject[] goldPiles;

    [SerializeField]
    private Ladder winningLadder;

    [SerializeField]
    private Rigidbody2D player;
    
    [SerializeField]
    private AudioSource backgroundMusic;
    
    [SerializeField]
    private AudioSource goldAudio;
    
    [SerializeField]
    private AudioSource allGoldAudio;
    
    #endregion
    
    #region Fields
    
    private bool _canWin;
    
    private int _numOfGoldPiles;

    private const float MAX_HEIGHT = 8.5f;
    
    public bool FreezeAll { get; set; }
    
    public static GameManager Shared { get; private set; }

    #endregion

    #region MonoBehaviour
    
    private void Awake()
    {
        Shared = this;
        StartCoroutine(StartCameras());
        _numOfGoldPiles = goldPiles.Length;
    }

    private void Update()
    {
        if (_canWin)
        {
            if (player.transform.position.y >= MAX_HEIGHT) 
            {
                MySceneManager.Load(MySceneManager.Scene.WinningScene);
            }
        }
    }
    
    private IEnumerator StartCameras()
    {
        FreezeAll = true;
        yield return new WaitForSeconds(0.5f);
        vCams[0].Priority = 8;
        yield return new WaitForSeconds(2);
        vCams[1].Priority = 7;
        yield return new WaitForSeconds(3);
        vCams[2].Priority = 6;
        yield return new WaitForSeconds(1.3f);
        vCams[3].Priority = 5;
        FreezeAll = false;
    }

    public void CollectedGold()
    {
        goldAudio.Play();
        if (_numOfGoldPiles == 0)
        {
            allGoldAudio.Play();
            winningLadder.gameObject.SetActive(true);
            _canWin = true;
        }
        _numOfGoldPiles--;
    }
    
    public void RestartGame()
    {
        MySceneManager.LifeLeft--;
        MySceneManager.Load(MySceneManager.LifeLeft > 0
            ? MySceneManager.Scene.LifeLeft
            : MySceneManager.Scene.GameOver);
        FreezeAll = false;
    }
    
    #endregion

}
