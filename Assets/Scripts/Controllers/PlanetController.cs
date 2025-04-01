using UnityEngine;
using System.Collections;

public class PlanetController : MonoBehaviour
{
    [SerializeField] private int requiredShotCount = 2;
    private int shotCount = 0;

    // Debug flag for troubleshooting
    [SerializeField] private bool showDebugMessages = true;

    // Shake parameters
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float upwardShiftAfterFirstShot = 0.2f;


    private Vector3 originalPosition;
    private bool isShaking = false;
    
    private System.Collections.Generic.HashSet<int> hitProjectiles = new System.Collections.Generic.HashSet<int>();

    private void Start()
    {
        originalPosition = transform.position;
        
        shotCount = 0;
        
        if (showDebugMessages)
        {
            Debug.Log("Planet initialized. Required shots to destroy: " + requiredShotCount);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        int projectileID = other.gameObject.GetInstanceID();
        
        if (hitProjectiles.Contains(projectileID))
        {
            if (showDebugMessages)
            {
                Debug.Log("Duplicate hit detected from projectile ID: " + projectileID);
            }
            return;
        }
        
        // Add to hit list
        hitProjectiles.Add(projectileID);

        ProjectileController normalProjectile = other.GetComponent<ProjectileController>();
        if (normalProjectile != null)
        {
            Destroy(other.gameObject);
            return;
        }
        
        EnergyProjectileController energyProjectile = other.GetComponent<EnergyProjectileController>();
        if (energyProjectile != null)
        {
            shotCount++;
            
            if (showDebugMessages)
            {
                Debug.Log("Planet hit by energy projectile! Shot count: " + shotCount + " / " + requiredShotCount);
            }

            if (shotCount == 1)
            {
                ShakePlanet();
            }
            else if (shotCount >= requiredShotCount)
            {
                ExplodePlanet();
            }
            else
            {
                ShakePlanet();
            }

        }
    }

    private void ShakePlanet()
    {
        if (isShaking)
            return;

        StartCoroutine(ShakeCoroutine());
    }
    private IEnumerator ShakeCoroutine()
    {
        isShaking = true;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity;
            transform.position = originalPosition + randomOffset;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move the planet upward slightly after first shot
        if (shotCount == 1)
        {
            originalPosition += new Vector3(0f, upwardShiftAfterFirstShot, 0f);
        }

        transform.position = originalPosition;
        isShaking = false;
    }


    private void ExplodePlanet()
    {
        if (showDebugMessages)
        {
            Debug.Log("Planet exploded by energy weapon after " + shotCount + " shots!");
        }
        
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.bigExplosionClip);
        }
        
        Destroy(gameObject);
    }
}