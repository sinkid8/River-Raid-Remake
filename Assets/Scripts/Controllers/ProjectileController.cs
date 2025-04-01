using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private bool isLaser = true; // True for laser, false for missile
    [SerializeField] private int scoreValue = 10; // Default score value for destroyed enemies
    [SerializeField] private bool showDebugMessages = true;

    private bool hasCollided = false;

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
        // Prevent multiple collision processing
        if (hasCollided)
            return;
            
        if (showDebugMessages)
        {
            Debug.Log($"Projectile hit: {collision.gameObject.name}, tag: {collision.tag}");
        }
        
        // Mark as collided
        hasCollided = true;
        
        // Handle planet hits differently
        if (collision.CompareTag("Planet"))
        {
            // Regular projectiles do nothing to planets
            if (isLaser)
            {
                // Just destroy the projectile
                Destroy(gameObject);
                return;
            }
        }

        if (collision.CompareTag("Enemy") || collision.CompareTag("Debris"))
        {
            // Try the new ImprovedEnemyController first
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Apply damage through the improved controller
                enemy.TakeDamage(damage);
            }

            // Destroy the projectile
            Destroy(gameObject);
        }

        // Special handling for batteries
        if (collision.CompareTag("Battery"))
        {
            // Ignore collision with battery
            Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>());
            hasCollided = false; // Reset to allow further collisions
        }
    }
    
    // Getter for damage value
    public int GetDamage()
    {
        return damage;
    }
}