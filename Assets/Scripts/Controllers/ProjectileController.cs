using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private bool isLaser = true;

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
        if (collision.CompareTag("Enemy") || collision.CompareTag("Obstacle"))
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
    }
}