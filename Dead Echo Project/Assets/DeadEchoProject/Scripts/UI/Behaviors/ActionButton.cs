using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AB_Extension))]
public class ActionButton : Button
{
    public AB_Extension _extension     => GetComponent<AB_Extension>();

    private UnityEvent _onSelect       = new UnityEvent();
    private UnityEvent _onDeselect     = new UnityEvent();

    protected override void Awake()
    {
        _extension.childText = GetComponentInChildren<TextMeshProUGUI>();
        onClick.AddListener(delegate { UpdateState(); });
        base.Awake();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        _extension.pointer.SetActive(true);
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (_extension.selected) 
            return;
        _extension.pointer.SetActive(false);
        base.OnPointerExit(eventData);
    }

    public void UpdateState()
    {
        if (_extension.selected)
        {
            _onSelect.Invoke();
            _extension.pointer.SetActive(true);
            _extension.childText.color = colors.selectedColor;
        }
        else
        {
            _onDeselect.Invoke();
            _extension.pointer.SetActive(false);
            _extension.childText.color = colors.normalColor;
        }
    }
}