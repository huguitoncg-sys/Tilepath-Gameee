using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Clips")]
    public AudioClip titleLoop;
    public AudioClip levelSelectLoop;
    public AudioClip loop4x4;
    public AudioClip loop5x5;
    public AudioClip loop6x6;
    public AudioClip loop7x7;
    public AudioClip loop8x8;

    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    public float fadeTime = 1.25f;

    private AudioSource sourceA;
    private AudioSource sourceB;

    private AudioSource activeSource;
    private AudioSource inactiveSource;

    private AudioClip currentClip;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        // Keep only one MusicManager alive
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create two AudioSources for smooth crossfades
        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();

        SetupSource(sourceA);
        SetupSource(sourceB);

        activeSource = sourceA;
        inactiveSource = sourceB;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        ChooseMusicForCurrentScene();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void SetupSource(AudioSource source)
    {
        source.loop = true;
        source.playOnAwake = false;
        source.volume = 0f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChooseMusicForCurrentScene();
    }

    private void ChooseMusicForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        // Change these names if your scenes are named differently
        if (sceneName == "MainMenu")
        {
            PlayMusic(titleLoop);
        }
        else if (sceneName == "WorldSelect" || sceneName == "LevelSelect")
        {
            PlayMusic(levelSelectLoop);
        }
        else if (sceneName == "Game")
        {
            PlayLevelSizeMusic();
        }
    }

    public void PlayTitleMusic()
    {
        PlayMusic(titleLoop);
    }

    public void PlayLevelSelectMusic()
    {
        PlayMusic(levelSelectLoop);
    }

    public void PlayLevelSizeMusic()
    {
        AudioClip clipToPlay = GetClipForSelectedLevel();

        if (clipToPlay != null)
        {
            PlayMusic(clipToPlay);
        }
    }

    private AudioClip GetClipForSelectedLevel()
    {
        if (GameManager.Instance == null || GameManager.Instance.SelectedLevel == null)
        {
            return loop4x4;
        }

        LevelData level = GameManager.Instance.SelectedLevel;

        int height = level.rows.Length;
        int width = level.rows[0].Length;

        int size = Mathf.Max(width, height);

        switch (size)
        {
            case 4:
                return loop4x4;

            case 5:
                return loop5x5;

            case 6:
                return loop6x6;

            case 7:
                return loop7x7;

            case 8:
                return loop8x8;

            default:
                return loop4x4;
        }
    }

    private void PlayMusic(AudioClip newClip)
    {
        if (newClip == null)
        {
            Debug.LogWarning("MusicManager: Tried to play a missing music clip.");
            return;
        }

        // Do not restart the same song
        if (currentClip == newClip)
        {
            return;
        }

        currentClip = newClip;

        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(CrossfadeTo(newClip));
    }

    private IEnumerator CrossfadeTo(AudioClip newClip)
    {
        inactiveSource.clip = newClip;
        inactiveSource.volume = 0f;
        inactiveSource.Play();

        float timer = 0f;
        float startingVolume = activeSource.volume;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / fadeTime;

            activeSource.volume = Mathf.Lerp(startingVolume, 0f, t);
            inactiveSource.volume = Mathf.Lerp(0f, musicVolume, t);

            yield return null;
        }

        activeSource.Stop();
        activeSource.volume = 0f;

        inactiveSource.volume = musicVolume;

        AudioSource temp = activeSource;
        activeSource = inactiveSource;
        inactiveSource = temp;
    }
}