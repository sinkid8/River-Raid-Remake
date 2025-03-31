using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int scoreValue = 10;
    
    private EnemyScoreHandler scoreHandler;
    private bool isDestroyed = false;
    
    private void Start()
    {
        // Initialize score handler
        scoreHandler = GetComponent<EnemyScoreHandler>();
        if (scoreHandler == null)
        {
            // Add score handler if not present
            scoreHandler = gameObject.AddComponent<EnemyScoreHandler>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Avoid multiple collisions
        if (isDestroyed)
            return;
            
        // Check if hit by a projectile
        if (collision.GetComponent<ProjectileController>() != null)
        {
            // Destroy self
            DestroyEnemy();
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

    // This can be called from the HealthComponent's OnDeath event or directly
    public void DestroyEnemy()
    {
        // Avoid multiple calls
        if (isDestroyed)
            return;
            
        isDestroyed = true;
            
        // Award score
        if (scoreHandler != null)
        {
            scoreHandler.AwardScore();
        }
        else if (ScoreManager.Instance != null)
        {
            // Fallback to direct score manager if score handler isn't available
            ScoreManager.Instance.AddScore(scoreValue);
        }

        // Spawn explosion effect
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Play explosion sound
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.explosionClip);
        }

        // Destroy the enemy
        Destroy(gameObject);
    }
}