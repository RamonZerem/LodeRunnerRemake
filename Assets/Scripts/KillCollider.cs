using UnityEngine;

public class KillCollider : MonoBehaviour
{

    #region Fields

    private FloorTile _floorTile;
    
    private PlayerDie _player;
    
    #endregion

    #region MonoBehaviour
    
    private void Start()
    {
        _floorTile = GetComponentInParent<FloorTile>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            StartCoroutine((col.GetComponentInParent<EnemyMove>().EnemyStuck(GetComponentInParent<FloorTile>())));
        }
        else if (col.CompareTag("Player"))
        {
            _player = col.GetComponent<PlayerDie>();
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            _player = null;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_floorTile.IsEmpty)
        {
            StartCoroutine(_player.LoseLife());
        }
    }
    
    #endregion

}
