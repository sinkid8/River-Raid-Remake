using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private bool isLaser = true; // True for laser, false for missile
    [SerializeField] private int scoreValue = 10; // Default score value for destroyed enemies

    // Removed explosion prefab from here to prevent double explosions

    private void Start()
    {
        Destroy(gameObject, lifeTime);
        if (AudioManager.instance != null && isLaser)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.laserClip);
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Debris"))
        {
            // Apply damage to health component
            HealthComponent health = collision.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
            
            // Try to trigger score directly on the enemy
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Let the enemy handle destruction, scoring and explosion
                enemy.DestroyEnemy();
            }
            else if (collision.CompareTag("Enemy") && ScoreManager.Instance != null)
            {
                // Direct score addition if no EnemyController exists
                ScoreManager.Instance.AddScore(scoreValue);
            }

            // Destroy the projectile without creating an explosion
            // The explosion will be created by the EnemyController instead
            Destroy(gameObject);
        }

        // Check if the projectile hits a battery (no collision)
        if (collision.CompareTag("Battery"))
        {
            // Ignore collision with battery
            Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>());
        }
    }
}