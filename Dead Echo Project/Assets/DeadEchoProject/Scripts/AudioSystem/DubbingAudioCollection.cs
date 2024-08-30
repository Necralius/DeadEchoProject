using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DubbingAudioCollection : AudioCollection
{
    private Queue<AudioClip> _dubbingClipsQueue = new Queue<AudioClip>();

    [Header("Clips")]
    [Tooltip("This list represents the dubbing clips in speech order")]
    [SerializeField] private List<AudioClip> _dubbingClips = new List<AudioClip>();

    public bool Executed { get => _dubbingClipsQueue.Count <= 0; }

    public void EnableAsset() => _dubbingClips.ForEach(e => _dubbingClipsQueue.Enqueue(e));

    public AudioClip GetAsset()
    {
        if (_dubbingClipsQueue is null && _dubbingClipsQueue.Count <= 0) 
            return null;

        return _dubbingClipsQueue.Dequeue();
    }
}