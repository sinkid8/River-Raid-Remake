using UnityEngine;

public class ShipCollision : MonoBehaviour
{
    public GameObject explosionPrefab;
    public FuelManager fuelManager; // Reference to FuelManager

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (AudioManager.instance != null && collision.gameObject.CompareTag("Planet"))
        {
            AudioManager.instance.PlaySound(AudioManager.instance.bigExplosionClip);
        }
        else if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.explosionClip);
        }
        // Check if the ship collides with debris
        if (collision.gameObject.CompareTag("Debris"))
        {
            TriggerExplosion();
            Destroy(gameObject);  // Destroy the ship
        }

       
        if (collision.gameObject.CompareTag("Wall"))
        {
            TriggerExplosion();
            Destroy(gameObject);  // Destroy the ship
        }

        if (collision.gameObject.CompareTag("Planet"))
        {
            TriggerExplosion();
            Destroy(gameObject);  // Destroy the ship
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the ship collides with a battery
        if (collision.CompareTag("Battery"))
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySound(AudioManager.instance.powerupClip);
            }
            // Add fuel when collecting battery
            if (fuelManager != null)
            {
                fuelManager.AddFuel();
            }
            else
            {
                Debug.LogWarning("FuelManager not assigned to ShipCollision.");
            }

            // Destroy the battery after collecting it
            Destroy(collision.gameObject);
        }
    }

    private void TriggerExplosion()
    {
        // Instantiate explosion at ship's position
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
}
