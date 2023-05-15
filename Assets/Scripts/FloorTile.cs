using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FloorTile : MonoBehaviour
{
    
    #region Inspector
    
    [SerializeField]
    private LayerMask rayLayerGround;
    
    [SerializeField]
    private float reBuildTime = 5f;

    #endregion

    #region Fields
    
    private readonly List<Collider2D> _colliders = new List<Collider2D>();
    
    private Animator _animator;
    
    private const float TILE_SIZE = 1f;

    private const float TILE_COLLIDER_HEIGHT = 0.45f;
    
    private const float HALF_TILE_SIZE = 0.5f;

    private const float RAY_UP_LEN = 0.1f;
    
    private static readonly int TileBreak = Animator.StringToHash("TileBreak");
    
    public FloorTile LeftTile;
    
    public FloorTile RightTile;
    
    public bool IsEmpty { get; private set; }
    
    #endregion

    #region MonoBehaviour

    private void Awake ()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Find tiles left and right
        Vector2 originLeft = new Vector2(transform.position.x - HALF_TILE_SIZE,
            transform.position.y + TILE_COLLIDER_HEIGHT);
        Vector2 originRight = new Vector2(transform.position.x + HALF_TILE_SIZE,
            transform.position.y + TILE_COLLIDER_HEIGHT);
        RaycastHit2D hitLeft = Physics2D.Raycast(originLeft, Vector2.left, HALF_TILE_SIZE,
            rayLayerGround);
        if (hitLeft &&hitLeft.collider && hitLeft.collider.GetComponent<FloorTile>())
            LeftTile = hitLeft.collider.GetComponent<FloorTile>();
        RaycastHit2D hitRight = Physics2D.Raycast(originRight, Vector2.right, HALF_TILE_SIZE,
            rayLayerGround);
        if (hitRight &&hitRight.collider && hitRight.collider.GetComponent<FloorTile>())
            RightTile = hitRight.collider.GetComponent<FloorTile>();
        _colliders.AddRange(GetComponents<Collider2D>());
        _colliders.AddRange(GetComponentsInChildren<Collider2D>());
        IsEmpty = false;
    }
    
    public IEnumerator Break()
    {
        _animator.SetBool(TileBreak, true);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length +
                                        _animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        DisableColliders();
        IsEmpty = true;
        StartCoroutine(ReBuild());
    }

    private IEnumerator ReBuild()
    {
        yield return new WaitForSeconds(reBuildTime);
        _animator.SetBool(TileBreak, false);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length +
                                        _animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        EnableColliders();
        IsEmpty = false;
    }

    public bool HasSomethingOnTop()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y + TILE_SIZE);
        RaycastHit2D hitUp = Physics2D.Raycast(origin, Vector3.up, RAY_UP_LEN);
        Debug.DrawRay(origin, Vector3.up * RAY_UP_LEN, Color.magenta);
        return hitUp;
    }

    public void EnableColliders()
    {
        foreach (Collider2D myCollider in _colliders)
        {
            myCollider.enabled = true;
        }
    }

    public void DisableColliders()
    {
        foreach (Collider2D myCollider in _colliders)
        {
            if (!myCollider.CompareTag("KillCollider"))
                myCollider.enabled = false;
        }
    }
    
    #endregion
    
}
