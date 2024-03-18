using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ActionButton))]
public class ButtonInteractor : MonoBehaviour
{
    ActionButton _button => GetComponent<ActionButton>();

    [SerializeField] private UnityEvent _onSelect = null;
    [SerializeField] private UnityEvent _onDeselect = null;

    private void Start() => _button.SetupActions(_onSelect, _onDeselect);
}