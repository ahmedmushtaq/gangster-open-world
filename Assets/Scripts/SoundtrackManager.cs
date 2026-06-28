using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundtrackManager : MonoBehaviour
{
    // ---- Singleton ----
    private static SoundtrackManager instance;
    public static SoundtrackManager Instance
    {
        get
        {
            if (instance == null)
            {
                // Try to find an existing instance
                instance = FindObjectOfType<SoundtrackManager>();
                if (instance == null)
                {
                    // Create a new GameObject with this component if none exists
                    GameObject go = new GameObject("SoundtrackManager");
                    instance = go.AddComponent<SoundtrackManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    // ---- Audio Source ----
    private AudioSource soundtrackSource;

    // ---- Clip Lists ----
    [Header("Soundtrack Lists")]
    public List<AudioClip> showroomSoundtracks = new List<AudioClip>();   // used for menu/garage
    public List<AudioClip> gameplaySoundtracks = new List<AudioClip>();   // used during missions

    [Header("Settings")]
    [Range(0f, 1f)] public float maximumVolume = 1f;
    public bool ignoreListenerPause = false;  // if true, music continues when game is paused

    // ---- Internal state ----
    private AudioClip lastPlayedGameplayClip = null;  // avoids repeating the same track in a row

    // ---- Volume cache (for PlayerPrefs) ----
    private float currentMusicVolume = 0.5f;

    private void Awake()
    {
        // Singleton enforcement
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Get or create AudioSource
        soundtrackSource = GetComponent<AudioSource>();
        if (soundtrackSource == null)
            soundtrackSource = gameObject.AddComponent<AudioSource>();

        soundtrackSource.loop = true;
        soundtrackSource.ignoreListenerPause = ignoreListenerPause;
        soundtrackSource.playOnAwake = false;

        // Load saved volume and apply
        LoadVolumeSettings();
        ApplyMusicVolume(currentMusicVolume);

        // Start with nothing playing
        StopMusic();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ---- Scene change handler ----
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // You can define your own scene indices or use scene names
        // Example: scene.buildIndex == 1 => main menu; others => gameplay
        // Adjust to your own scene setup.
        RefreshMusicForScene(scene.buildIndex);
    }

    private void RefreshMusicForScene(int sceneIndex)
    {
        // ---------- Main Menu / Showroom ----------
        // Here we assume scene index 1 is the main menu (change as needed)
        if (sceneIndex == 1)
        {
            if (showroomSoundtracks.Count > 0)
            {
                AudioClip randomClip = showroomSoundtracks[Random.Range(0, showroomSoundtracks.Count)];
                PlayClip(randomClip);
            }
            return;
        }

        // ---------- Gameplay ----------
        if (gameplaySoundtracks.Count > 0)
        {
            AudioClip newClip = GetRandomGameplayClipExcluding(lastPlayedGameplayClip);
            if (newClip != null)
            {
                PlayClip(newClip);
                lastPlayedGameplayClip = newClip;
            }
        }
    }

    // ---- Clip selection logic ----
    private AudioClip GetRandomGameplayClipExcluding(AudioClip excludeClip)
    {
        if (gameplaySoundtracks.Count == 0) return null;

        if (gameplaySoundtracks.Count == 1)
            return gameplaySoundtracks[0];

        // Build a list of available clips (excluding the last played one)
        List<AudioClip> available = new List<AudioClip>(gameplaySoundtracks);
        if (excludeClip != null && available.Contains(excludeClip))
            available.Remove(excludeClip);

        // Fallback if all clips were the same as excludeClip
        if (available.Count == 0)
            available = new List<AudioClip>(gameplaySoundtracks);

        return available[Random.Range(0, available.Count)];
    }

    // ---- Public playback controls ----
    public void PlayClip(AudioClip clip)
    {
        if (clip == null) return;

        if (soundtrackSource.clip != clip)
            soundtrackSource.clip = clip;

        if (!soundtrackSource.isPlaying)
            soundtrackSource.Play();
    }

    public void StopMusic()
    {
        soundtrackSource.Stop();
        soundtrackSource.clip = null;
    }

    // ---- Volume control ----
    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = Mathf.Clamp01(volume);
        ApplyMusicVolume(currentMusicVolume);
        SaveVolumeSettings();
    }

    private void ApplyMusicVolume(float volume)
    {
        if (soundtrackSource != null)
        {
            soundtrackSource.volume = Mathf.Min(volume, maximumVolume);
        }
    }

    // ---- Save / Load ----
    private void LoadVolumeSettings()
    {
        currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MusicVolume", currentMusicVolume);
        PlayerPrefs.Save();
    }

    // ---- Optional: Pause/Resume music (if not using ignoreListenerPause) ----
    public void PauseMusic()
    {
        if (soundtrackSource.isPlaying)
            soundtrackSource.Pause();
    }

    public void ResumeMusic()
    {
        if (!soundtrackSource.isPlaying && soundtrackSource.clip != null)
            soundtrackSource.Play();
    }
}