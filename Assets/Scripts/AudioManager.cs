/**
 * Copyright (c) Nelson Laracuente
 * http://nlaracuente.com
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the information for a sound clip such as volume and clip itself
/// </summary>
[System.Serializable]
public class SoundClip
{
    /// <summary>
    /// The name of the sound clip
    /// </summary>
    public string soundName;

    /// <summary>
    /// volume level to play this clip at
    /// </summary>
    [Range(0, 1)]
    public float volume = 1f;

    /// <summary>
    /// The audio clip component
    /// </summary>
    public AudioClip clip;

    /// <summary>
    /// References the audio source component associated with this clip
    /// </summary>
    AudioSource source;

    /// <summary>
    /// Create the AudioSource and assign it the clio
    /// </summary>
    public void SetSource(AudioSource source)
    {
        this.source = source;
        this.source.playOnAwake = false;
        this.source.clip = this.clip;
    }

    /// <summary>
    /// Plays this sound clip if not already playing
    /// </summary>
    public void Play()
    {
        this.source.volume = this.volume;
        this.source.Play();
    }

    /// <summary>
    /// Stops playback
    /// </summary>
    public void Stop()
    {
        this.source.Stop();
    }

    /// <summary>
    /// Plays this sound clip if not already playing
    /// </summary>
    public void Loop()
    {
        this.source.loop = true;
        this.Play();
    }

    /// <summary>
    /// Changes the volume directly on the source to 
    /// affect any clips playing
    /// </summary>
    /// <param name="volume"></param>
    public void ChangeVolume(float volume)
    {
        this.source.volume = volume;
    }

    /// <summary>
    /// Changes the volume to 0 to mute the sound
    /// </summary>
    public void Mute()
    {
        this.ChangeVolume(0f);
    }

    /// <summary>
    /// Resets the audio source's volume to the clips origin volume
    /// </summary>
    public void UnMute()
    {
        this.ChangeVolume(this.volume);
    }
}

/// <summary>
/// Controls playing the sounds for the game
/// There's only ever one of the audio manager (singleton)
/// Creates AudioSources for each sound so that all sounds can be independent from each other
/// </summary>
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// A reference to the AudioManager singleton
    /// </summary>
    public static AudioManager instance;

    /// <summary>
    /// Contains a list of all the sound clips to use
    /// </summary>
    [SerializeField]
    SoundClip[] clips;

    /// <summary>
    /// A references to the 
    /// </summary>
    Dictionary<string, SoundClip> soundClips = new Dictionary<string, SoundClip>();

    /// <summary>
    /// Singleton setup
    /// </summary>
    void Update()
    {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Creates game objects for all of the sound clips
    /// </summary>
    void Start()
    {
        foreach (SoundClip clip in this.clips) {
            // store a reference so that we can invoke it later
            this.soundClips[clip.soundName] = clip;
        }
    }

    /// <summary>
    /// Spawns a new object with an AudioSource to fire off the sound given
    /// Thus preventing the same sound trying to play twice from the same audio source
    /// Triggers the destruction of the object based on the clip's length
    /// </summary>
    /// <param name="soundName"></param>
    public void PlaySound(string soundName)
    {
        if (this.soundClips.ContainsKey(soundName)) {
            SoundClip soundClip = this.soundClips[soundName];
            GameObject soundGO = new GameObject(soundClip.soundName);            
            soundGO.AddComponent(typeof(SoundClip));

            soundClip.SetSource(soundGO.AddComponent<AudioSource>());
            soundClip.Play();
            
            Destroy(soundGO, soundClip.clip.length);
        }
    }

    /// <summary>
    /// Plays the given sound if it exists
    /// </summary>
    /// <param name="soundName"></param>
    public void LoopSound(string soundName)
    {
        if (this.soundClips.ContainsKey(soundName)) {
            this.soundClips[soundName].Loop();
        }
    }

    /// <summary>
    /// Stops the given sound if it exists from playing
    /// </summary>
    /// <param name="soundName"></param>
    public void StopSound(string soundName)
    {
        if (this.soundClips.ContainsKey(soundName)) {
            this.soundClips[soundName].Stop();
        }
    }
}