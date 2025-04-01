using UnityEngine;
using UnityEngine.Events;

public class EnemyController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private bool showDebugMessages = true;
    
    [Header("Enemy Settings")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private int scoreValue = 10;
    
    // Events
    public UnityEvent OnDeath = new UnityEvent();
    public UnityEvent<int, int> OnDamaged = new UnityEvent<int, int>(); // Passes current and max health
    
    // Internal state
    private int currentHealth;
    private EnemyScoreHandler scoreHandler;
    private bool isDestroyed = false;
    
    private void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        
        // Initialize score handler
        scoreHandler = GetComponent<EnemyScoreHandler>();
        if (scoreHandler == null)
        {
            // Add score handler if not present
            scoreHandler = gameObject.AddComponent<EnemyScoreHandler>();
        }
        
        // Set the score handler's score value to match this enemy's score value
        if (scoreHandler != null)
        {
            scoreHandler.SetScoreValue(scoreValue);
        }
        
        if (showDebugMessages)
        {
            Debug.Log($"Enemy initialized: {gameObject.name}, Health = {currentHealth}/{maxHealth}, Score = {scoreValue}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Avoid processing if already destroyed
        if (isDestroyed)
            return;
        
        if (showDebugMessages)
        {
            Debug.Log($"Collision detected on {gameObject.name} with {collision.gameObject.name}");
        }
            
        // Handle projectile collisions
        if (collision.GetComponent<ProjectileController>() != null)
        {
            ProjectileController projectile = collision.GetComponent<ProjectileController>();
            TakeDamage(projectile.GetDamage());
        }
        else if (collision.GetComponent<EnergyProjectileController>() != null) 
        {
            EnergyProjectileController energyProjectile = collision.GetComponent<EnergyProjectileController>();
            TakeDamage(energyProjectile.GetDamage());
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

            // Destroy self - immediate death on player collision
            DestroyEnemy();
        }
    }
    
    // Public method to apply damage
    public void TakeDamage(int damage)
    {
        // Ensure damage is positive
        if (damage <= 0) 
            return;
        
        // Apply damage
        currentHealth -= damage;
        
        if (showDebugMessages)
        {
            Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        }
        
        // Trigger OnDamaged event with current and max health
        OnDamaged.Invoke(currentHealth, maxHealth);

        // Check for death
        if (currentHealth <= 0)
        {
            DestroyEnemy();
        }
    }
    
    // Handle enemy destruction - can be called directly or via health
    public void DestroyEnemy()
    {
        // Avoid multiple calls
        if (isDestroyed)
            return;
            
        isDestroyed = true;
        
        if (showDebugMessages)
        {
            Debug.Log($"Destroying enemy: {gameObject.name}");
        }
            
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
        
        // Trigger death event
        OnDeath.Invoke();

        // Destroy the enemy gameObject
        Destroy(gameObject);
    }
    
    // Helper methods to get health info
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public int GetMaxHealth()
    {
        return maxHealth;
    }
    
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
}