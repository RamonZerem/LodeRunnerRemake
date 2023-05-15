using UnityEngine;

public class Ladder : MonoBehaviour
{

    #region Inspector

    [SerializeField]
    private LadderPart part = LadderPart.Complete;

    [SerializeField]
    public bool lastLadder;
    
    #endregion

    #region Fields

    private enum LadderPart
    {
        Complete,
        Bottom,
        Top
    };

    public float Size { get; private set; }
    
    #endregion

    #region MonoBehaviour
    
    private void Start()
    {
        Size = GetComponentInParent<SpriteRenderer>().size.y;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMove player = other.GetComponent<PlayerMove>();
            player.Ladder = this;
            switch (part)
            {
                case LadderPart.Complete:
                    player.CanClimb = true;
                    break;
                case LadderPart.Bottom:
                    player.BottomLadder = this;
                    break;
                case LadderPart.Top:
                    player.TopLadder = this;
                    break;
            }
        }
        if (other.CompareTag("Enemy"))
        {
            EnemyMove enemy = other.GetComponent<EnemyMove>();
            enemy.Ladder = this;
            switch (part)
            {
                case LadderPart.Complete:
                    enemy.CanClimb = true;
                    break;
                case LadderPart.Bottom:
                    enemy.BottomLadder = this;
                    break;
                case LadderPart.Top:
                    enemy.TopLadder = this;
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerMove player = col.GetComponent<PlayerMove>();
            switch (part)
            {
                case LadderPart.Complete:
                    player.CanClimb = false;
                    break;
                case LadderPart.Bottom:
                    player.BottomLadder = null;
                    break;
                case LadderPart.Top:
                    player.TopLadder = null;
                    break;
            }
        }
        if (col.CompareTag("Enemy"))
        {
            EnemyMove enemy = col.GetComponent<EnemyMove>();
            enemy.Ladder = this;
            switch (part)
            {
                case LadderPart.Complete:
                    enemy.CanClimb = false;
                    break;
                case LadderPart.Bottom:
                    enemy.BottomLadder = null;
                    break;
                case LadderPart.Top:
                    enemy.TopLadder = null;
                    break;
            }
        }
    }

    public void Take()
    {
        if (transform.parent)
            transform.parent.gameObject.SetActive(false);
        else
            transform.gameObject.SetActive(false);
    }

    public void Place(Vector3 place)
    {
        Vector3 newPos = new Vector3(place.x, place.y + (Size / 2), 0);
        if (transform.parent)
        {
            transform.parent.position = newPos;
            transform.parent.gameObject.SetActive(true);
        }
        else
        {
            transform.position = newPos;
            transform.gameObject.SetActive(true);
        }
    }
    
    #endregion

}
