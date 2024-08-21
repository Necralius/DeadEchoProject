using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class EventTrigger : MonoBehaviour
{
    private BoxCollider _col => GetComponent<BoxCollider>();

    [SerializeField] private bool   onTagBased      = true;
    [SerializeField] private string tagToInteract   = string.Empty;

    [SerializeField] private UnityEvent _onEnter    = null;
    [SerializeField] private UnityEvent _onExit     = null;

    private void Start() => _col.isTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        if (onTagBased)
        {
            if (other.CompareTag(tagToInteract)) 
                _onEnter.Invoke();
            return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (onTagBased)
        {
            if (other.CompareTag(tagToInteract))
                _onExit.Invoke();
            return;
        }
    }
}