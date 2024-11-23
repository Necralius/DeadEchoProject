using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MusicEventTrigger : MonoBehaviour
{
    private BoxCollider _col = null;

    [SerializeField] private AudioMusicEvent _musicEvent = null;

    [SerializeField] private bool playOnStart = false;

    private void Start()
    {
        _col = GetComponent<BoxCollider>();
        if (playOnStart)
            TriggerMusic();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playOnStart)
            TriggerMusic();
    }

    public void TriggerMusic()
    {
        AudioManager.Instance.musicManager.SelectMusicEvent(_musicEvent.EventTag);
    }
}