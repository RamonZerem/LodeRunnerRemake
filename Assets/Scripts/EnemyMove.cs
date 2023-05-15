using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyMove : MonoBehaviour
{
  
  #region Inspector

  [SerializeField, Min(0.1f)]
  private float speed = 1;
  
  [SerializeField]
  private float respawnAnimationTime;
  
  [SerializeField]
  private float timeToBeStuck = 3;

  [SerializeField]
  private Rigidbody2D player;
  
  [SerializeField]
  private CharacterGround ground;

  [SerializeField]
  private LayerMask rayLayerLadder;
  
  [SerializeField]
  private GameObject[] dropTilesPos;
  
  [SerializeField]
  private Vector3[] respawnPlaces;
  
  #endregion
  
  #region Fields
  
  private bool _isFreeze;
  
  private bool _climbing;
  
  private bool _falling;
  
  private Vector2 _whereToGo;
  
  private Vector2 _dropTile;
  
  private Vector2 _curVelocity;

  private Animator _animator;
  
  private Rigidbody2D _rigidbody;
  
  private const int NUM_OF_UPDATE_TO_SHAKE = 20;
  
  private const float SAME_LEVEL_DIFF = 0.06f;
  
  private const float DROP_TILE_DIFF = 0.2f;

  private const float BLOCK_SIZE = 1f;
  
  private const float SEARCH_RAY_LENGTH = 20f;
  
  private const float SHAKE_RADIUS = 0.12f;
  
  private const float CLIMB_OUT_SIZE = 0.8f;

  private static readonly Vector3 LOOK_LEFT = Vector3.one;
  
  private static readonly Vector3 LOOK_RIGHT = new Vector3(-1, 1, 1);

  private static readonly int IS_CLIMBING = Animator.StringToHash("IsClimbing");
  
  private static readonly int IS_FALLING = Animator.StringToHash("IsFalling");
  
  private static readonly int IS_STUCK = Animator.StringToHash("IsStuck");
  
  private static readonly int IS_DEAD = Animator.StringToHash("IsDead");

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
    _whereToGo = player.transform.position - transform.position;
  }
  
  private void FixedUpdate()
  {
    // check if can change direction
    if (GameManager.Shared.FreezeAll)
    {
      _animator.enabled = false;
      _rigidbody.velocity = Vector2.zero;
      return;
    }
    _animator.enabled = true;
    if (_isFreeze)
      return;
    if (_falling)
      goto fall;
    
    _curVelocity = _rigidbody.velocity;
    
    // go up and down if needed
    if (MathF.Abs(_whereToGo.y) < SAME_LEVEL_DIFF)
      goto moveToSides;
    
    if (_whereToGo.y > 0)
    {
      ClimbLadder();
      goto doneMove;
    }
    if (_whereToGo.y < 0)
    {
      FindWayDown();
      goto doneMove;
    }
    
    moveToSides:
    if (_whereToGo.x > 0)
      GoRight();
    else
      GoLeft();
    doneMove:
    
    fall:
    if (!ground.IsGrounded && !CanClimb && !TopLadder && !_isFreeze)
    {
      _falling = true;
      _curVelocity.y = -speed;
      Vector2 fixPosition = new Vector2(MathF.Round((transform.position.x * 2),
        MidpointRounding.AwayFromZero) / 2, transform.position.y);
      transform.position = fixPosition;
      _curVelocity.x = 0;
      _rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }
    else
    {
      _falling = false;
      _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    
    if (!_climbing && !_falling)
      FixPlace();
    _rigidbody.velocity = _curVelocity;   
    _animator.SetBool(IS_FALLING, _falling);
    _animator.SetBool(IS_CLIMBING, _climbing);
  }

  private void GoLeft()
  {
    _curVelocity.x = -speed;
    _curVelocity.y = 0;
    transform.localScale = LOOK_LEFT;
    _climbing = false;
  }

  private void GoRight()
  {
    _curVelocity.x = speed;
    _curVelocity.y = 0;
    transform.localScale = LOOK_RIGHT;
    _climbing = false;
  }

  private void FindWayDown()
  {
    // if on ladder climb down
    if (((CanClimb || TopLadder) && !BottomLadder) || (BottomLadder && TopLadder))
    {
      transform.position = new Vector3(Ladder.transform.position.x, transform.position.y, 0);
      _curVelocity.y = -speed;
      _curVelocity.x = 0;
      _climbing = true;
      return;
    }
    
    //else find closest Drop tile and go there
    _dropTile = FindDropTile();
    if (_dropTile.x < transform.position.x)
    {
      GoLeft();
      return;
    }
    GoRight();
  }

  private Vector2 FindDropTile()
  {
    List<GameObject> possibleTiles = new List<GameObject>();
    foreach (GameObject tile in dropTilesPos)
    {
      if (Mathf.Abs
            (tile.transform.position.y - (transform.position.y - BLOCK_SIZE)) < DROP_TILE_DIFF)
      {
        possibleTiles.Add(tile);
        _dropTile = tile.transform.position;
      }
    }
    foreach (GameObject tile in possibleTiles)
    {
      if (Mathf.Abs(transform.position.x - tile.transform.position.x) < 
          Mathf.Abs(transform.position.x - _dropTile.x))
        _dropTile = tile.transform.position;
    }
    return _dropTile;
  }

  private void ClimbLadder()
  {
    if (CanClimb)
    {
      transform.position = new Vector3(Ladder.transform.position.x, transform.position.y, 0);
      _curVelocity.y = speed;
      _curVelocity.x = 0;
      _climbing = true;
      return;
    }
    Ladder leftLadder = FindLadder(-1);
    Ladder rightLadder = FindLadder(1);
    if (!leftLadder)
    {
      GoRight();
      return;
    }
    if (!rightLadder)
    {
      GoLeft();
      return;
    }
    if (player.transform.position.x - leftLadder.transform.position.x <
        rightLadder.transform.position.x - player.transform.position.x)
    {
      GoLeft();
      return;
    }
    GoRight();
    }

  private Ladder FindLadder(int i)
  {
    Vector3 origin = transform.position;
    RaycastHit2D hit = Physics2D.Raycast(origin, new Vector2(i, 0),
      SEARCH_RAY_LENGTH, rayLayerLadder);
    
    // will only show on editor
    Debug.DrawRay(origin, new Vector2(i, 0)  * SEARCH_RAY_LENGTH, Color.magenta);
    
    if (hit.collider && hit.collider.GetComponent<Ladder>())
      return hit.collider.GetComponent<Ladder>();
    return null;
  }

  private void FixPlace()
  {
    Vector2 fixPosition = new Vector2(transform.position.x,
      MathF.Round((transform.position.y * 2), MidpointRounding.AwayFromZero) / 2);
    transform.position = fixPosition;
  }
  
  private IEnumerator Respawn()
  {
    transform.position = respawnPlaces[Random.Range(0, respawnPlaces.Length)];
    _animator.SetBool(IS_DEAD, true);
    _animator.SetBool(IS_STUCK, false);
    _animator.Play("EnemyRespawn");
    yield return new WaitForSeconds(respawnAnimationTime);
    _animator.SetBool(IS_DEAD, false);
    _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    _isFreeze = false;
  }

  private IEnumerator ClimbOut()
  {
    Vector2 originalPlace = transform.position;
    yield return StartCoroutine(Shake(originalPlace));

    transform.position = originalPlace;
    Vector3 newPos;
    if (Random.value > 0.5f)
    {
      newPos = new Vector2(transform.position.x + CLIMB_OUT_SIZE, 
        transform.position.y + BLOCK_SIZE);
      transform.localScale = LOOK_RIGHT;
    }
    else
    {
      newPos = new Vector2(transform.position.x - CLIMB_OUT_SIZE, 
        transform.position.y + BLOCK_SIZE);
      transform.localScale = LOOK_LEFT;
    }
    _animator.SetBool(IS_CLIMBING, true);
    Vector2 climbMostWay = new Vector2(transform.position.x, transform.position.y + CLIMB_OUT_SIZE);
    while (transform.position.y != climbMostWay.y)
    {
      transform.position = Vector2.MoveTowards(transform.position, climbMostWay,
        speed * Time.deltaTime);
      yield return new WaitForFixedUpdate();
    }
    _animator.SetBool(IS_CLIMBING, false);
    while (transform.position != newPos)
    {
      transform.position = Vector3.MoveTowards(transform.position, newPos,
        speed * Time.deltaTime);
      yield return new WaitForFixedUpdate();
    }
    _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    _isFreeze = false;
    _animator.SetBool(IS_STUCK, false);
  }

  private IEnumerator Shake(Vector2 originalPlace)
  {
    for (int i = 0; i < NUM_OF_UPDATE_TO_SHAKE; i++)
    {
      if ((i == 0 || i == NUM_OF_UPDATE_TO_SHAKE/5*2) || i == NUM_OF_UPDATE_TO_SHAKE/5*4)
        transform.position = new Vector2(originalPlace.x + SHAKE_RADIUS, originalPlace.y);
      if (i == NUM_OF_UPDATE_TO_SHAKE/5 || i == NUM_OF_UPDATE_TO_SHAKE/5*3)
        transform.position = new Vector2(originalPlace.x - SHAKE_RADIUS, originalPlace.y);
      yield return new WaitForFixedUpdate();
    }
  }

  public IEnumerator EnemyStuck(FloorTile floorTile)
  {
    transform.position = floorTile.transform.position;
    floorTile.EnableColliders();
    _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    _isFreeze = true;
    _animator.SetBool(IS_STUCK, true);
    _animator.SetBool(IS_FALLING, false);
    yield return new WaitForSeconds(timeToBeStuck);
    if (!floorTile.IsEmpty)
    {
      yield return StartCoroutine(Respawn());
      floorTile.EnableColliders();
    }
    else
    {
      yield return StartCoroutine(ClimbOut());
      if (floorTile.IsEmpty)
        floorTile.DisableColliders();
    }
  }

  #endregion

}