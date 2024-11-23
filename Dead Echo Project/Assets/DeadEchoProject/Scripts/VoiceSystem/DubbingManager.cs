using System;
using System.Collections.Generic;
using UnityEngine;
using static NekraByte.Core.Enumerators;
using System.Linq;

public class DubbingManager : MonoBehaviour
{
    #region - Sigleton Pattern -
    public static DubbingManager Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;
    }
    #endregion

    public void TriggerDub(VoiceAsset asset, Vector3 position = new Vector3())
    {
        if (asset == null)
        {
            Debug.LogWarning("Null voice asset.");
            return;
        }

        AudioClip clip = asset.Collection[asset.Track, asset.Index];

        if (clip == null)
            return;

        if (asset != null)
            AudioManager.Instance.PlayOneShotSound(clip, position, asset.Collection);
    }
}

[Serializable]
public class VoiceAsset
{
    public string           Tag         = null;
    public DubType          Type        = DubType.Event;
    public int              Index       = 0;
    public int              Track       = 0;
    public AudioCollection  Collection  = null;
}