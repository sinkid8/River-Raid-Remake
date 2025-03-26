using UnityEngine;

public class EnergyProjectileController : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 30; // Higher damage than regular projectiles
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float scale = 1.5f; // Larger projectile
    [SerializeField] private Color projectileColor = Color.cyan; // Distinctive color

    // Optional visual effects
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private ParticleSystem particles;

    private void Start()
    {
        Destroy(gameObject, lifeTime);

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
        if (collision.CompareTag("Enemy") || collision.CompareTag("Debris"))
        {
            // Apply damage to health component
            HealthComponent health = collision.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // Add score if it's an enemy
            if (collision.CompareTag("Enemy") && GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(25); // More points for energy weapon
            }

            // Create explosion
            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

                // Scale up the explosion
                explosion.transform.localScale *= 1.5f;
            }

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