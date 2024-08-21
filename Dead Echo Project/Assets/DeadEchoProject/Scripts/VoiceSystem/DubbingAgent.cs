using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static NekraByte.Core.Enumerators;
using Random = UnityEngine.Random;

public class DubbingAgent : MonoBehaviour
{
    [SerializeField] private List<VoiceAsset> _dubbingAssets;

    private VoiceAsset GetAsset(string tag)      => _dubbingAssets.FirstOrDefault(e => e.Tag == tag);
    private VoiceAsset GetAsset(DubType dubType) => _dubbingAssets.FirstOrDefault(e => e.Type == dubType);

    /// <summary>
    /// Triggers a dubbing asset based in a dubbing asset tag.
    /// </summary>
    /// <param name="tag"></param>
    public void TriggerDub(string tag)
    {
        VoiceAsset asset = GetAsset(tag);

        if (asset != null) 
            AudioManager.Instance.PlayOneShotSound(asset.collection.audioClip, transform.position, asset.collection);
    }

    /// <summary>
    /// Triggers a dubbing asset based on a dubbing asset type.
    /// </summary>
    /// <param name="dubType"> The dubbing type to be triggered. </param>
    public void TriggerDub(DubType dubType)
    {
        VoiceAsset asset = GetAsset(dubType);

        if (asset != null)
            AudioManager.Instance.PlayOneShotSound(asset.collection.audioClip, transform.position, asset.collection);
    }
}

[Serializable]
public class VoiceAsset
{
    public string           Tag   = string.Empty;
    public AudioCollection  collection;
    public DubType          Type  = DubType.Event;
}