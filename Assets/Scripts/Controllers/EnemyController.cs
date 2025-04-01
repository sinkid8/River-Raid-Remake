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
    public UnityEvent<int, int> OnDamaged = new UnityEvent<int, int>(); 
    
    // Internal state
    private int currentHealth;
    private EnemyScoreHandler scoreHandler;
    private bool isDestroyed = false;
    
    private void Start()
    {
        currentHealth = maxHealth;

        scoreHandler = GetComponent<EnemyScoreHandler>();
        if (scoreHandler == null)
        {
            scoreHandler = gameObject.AddComponent<EnemyScoreHandler>();
        }
        
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
        if (isDestroyed)
            return;
        
        if (showDebugMessages)
        {
            Debug.Log($"Collision detected on {gameObject.name} with {collision.gameObject.name}");
        }
            
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

        if (collision.CompareTag("Player"))
        {
            ShipCollision playerCollision = collision.GetComponent<ShipCollision>();
            DestroyEnemy();
        }
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) 
            return;
        
        currentHealth -= damage;
        
        if (showDebugMessages)
        {
            Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        }

        OnDamaged.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            DestroyEnemy();
        }
    }
    
    public void DestroyEnemy()
    {
        if (isDestroyed)
            return;
            
        isDestroyed = true;
        
        if (showDebugMessages)
        {
            Debug.Log($"Destroying enemy: {gameObject.name}");
        }
            
        if (scoreHandler != null)
        {
            scoreHandler.AwardScore();
        }
        else if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.explosionClip);
        }
        
        OnDeath.Invoke();

        Destroy(gameObject);
    }
    
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