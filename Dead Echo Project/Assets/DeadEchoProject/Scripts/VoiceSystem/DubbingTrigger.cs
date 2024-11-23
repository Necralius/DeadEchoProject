using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DubbingTrigger : MonoBehaviour
{
    private BoxCollider _col = null;
    [SerializeField] private VoiceAsset _dubbingAsset = null;
    [SerializeField] private bool _consumed      = false;
    [SerializeField] private bool _consumeInLoop = false;

    private void Start()
    {
        _col = GetComponent<BoxCollider>();
    }

    public void TriggerByMethod()
    {
        ConsumeDubbingAsset();
    }

    private void ConsumeDubbingAsset()
    {
        if (!_consumed || _consumeInLoop)
            DubbingManager.Instance.TriggerDub(_dubbingAsset, transform.position);
        _consumed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            ConsumeDubbingAsset();
    }
}