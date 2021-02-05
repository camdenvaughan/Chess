using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    // Name of sound
    public string name;

    // File
    public AudioClip clip;

    // Volume
    [Range(0f, 1f)]
    public float volume;

    // Pitch
    [Range(.1f, 3f)]
    public float pitch;

    // Loop
    public bool loop;

    public AudioMixerGroup group;

    // AudioSource Component
    [HideInInspector] public AudioSource source;

}