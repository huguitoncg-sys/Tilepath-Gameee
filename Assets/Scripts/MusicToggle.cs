using UnityEngine;

public class MusicToggle : MonoBehaviour
{
    public static MusicToggle instance;

    private AudioSource audioSource;
    private bool musicOn = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.loop = true;
            audioSource.Play();
            musicOn = true;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMusic();
        }
    }

    void ToggleMusic()
    {
        if (audioSource == null) return;

        if (musicOn)
        {
            audioSource.Pause();
            musicOn = false;
        }
        else
        {
            audioSource.UnPause();
            musicOn = true;
        }
    }
}