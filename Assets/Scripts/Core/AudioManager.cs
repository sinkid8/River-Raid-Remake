using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]

    public AudioSource sfxSource;
    public AudioSource ambientSource;
    public AudioSource levelupSource;

    [Header("Audio Clips")]

    public AudioClip laserClip;
    public AudioClip energyProjectileClip;
    public AudioClip ambientClip;
    public AudioClip explosionClip;
    public AudioClip bigExplosionClip;
    public AudioClip powerupClip;
    public AudioClip levelupClip;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayLevelupSound(AudioClip clip)
    {
        if (clip != null)
        {
            levelupSource.PlayOneShot(clip);
        }
    }

    void Start()
    {
        PlayGameSound();
    }

    public void PlayGameSound()
    {
        if (ambientSource != null && ambientClip != null)
        {
            ambientSource.clip = ambientClip;
            ambientSource.loop = true;
            ambientSource.Play();
        }
    }
}
