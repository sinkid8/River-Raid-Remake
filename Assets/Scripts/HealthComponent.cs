using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private GameObject destroyedEffect; // Optional explosion prefab

    private int currentHealth;

    public UnityEvent OnDeath = new UnityEvent();

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Trigger the death event
        OnDeath.Invoke();

        // Spawn destroy effect if assigned
        if (destroyedEffect != null)
        {
            Instantiate(destroyedEffect, transform.position, Quaternion.identity);
        }

        // Destroy the game object
        Destroy(gameObject);
    }
}