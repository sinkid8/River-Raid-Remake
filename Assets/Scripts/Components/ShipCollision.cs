using UnityEngine;
using UnityEngine.Events;

public class ShipCollision : MonoBehaviour
{
    public GameObject explosionPrefab;
    public FuelManager fuelManager;
    
    public UnityEvent OnShipDestroyed = new UnityEvent();
    
    private bool isDestroyed = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDestroyed)
            return;
            
        if (AudioManager.instance != null && collision.gameObject.CompareTag("Planet"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.bigExplosionClip);
        }
        else if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.explosionClip);
        }
        
        if (collision.gameObject.CompareTag("Debris"))
        {
            DestroyShip();
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            DestroyShip();
        }

        if (collision.gameObject.CompareTag("Planet"))
        {
            DestroyShip();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroyed)
            return;
            
        if (collision.CompareTag("Battery"))
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySound(AudioManager.instance.powerupClip);
            }
            if (fuelManager != null)
            {
                fuelManager.AddFuel();
            }
            else
            {
                Debug.LogWarning("FuelManager not assigned to ShipCollision.");
            }
            Destroy(collision.gameObject);
        }
    }

    private void DestroyShip()
    {

        if (isDestroyed)
            return;
            
        isDestroyed = true;
        TriggerExplosion();
        OnShipDestroyed.Invoke();
        Destroy(gameObject);
    }

    private void TriggerExplosion()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
}