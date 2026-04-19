using UnityEngine;

public class PersistentSFXPlayer : MonoBehaviour
{
    public static PersistentSFXPlayer Instance;

    private AudioSource audioSource;

    private void Awake()
    {
        // Keep only one copy alive
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.ignoreListenerPause = true;
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        audioSource.PlayOneShot(clip, volume);
    }
}