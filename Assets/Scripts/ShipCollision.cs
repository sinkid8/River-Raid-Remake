using UnityEngine;

public class ShipCollision : MonoBehaviour
{
    public GameObject explosionPrefab;  

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Debris"))
        {
            TriggerExplosion();
            Destroy(gameObject);  // Destroy the ship
        }
    }

    private void TriggerExplosion()
    {
       
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
}
