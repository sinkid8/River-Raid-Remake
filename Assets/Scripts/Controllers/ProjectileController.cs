using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 3f;
    // [SerializeField] private bool isLaser = true; // True for laser, false for missile
    [SerializeField] private GameObject explosionPrefab;  // Explosion prefab

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Debris"))
        {
            HealthComponent health = collision.GetComponent<HealthComponent>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            if (collision.CompareTag("Enemy") && GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(10);
            }

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