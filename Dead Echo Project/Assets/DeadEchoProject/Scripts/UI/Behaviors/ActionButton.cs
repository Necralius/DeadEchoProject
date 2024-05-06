using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionButton : Button
{
    private UnityEvent _onSelect       = new UnityEvent();
    private UnityEvent _onDeselect     = new UnityEvent();

    private     TextMeshProUGUI _childText = null;
    [SerializeField] private GameObject _pointer = null;
    public      bool            selected = false;

    protected override void Awake()
    {
        _childText = GetComponentInChildren<TextMeshProUGUI>();
        onClick.AddListener(delegate { UpdateState(); });
        base.Awake();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        _pointer.SetActive(true);
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        _pointer.SetActive(false);
        base.OnPointerExit(eventData);
    }

    public void UpdateState()
    {
        if (selected)
        {
            _onSelect.Invoke();
            _childText.color = colors.selectedColor;
        }
        else
        {
            _onDeselect.Invoke();
            _childText.color = colors.normalColor;
        }
    }
}