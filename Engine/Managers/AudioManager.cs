using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace SlimeTogetherStrong.Engine.Managers;

public class AudioManager
{
    public static AudioManager Instance { get; private set; } = new AudioManager();
    
    private float _bgmVolume = 0.7f;
    private float _sfxVolume = 0.7f;
    
    public float BGMVolume
    {
        get => _bgmVolume;
        set
        {
            _bgmVolume = Math.Clamp(value, 0f, 1f);
            MediaPlayer.Volume = _bgmVolume;
        }
    }
    
    public float SFXVolume
    {
        get => _sfxVolume;
        set => _sfxVolume = Math.Clamp(value, 0f, 1f);
    }
    
    // Legacy property for backward compatibility
    public static float Volume
    {
        get => MediaPlayer.Volume;
        set => MediaPlayer.Volume = Math.Clamp(value, 0f, 1f);
    }
    
    
    private readonly Dictionary<string, SoundEffect> _soundsEffects = [];
    private readonly Dictionary<string, Song> _songs = [];
    
    private AudioManager() 
    {
        // Initialize MediaPlayer volume
        MediaPlayer.Volume = _bgmVolume;
    }

    public void LoadSound(string name, SoundEffect sound)
    {
        _soundsEffects[name] = sound;
    }
    
    public void PlaySound(string name)
    {
        if (_soundsEffects.TryGetValue(name, out var sound))
        {
            sound.Play(_sfxVolume, 0f, 0f);
        }
    }

    public void LoadSong(string name, Song song)
    {
        _songs[name] = song;
    }

    public void PlaySong(string name, bool isRepeating = true)
    {
        if (_songs.TryGetValue(name, out var song))
        {
            MediaPlayer.Stop();
            MediaPlayer.IsRepeating = isRepeating;
            MediaPlayer.Volume = _bgmVolume; // Apply current BGM volume
            MediaPlayer.Play(song);
        }
    }
}