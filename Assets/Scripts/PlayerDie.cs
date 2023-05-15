using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerDie : MonoBehaviour
{

    #region Inspector

    [SerializeField]
    private float dieAnimationTime;

    #endregion

    #region Fields

    private Animator _animator;
    
    private static readonly int IsDying = Animator.StringToHash("IsDying");

    #endregion

    #region MonoBehaviour

    private void Awake ()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Enemy"))
        {
            StartCoroutine(LoseLife());
        }
    }

    public IEnumerator LoseLife()
    {
        _animator.SetBool(IsDying, true);
        GameManager.Shared.FreezeAll = true;
        yield return new WaitForSeconds(dieAnimationTime);
        GameManager.Shared.RestartGame();
    }

    #endregion
    
}
