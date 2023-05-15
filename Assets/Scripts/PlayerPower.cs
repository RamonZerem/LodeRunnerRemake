using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerPower : MonoBehaviour
{
    
    #region Inspector

    [SerializeField]
    private CharacterGround ground;
    
    [SerializeField]
    private float timeToWait = 0.3f;
    
    [SerializeField]
    private PlayerMove move;
    
    [SerializeField]
    private LayerMask rayLayerGround;
    
    [SerializeField]
    private LayerMask rayLayerLadder;
    
    #endregion

    #region Fields

    private bool _shouldBreakLeft;
    
    private bool _shouldBreakRight;
    
    private bool _shouldTakeLadder;
    
    private bool _shouldReBuildLadder;
    
    private bool _hasLadder;

    private float _ladderSize;
    
    private Ladder _myLadder;
    
    private FloorTile _curTile;

    private Rigidbody2D _rigidbody;

    private Animator _animator;
    
    private const float SPRITE_BOUNDS_SAFE_DIS = 0.05f;
  
    private const float LADDER_BOUNDS_SAFE_DIS = 0.1f;

    private static readonly Vector3 LOOK_LEFT = Vector3.one;
  
    private static readonly Vector3 LOOK_RIGHT = new Vector3(-1, 1, 1);
    
    private static readonly int IS_DIGGING = Animator.StringToHash("IsDigging");
    
    private static readonly int IS_POWER = Animator.StringToHash("IsPower");

    private enum Sides
    {
        Left,
        Right,
    };
    
    public bool IsFreeze { get; private set; }

    #endregion

    #region MonoBehaviour
    
    private void Awake ()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            _shouldBreakLeft = true;
        }
        else if (Input.GetKey(KeyCode.X))
        {
            _shouldBreakRight = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_hasLadder)
                _shouldTakeLadder = true;
            else
                _shouldReBuildLadder = true;
        }
    }

    private void FixedUpdate()
    {
        if (ground.WhatTile)
            _curTile = ground.WhatTile.GetComponent<FloorTile>();
        else
        {
            ShouldDoNothing();
            return;
        }
        if (!_shouldTakeLadder && _shouldReBuildLadder && CanPlaceLadder() && ground.IsGrounded)
        {
            StartCoroutine(PlaceLadder());
            ShouldDoNothing();
            return;
        }
        if (_shouldTakeLadder && move.BottomLadder && ground.IsGrounded &&
            move.Ladder && !move.Ladder.lastLadder)
        {
            _myLadder = move.Ladder;
            StartCoroutine(TakeLadder());
            ShouldDoNothing();
            return;
        }
        if (_shouldBreakLeft)
        {
            if (_curTile && _curTile.LeftTile && !_curTile.LeftTile.IsEmpty &&
                !_curTile.LeftTile.HasSomethingOnTop())
            {   
                transform.localScale = LOOK_LEFT;
                StartCoroutine(BreakTile(Sides.Left));
            }
            _shouldBreakLeft = false;
        }
        else if (_shouldBreakRight)
        {
            if (_curTile && _curTile.RightTile && !_curTile.RightTile.IsEmpty &&
                !_curTile.RightTile.HasSomethingOnTop())
            {
                transform.localScale = LOOK_RIGHT;
                StartCoroutine(BreakTile(Sides.Right));
            }
            _shouldBreakRight = false;
        }
        ShouldDoNothing();
    }

    private void ShouldDoNothing()
    {
        _shouldBreakLeft = false;
        _shouldBreakRight = false;
        _shouldTakeLadder = false;
        _shouldReBuildLadder = false;
    }

    private bool CanPlaceLadder()
    {
        Vector3 origin = _curTile.transform.position;
        origin.y = GetComponent<SpriteRenderer>().bounds.min.y + SPRITE_BOUNDS_SAFE_DIS;

        RaycastHit2D hitGround = Physics2D.Raycast(origin, Vector2.up,
            _ladderSize - LADDER_BOUNDS_SAFE_DIS , rayLayerGround);
        RaycastHit2D hitLadder = Physics2D.Raycast(origin, Vector2.up,
            _ladderSize - LADDER_BOUNDS_SAFE_DIS , rayLayerLadder);

        // will only show on editor
        Debug.DrawRay(origin, Vector3.up * _ladderSize, Color.magenta);

        return !(hitGround) && !(hitLadder);
    }

    private IEnumerator BreakTile(Sides side)
    {
        CenterAndFreeze();
        _animator.SetBool(IS_DIGGING, true);
        switch (side)
        {
            case Sides.Left:
                StartCoroutine(_curTile.LeftTile.Break());
                break;
            case Sides.Right:
                StartCoroutine(_curTile.RightTile.Break());
                break;
        }
        yield return new WaitForSecondsRealtime(timeToWait);
        UnFreeze();
        _animator.SetBool(IS_DIGGING, false);
    }
    
    private IEnumerator TakeLadder()
    {
        if (_myLadder)
            _myLadder.Take();
        CenterAndFreeze();
        _animator.SetBool(IS_POWER, true);
        _ladderSize = _myLadder.Size;
        _hasLadder = true;
        _shouldTakeLadder = false;
        yield return new WaitForSecondsRealtime(timeToWait);
        UnFreeze();
        _animator.SetBool(IS_POWER, false);
    }
    
    private IEnumerator PlaceLadder()
    {
        CenterAndFreeze();
        _animator.SetBool(IS_POWER, true);
        _myLadder.Place(new Vector3(transform.position.x, GetComponent<SpriteRenderer>().bounds.min.y, 0));
        move.Ladder = _myLadder;
        _hasLadder = false;
        _shouldReBuildLadder = false;
        yield return new WaitForSecondsRealtime(timeToWait);
        UnFreeze();
        _animator.SetBool(IS_POWER, false);
    }

    private void CenterAndFreeze()
    {
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        IsFreeze = true;
        transform.position = new Vector3(_curTile.transform.position.x, transform.position.y, 0);
    }

    private void UnFreeze()
    {
        IsFreeze = false;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    #endregion

}
