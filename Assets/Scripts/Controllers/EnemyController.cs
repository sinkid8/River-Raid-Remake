using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int scoreValue = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if hit by a projectile (the projectile script will handle damaging us)
        if (collision.GetComponent<ProjectileController>() != null)
        {
            // Nothing needed here since the ProjectileController handles the damage
            Debug.Log("Enemy hit by projectile");
        }

        // Check if hit by the player
        if (collision.CompareTag("Player"))
        {
            // Destroy player if possible
            ShipCollision playerCollision = collision.GetComponent<ShipCollision>();
            if (playerCollision != null)
            {
                // Use the player's collision handling logic
                // This will trigger the player's explosion and destruction
            }

            // Destroy self
            DestroyEnemy();
        }
    }

    // This can be called from the HealthComponent's OnDeath event
    public void DestroyEnemy()
    {
        // Award score
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }

        // Spawn explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Destroy the enemy
        Destroy(gameObject);
    }
}