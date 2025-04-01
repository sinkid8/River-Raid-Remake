using UnityEngine;
using System.Collections;

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
        // Set a timer to destroy this projectile after lifeTime seconds
        Destroy(gameObject, lifeTime);
        
        // Play sound effect if available
        if (AudioManager.instance != null && isLaser)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.laserClip);
        }
        
        // Temporarily disable the collider to prevent immediate collision with the ship
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            StartCoroutine(EnableColliderAfterDelay(0.1f, collider));
        }
        
        // Ignore collisions with the player ship
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            Collider2D projectileCollider = GetComponent<Collider2D>();
            
            if (playerCollider != null && projectileCollider != null)
            {
                Physics2D.IgnoreCollision(playerCollider, projectileCollider, true);
            }
        }
    }

    private IEnumerator EnableColliderAfterDelay(float delay, Collider2D collider)
    {
        yield return new WaitForSeconds(delay);
        collider.enabled = true;
    }

    private void Update()
    {
        // Move the projectile forward
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
        
        // Skip collisions with the player's ship
        if (collision.CompareTag("Player"))
        {
            return;
        }
        
        // Mark as collided
        hasCollided = true;
        
        // Handle collision with battery - allow projectile to pass through
        if (collision.CompareTag("Battery"))
        {
            // Reset the collision flag to allow further collisions
            hasCollided = false;
            return;
        }
        
        // Handle planet hits
        if (collision.CompareTag("Planet"))
        {
            // Just destroy the projectile
            Destroy(gameObject);
            return;
        }

        // Handle enemy and debris hits
        if (collision.CompareTag("Enemy") || collision.CompareTag("Debris"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Apply damage
                enemy.TakeDamage(damage);
            }

            // Destroy the projectile
            Destroy(gameObject);
            return;
        }
        
        // For any other collision, destroy the projectile
        Destroy(gameObject);
    }
    
    // Getter for damage value
    public int GetDamage()
    {
        return damage;
    }
}