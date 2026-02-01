using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace SlimeTogetherStrong.Engine.Managers;

public class AudioManager
{
    public static AudioManager Instance { get; private set; } = new AudioManager();
    
    private float _sfxVolume = 0.7f;
    
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
    
    private AudioManager() 
    {
        // Initialize MediaPlayer volume
        MediaPlayer.Volume = _sfxVolume;
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
}