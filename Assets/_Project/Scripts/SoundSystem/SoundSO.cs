using UnityEngine;

[CreateAssetMenu(fileName = "NewSound", menuName = "Audio/Sound")]
public class SoundSO : ScriptableObject
{
    [Header("Basic Info")]
    public string soundName;          // Name used to call from code
    public AudioClip[] clips;         // Multiple clips for random variation

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    [Range(0f, 0.3f)] public float pitchVariation = 0.05f;
    [Range(0f, 0.3f)] public float volumeVariation = 0.05f;

    [Header("Type")]
    public bool isMusic = false;
    public bool loop = false;
}
