using UnityEngine;
using System.Collections;

public class EnergyProjectileController : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private int damage = 30; 
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private float scale = 1.5f; 
    [SerializeField] private Color projectileColor = Color.cyan; 

    private bool hasCollided = false;

    private void Start()
    {
        Destroy(gameObject, lifeTime);

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.energyProjectileClip);
        }
        
        transform.localScale *= scale;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = projectileColor;
        }
        
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
            StartCoroutine(EnableColliderAfterDelay(0.3f, collider));
        }
        
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
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasCollided)
            return;
        
        if (collision.CompareTag("Player"))
        {
            return;
        }

        hasCollided = true;

        if (collision.CompareTag("Battery"))
        {
            hasCollided = false;
            return;
        }

        if (collision.CompareTag("Planet"))
        {
            PlanetController planet = collision.GetComponent<PlanetController>();
            if (planet != null)
            {
                planet.OnTriggerEnter2D(GetComponent<Collider2D>());
            }

            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySound(AudioManager.instance.energyProjectileClip);
            }

            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Enemy") || collision.CompareTag("Debris"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            
            Destroy(gameObject);
            return;
        }
        
        Destroy(gameObject);
    }
    
    public int GetDamage()
    {
        return damage;
    }
}