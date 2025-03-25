using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private bool isLaser = true; // True for laser, false for missile
    [SerializeField] private GameObject explosionPrefab;  // Explosion prefab

    private void Start()
    {
        // Destroy projectile after lifeTime seconds
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Move projectile forward (assuming the ship fires upward)
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the projectile hit an enemy or obstacle
        if (collision.CompareTag("Enemy") || collision.CompareTag("Obstacle"))
        {
            // Try to get the health component and damage it
            HealthComponent health = collision.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            // Add score if it's an enemy
            if (collision.CompareTag("Enemy") && GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(10); // Base score value, can be adjusted
            }

            // Destroy the projectile
            Destroy(gameObject);
        }

        // Check if the projectile hits debris
        if (collision.CompareTag("Debris"))
        {
            // Trigger explosion at the debris' position
            TriggerExplosion(collision.transform.position);

            // Destroy the debris
            Destroy(collision.gameObject);

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

    private void TriggerExplosion(Vector2 position)
    {
        // Instantiate explosion at the debris' position
        Instantiate(explosionPrefab, position, Quaternion.identity);
    }
}
