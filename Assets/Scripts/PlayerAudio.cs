using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAudio : MonoBehaviour
{

    #region Inspector

    [SerializeField]
    private AudioSource walkingAudio1;
    
    [SerializeField]
    private AudioSource walkingAudio2;
    
    [SerializeField]
    private AudioSource climbingAudio1;
    
    [SerializeField]
    private AudioSource climbingAudio2;
    
    [SerializeField]
    private AudioSource deathAudio;
    
    [SerializeField]
    private AudioSource fallingAudio;
    
    [SerializeField]
    private AudioSource powerAudio;

    #endregion

    #region Fields

    private Animator _animator;
    
    private static readonly int IS_FALLING = Animator.StringToHash("IsFalling");

    #endregion

    #region MonoBehaviour
    
    private void Awake ()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayWalkingAudio1()
    {
        walkingAudio1.Play();
    }
    public void PlayWalkingAudio2()
    {
        walkingAudio2.Play();
    }
  
    public void PlayClimbingAudio1()
    {
        climbingAudio1.Play();
    }
    
    public void PlayClimbingAudio2()
    {
        climbingAudio2.Play();
    }
    
    public void PlayFallingAudio()
    {
        fallingAudio.Play();
    }
    
    public void PlayDeathAudio()
    {
        deathAudio.Play();
    }
    
    public void PlayPowerAudio()
    {
        powerAudio.Play();
    }

    private void Update()
    {
        if (!_animator.GetBool(IS_FALLING))
            fallingAudio.Stop();
    }
    
    #endregion

}
