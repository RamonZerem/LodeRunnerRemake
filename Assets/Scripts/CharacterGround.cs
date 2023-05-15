using UnityEngine;

public sealed class CharacterGround : MonoBehaviour
{
    
  #region Inspector

  [SerializeField]
  private float rayLength = 1.05f;

  [SerializeField]
  private float rayRadius = 0.3f;

  [SerializeField]
  private LayerMask rayLayerGround = default;
  
  #endregion
  
  #region Fields

  private const float DEFAULT_RADIUS = 0.1f;
  
  public bool IsGrounded { get; private set; }
  
  public Collider2D WhatTile{ get; private set; }
  
  #endregion
  
  #region MonoBehaviour

  /*
   * IsTouchingGround is used to find out if the character is touching the ground
   * it do so by sending rays down and see if they collide with the ground
   *
   * @Param: radiusX where to send the ray
   * @Return: true if touching ground false otherwise
   */
  private Collider2D IsTouching(float radiusX, LayerMask rayLayer)
  {
    Vector3 origin = transform.position;
    origin.x += radiusX;
    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, rayLength, rayLayer);
    return hit.collider;
  }

  private void FixedUpdate()
  {
      IsGrounded = IsTouching(rayRadius, rayLayerGround) || IsTouching(-rayRadius, rayLayerGround);
      WhatTile = IsTouching(DEFAULT_RADIUS, rayLayerGround);
  }
  
  #endregion
  
}