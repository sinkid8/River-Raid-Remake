using UnityEngine;

public class EnergyProjectileController : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 30; // Higher damage than regular projectiles
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private float scale = 1.5f; // Larger projectile
    [SerializeField] private Color projectileColor = Color.cyan; // Distinctive color
    [SerializeField] private int scoreValue = 25; // Higher score value for energy weapon kills

    // Removed explosion prefab from here to prevent double explosions

    // Optional visual effects
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private ParticleSystem particles;

    private bool hasCollided = false;

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.energyProjectileClip);
        }
        // Apply scaling
        transform.localScale *= scale;

        // Apply color to sprite renderer if present
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = projectileColor;
        }

        // Set trail color if present
        if (trailRenderer != null)
        {
            trailRenderer.startColor = projectileColor;
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasCollided)
            return; // Ignore further collisions if the projectile already hit

        if (collision.CompareTag("Planet"))
        {
            hasCollided = true; // Mark that the projectile has hit the planet

            // Apply the collision effects
            PlanetController planet = collision.GetComponent<PlanetController>();
            if (planet != null)
            {
                // Send a trigger event to the planet to handle the shake and explosion
                planet.SendMessage("OnTriggerEnter2D", GetComponent<Collider2D>());
            }

            // Destroy the projectile after the collision
            Destroy(gameObject);
        }

        if (collision.CompareTag("Enemy") || collision.CompareTag("Debris"))
        {
            // Apply damage to health component
            HealthComponent health = collision.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // Try to award score through enemy controller
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

            // No explosion here - let the enemy handle it
            
            // Destroy the projectile
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