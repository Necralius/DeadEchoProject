using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDubbingManager : MonoBehaviour
{
    public List<VoiceAsset> _voiceAssets = new List<VoiceAsset>();

    public void TriggerVoiceAsset(string assetTag)
    {
        VoiceAsset asset = _voiceAssets.Find(e => e.Tag == assetTag);

        if (asset != null)
            DubbingManager.Instance.TriggerDub(asset, transform.position);
    }
}