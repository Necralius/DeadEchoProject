using UnityEngine;

[RequireComponent(typeof(Interactor), typeof(Animator))]
public class Door : MonoBehaviour
{
    private Animator   _anim        = null;
    private Interactor _interactor  = null;

    [SerializeField] private bool _open = false;

    [SerializeField] private AudioCollection _collection = null;

    private void Start()
    {
        _anim       = GetComponent<Animator>();
        _interactor = GetComponent<Interactor>();

        _interactor.OnStart.AddListener(() => OnInteract());

        OnInteract(false);
    }

    public void OnInteract(bool changeState = true)
    {
        if (changeState)
            _open = !_open;
        _anim.SetBool("IsOpen", _open);
    }

    public void DoorEffect()
    {
        AudioClip clip = _open ? _collection[0, 0] : _collection[0, 1];

        AudioManager.Instance?.PlayOneShotSound(clip, transform.position, _collection);
    }
}