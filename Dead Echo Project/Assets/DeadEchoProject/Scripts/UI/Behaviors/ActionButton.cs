using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : Button, ISelectHandler
{
    private UnityEvent _onSelect       = null;
    private UnityEvent _onDeselect     = null;

    private TextMeshProUGUI _childText = null;

    protected override void Awake()
    {
        _childText = GetComponentInChildren<TextMeshProUGUI>();
        base.Awake();
    }

    public void SetupActions(UnityEvent onSelect, UnityEvent onDeselect)
    {
        _onSelect       = onSelect;
        _onDeselect     = onDeselect;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        _onSelect.Invoke();
        _childText.color = colors.selectedColor;
        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        _onDeselect.Invoke();
        _childText.color = colors.normalColor;
        base.OnDeselect(eventData);
    }
}