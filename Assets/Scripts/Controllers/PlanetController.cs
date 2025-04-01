using UnityEngine;
using System.Collections;

public class PlanetController : MonoBehaviour
{
    private int shotCount = 0;

    // Shake parameters
    [SerializeField] private float shakeIntensity = 0.1f; // How much the planet shakes
    [SerializeField] private float shakeDuration = 0.5f;  // How long the shake lasts

    private Vector3 originalPosition;
    private void Start()
    {
        // Save the planet's original position
        originalPosition = transform.position;
    }

    // Changed from private to public so it can be called from EnergyProjectileController
    public void OnTriggerEnter2D(Collider2D other)
    {
        // First, check if this is a normal projectile - if so, do nothing to the planet
        ProjectileController normalProjectile = other.GetComponent<ProjectileController>();
        if (normalProjectile != null)
        {
            // Just destroy the normal projectile with no effect on the planet
            Destroy(other.gameObject);
            Debug.Log("Regular projectile hit planet - DESTROYED WITH NO EFFECT");
            return;
        }
        
        // Only energy projectiles can damage planets - explicit check for energy projectile component
        EnergyProjectileController energyProjectile = other.GetComponent<EnergyProjectileController>();
        if (energyProjectile != null)
        {
            shotCount++;
            Debug.Log("Planet hit by energy projectile! Shot count: " + shotCount);

            if (shotCount == 1)
            {
                // Apply shaking effect when first shot is fired
                ShakePlanet();
            }
            else if (shotCount >= 2)
            {
                // Explode the planet on the second shot
                ExplodePlanet();
            }

            // Destroy the energy projectile after it hits
            Destroy(other.gameObject);
        }
    }

    private void ShakePlanet()
    {
        // Start the shaking effect with a Coroutine
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Apply random shake offset
            Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;
            transform.position = originalPosition + randomOffset;

            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Reset the position after shaking
        transform.position = originalPosition;
    }

    private void ExplodePlanet()
    {
        // Add explosion effect or destroy the planet
        Debug.Log("Planet exploded by energy weapon!");
        
        // Optional: Add a visual explosion effect here
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.bigExplosionClip);
        }
        
        Destroy(gameObject); // Destroy the planet
    }
}