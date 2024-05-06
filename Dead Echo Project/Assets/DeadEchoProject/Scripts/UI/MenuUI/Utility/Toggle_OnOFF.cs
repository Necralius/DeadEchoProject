using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Toggle_OnOFF : MonoBehaviour
{
    private Button _btn => GetComponent<Button>();

    [Header("Objects")]
    [SerializeField] private GameObject _active     = null;
    [SerializeField] private GameObject _deactive   = null;

    [Header("Actions")]
    [SerializeField] private UnityEvent _onActive   = null;
    [SerializeField] private UnityEvent _onDeactive = null;

    [Header("State and Settings")]
    [SerializeField] private bool _state            = false;
    [SerializeField] private bool _invertOnClick    = true;

    public bool State
    {
        get => _state;
        set
        {
            _state = value;
            UpdateState();
        }
    }

    private void Start()
    {
        if (_invertOnClick)
            _btn.onClick.AddListener(delegate { InvertState(); });
    }

    public void InvertState()
    {
        _state = !_state;
        UpdateState();
    }

    private void UpdateState()
    {
        if (_state)
        {
            _active.SetActive(true);
            _deactive.SetActive(false);
            _onActive.Invoke();
        }
        else
        {
            _active.SetActive(false);
            _deactive.SetActive(true);
            _onDeactive.Invoke();
        }
    }
}