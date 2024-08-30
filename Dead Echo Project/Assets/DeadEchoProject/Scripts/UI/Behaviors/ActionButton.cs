using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AB_Extension))]
public class ActionButton : Button
{
    public AB_Extension _extension     => GetComponent<AB_Extension>();

    protected override void Awake()
    {
        _extension.childText = GetComponentInChildren<TextMeshProUGUI>();
        onClick.AddListener(delegate { UpdateState(); });
        base.Awake();
    }
    public override void OnSelect(BaseEventData eventData)
    {
        _extension.selected = true;
        UpdateState();
        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        _extension.selected = false;
        UpdateState();
        base.OnDeselect(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        _extension.pointer.SetActive(true);
        UpdateState();
        base.OnPointerDown(eventData);
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
            _extension.childText.color = colors.selectedColor;
            _extension.pointer.SetActive(true);
            _extension.OnSelect.Invoke();
        }
        else
        {
            _extension.childText.color = colors.normalColor;
            _extension.pointer.SetActive(false);
            _extension.OnDeselect.Invoke();
        }
    }
}