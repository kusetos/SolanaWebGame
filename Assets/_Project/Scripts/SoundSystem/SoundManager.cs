using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sound Library")]
    public SoundSO[] sounds; // Assign all your SoundSO assets here

    private Dictionary<string, SoundSO> soundDict;
    private AudioSource musicSource;
    private List<AudioSource> sfxSources = new List<AudioSource>();

    void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Init
        soundDict = new Dictionary<string, SoundSO>();
        foreach (var sound in sounds)
        {
            if (!soundDict.ContainsKey(sound.soundName))
                soundDict.Add(sound.soundName, sound);
        }

        // Create AudioSources
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        Play("back_music");
    }

    // Play sound by name
    public void Play(string name)
    {
        if (!soundDict.TryGetValue(name, out SoundSO sound))
        {
            Debug.LogWarning($"Sound '{name}' not found!");
            return;
        }

        if (sound.isMusic)
        {
            PlayMusic(sound);
        }
        else
        {
            PlaySFX(sound);
        }
    }

    // Stop sound by name
    public void Stop(string name)
    {
        if (!soundDict.TryGetValue(name, out SoundSO sound)) return;

        if (sound.isMusic)
            musicSource.Stop();
        else
        {
            // Stop all SFX sources playing this clip
            foreach (var src in sfxSources)
                if (src.isPlaying && src.clip == sound.clips[0])
                    src.Stop();
        }
    }

    private void PlayMusic(SoundSO sound)
    {
        if (musicSource.clip == sound.clips[0] && musicSource.isPlaying)
            return;

        musicSource.clip = sound.clips[0];
        musicSource.volume = sound.volume;
        musicSource.loop = sound.loop;
        musicSource.pitch = 1f;
        musicSource.Play();
    }

    private HashSet<string> currentlyPlaying = new HashSet<string>();
    private void PlaySFX(SoundSO sound)
    {
        if (sound.clips.Length == 0) return;
        if (currentlyPlaying.Contains(sound.soundName)) return;
        currentlyPlaying.Add(sound.soundName);


        AudioSource src = GetFreeSFXSource();

        // Pick random clip
        AudioClip clip = sound.clips[Random.Range(0, sound.clips.Length)];

        // // Randomize pitch and volume
        // float pitch = sound.pitch + Random.Range(-sound.pitchVariation, sound.pitchVariation);
        // float vol = sound.volume + Random.Range(-sound.volumeVariation, sound.volumeVariation);

        src.pitch = sound.pitch;
        src.volume = Mathf.Clamp01(sound.volume);
        src.loop = sound.loop;
        src.clip = clip;
        src.Play();

        StartCoroutine(ReleaseLockAfter(src, clip.length, sound.soundName));
    }

    private AudioSource GetFreeSFXSource()
    {
        // Try to find an available one
        foreach (var src in sfxSources)
            if (!src.isPlaying)
                return src;

        // Create new one if none free
        var newSrc = gameObject.AddComponent<AudioSource>();
        sfxSources.Add(newSrc);
        return newSrc;
    }
    private IEnumerator ReleaseLockAfter(AudioSource src, float delay, string name)
    {
        yield return new WaitForSeconds(delay);
        currentlyPlaying.Remove(name);
    }
    private System.Collections.IEnumerator DestroyAfterPlay(AudioSource src, float delay)
    {
        yield return new WaitForSeconds(delay);
        src.Stop();
    }
}
