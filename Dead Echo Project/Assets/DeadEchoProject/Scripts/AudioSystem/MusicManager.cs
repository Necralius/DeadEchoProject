using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MusicManager
{
    [SerializeField] private List<AudioMusicEvent> musicEventList = new List<AudioMusicEvent>();

    [SerializeField] private MusicEvent currentMusicEvent;

    private float       _timer          = 0f;
    private float       _musicDuration  = 0f;
    private AudioClip   _previousClip   = null;

    [SerializeField] private bool _isPlayingMusic = false;

    [SerializeField] private Vector2 idleTimeRange = new Vector2(10, 40);

    private AudioManager _audioManager;
    public AudioManager AudioManager { set => _audioManager = value; }

    public void SelectMusicEvent(string eventTag)
    {
        if (Enum.TryParse(eventTag, out currentMusicEvent))
        {
            foreach (var musicEvent in musicEventList)
            {
                if (musicEvent.EventTag == currentMusicEvent)
                {
                    PlayMusicEvent(musicEvent);
                    break;
                }
            }
        }    
    }

    public void SelectMusicEvent(MusicEvent eventType)
    {
        currentMusicEvent = eventType;

        foreach (var musicEvent in musicEventList)
        {
            musicEvent.onEvent = false;
            if (musicEvent.EventTag == currentMusicEvent)
            {
                musicEvent.onEvent = true;
                PlayMusicEvent(musicEvent);
            }
        }
    }

    //CHANGE TO COROUTINE (URGENT)
    public void OnUpdate()
    {
        if (_isPlayingMusic)
        {
            if (_timer >= _musicDuration)
            {
                foreach (var musicEvent in musicEventList)
                {
                    if (musicEvent.loop)
                    {
                        _isPlayingMusic = false;
                        _musicDuration  = 0f;
                        _timer          = 0f;
                        return;
                    }

                    musicEvent.onEvent = false;
                    if (musicEvent.EventTag == currentMusicEvent)
                    {
                        musicEvent.onEvent = true;
                        PlayMusicEvent(musicEvent);
                    }
                }
            }
            else _timer += Time.deltaTime;
        }
    }

    private void PlayMusicEvent(AudioMusicEvent musicEvent)
    {
        AudioClip clip = musicEvent.Collection.audioClip;

        if (musicEvent.Collection.bankCount > 1) 
            while (clip == _previousClip) 
                clip = musicEvent.Collection.audioClip;

        _audioManager.PlayOneShotSound(clip, Vector3.zero, musicEvent.Collection);

        _timer              = 0f;
        _previousClip       = clip;
        _musicDuration      = clip.length;
        _isPlayingMusic     = true;
    }

    private void OnMusicEnd()
    {
        //TODO -> Trigger FadeOut
    }
}
public enum MusicEvent { Menu = 0, Environment = 1, Horror = 2, Atmosphere, Effects };
[Serializable]
public class AudioMusicEvent
{
    public AudioCollection  Collection  = null;
    public MusicEvent       EventTag    = MusicEvent.Environment;
    public bool             onEvent     = false;
    public bool             loop        = false;
}