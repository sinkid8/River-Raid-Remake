using UnityEngine;

public class EnergyProjectileController : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 30; // Higher damage than regular projectiles
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private float scale = 1.5f; // Larger projectile
    [SerializeField] private Color projectileColor = Color.cyan; // Distinctive color
    [SerializeField] private int scoreValue = 25; // Higher score value for energy weapon kills
    [SerializeField] private bool showDebugMessages = true;

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
            Debug.Log($"Energy projectile hit: {collision.gameObject.name}, tag: {collision.tag}");
        }
        
        // Mark as collided
        hasCollided = true;

        // Special handling for planets
        if (collision.CompareTag("Planet"))
        {
            // Energy weapons can affect planets
            PlanetController planet = collision.GetComponent<PlanetController>();
            if (planet != null)
            {
                // Let the planet handle the hit
                // The planet controller will count shots and trigger effects
            }

            // Play special effect for planet hit
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySound(AudioManager.instance.energyProjectileClip);
            }

            // Destroy the projectile after the collision
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Enemy") || collision.CompareTag("Debris"))
        {
            // Try the new ImprovedEnemyController first
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Apply higher damage through the improved controller
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