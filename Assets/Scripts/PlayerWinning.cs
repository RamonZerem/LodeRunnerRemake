    using System.Collections;
    using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerWinning : MonoBehaviour
{

    #region Inspector

    [SerializeField]
    private  float topOfLadder;

    #endregion

    #region Fields

    private Rigidbody2D _rigidbody;
    
    private Animator _animator;
    
    private static readonly int IS_CLIMBING = Animator.StringToHash("IsClimbing");
    
    private static readonly int IS_WINNING = Animator.StringToHash("IsWinning");

    #endregion

    #region MonoBehaviour

    private void Awake ()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        StartCoroutine(ClimbLadder());
    }

    private IEnumerator ClimbLadder()
    {
        _animator.SetBool(IS_CLIMBING, true);
        while (_rigidbody.transform.position.y <= topOfLadder)
        {
            _rigidbody.velocity = Vector2.up / 2;
            yield return new WaitForFixedUpdate();
        }
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        _animator.SetBool(IS_WINNING, true);
    }

    #endregion
    
}
