using UnityEngine;

public class Gold : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        Destroy(gameObject);
        GameManager.Shared.CollectedGold();
    }
    
}
