using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public sealed class PlayerMove : MonoBehaviour
{
  
  #region Inspector

  [SerializeField, Min(0.1f)]
  private float speed = 2;

  [SerializeField]
  private CharacterGround ground;
  
  [SerializeField]
  private PlayerPower power;
  
  #endregion
  
  #region Fields
  
  private bool _hasInput;
  
  private bool _onGround;
  
  private bool _climbing;
  
  private bool _climbed;
  
  private bool _falling;
  
  private float _vertical;

  private float _horizontal;

  private Rigidbody2D _rigidbody;
  
  private Animator _animator;
  
  private const float MIN_TO_CLIMB = 0.1f;

  private static readonly int IS_DYING = Animator.StringToHash("IsDying");
  
  private static readonly int IS_FALLING = Animator.StringToHash("IsFalling");
  
  private static readonly int IS_CLIMBING = Animator.StringToHash("IsClimbing");
  
  private static readonly int IS_MOVING = Animator.StringToHash("IsMoving");

  public bool CanClimb {get; set;}
  public Ladder BottomLadder {get; set;}
  public Ladder TopLadder {get; set;}
  public Ladder Ladder { get; set; }
  
  #endregion
  
  #region MonoBehaviour

  private void Awake ()
  {
    _rigidbody = GetComponent<Rigidbody2D>();
    _animator = GetComponent<Animator>();
  }

  private void Update()
  {
    _horizontal = Input.GetAxisRaw("Horizontal") * speed;
    
    if (CanClimb && Mathf.Abs(Input.GetAxisRaw("Vertical")) > MIN_TO_CLIMB)
    {
      _vertical = Input.GetAxisRaw("Vertical") * speed;
      transform.position = new Vector3(Ladder.transform.position.x, transform.position.y, 0);
      _climbing = true;
    }
    
    else if (TopLadder && Input.GetAxisRaw("Vertical") < -MIN_TO_CLIMB)
    {
      _vertical = Input.GetAxisRaw("Vertical") * speed;
      transform.position = new Vector3(Ladder.transform.position.x, transform.position.y, 0);
      _climbing = true; 
    }
    
    else
    {
      _climbing = false;
      _vertical = 0;
    }
  }
  
  private void FixedUpdate()
  {
    if (GameManager.Shared.FreezeAll)
    {
      if (!_animator.GetBool(IS_DYING))
        _animator.enabled = false;
      _rigidbody.velocity = Vector2.zero;
      return;
    }
    _animator.enabled = true;
    if (power.IsFreeze)
      return;
    Vector2 velocity = _rigidbody.velocity;
    velocity.x = _horizontal;
    if (_horizontal != 0)
    {
      float xScale = Mathf.Sign(_horizontal) < 0 ? 1 : -1;
      transform.localScale = new Vector3(xScale, 1, 1);
      _climbed = false;
    }

    // climb
    if (_climbing)
    {
      if (_vertical > .1f)
        velocity.y = _vertical;
      else if (_vertical < -.1f && !BottomLadder)
        velocity.y = _vertical;
      _climbed = true;
    }
    else
      velocity.y = 0;
    
    //fall
    if (!ground.IsGrounded && !CanClimb && !TopLadder)
    {
      _falling = true;
      velocity.y = -speed;
      velocity.x = 0;
      _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }
    else
    {
      _falling = false;
      _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    if (!_climbing && !_falling)
      FixPlace();
    _animator.SetBool(IS_FALLING, _falling);
    _animator.SetBool(IS_CLIMBING, _climbed || _climbing);
    _animator.SetBool(IS_MOVING, _horizontal != 0);
    if (!_climbing && _climbed) // stopped in the middle of a ladder
      _animator.speed = 0;
    else
      _animator.speed = 1;
    _rigidbody.velocity = velocity;   
  }

  private void FixPlace()
  {
    Vector2 fixPosition = new Vector2(transform.position.x,
      MathF.Round((transform.position.y * 2), MidpointRounding.AwayFromZero) / 2);
    transform.position = fixPosition;
  }
  
  #endregion
  
}