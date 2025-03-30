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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnergyProjectile"))
        {
            shotCount++;

            if (shotCount == 1)
            {
                // Apply shaking effect when first shot is fired
                ShakePlanet();
            }
            else if (shotCount == 2)
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
        Debug.Log("Planet exploded!");
        Destroy(gameObject); // Destroy the planet
    }
}
