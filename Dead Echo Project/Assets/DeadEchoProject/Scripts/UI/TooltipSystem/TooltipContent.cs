using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

[RequireComponent(typeof(Image))]
public class TooltipContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, TextArea]       private string _tooltipContent = "";
    [SerializeField, Range(0f, 10f)] private float  _timeToTooltip = 1f;
    private float _timer = 0f;
    [SerializeField] private string _localizationKey = "";

    [SerializeField] private bool onTooltipArea = false;

    public string GetContent() => _tooltipContent;

    private void Update()
    {
        if (onTooltipArea)
        {
            if (_timer >= _timeToTooltip)
            {
                TooltipManager.Instance.ShowTooltip(_tooltipContent, _localizationKey);
                _timer = _timeToTooltip;
            }
            else _timer += Time.deltaTime;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _timer = 0f;
        onTooltipArea = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onTooltipArea = false;
        TooltipManager.Instance.HideTooltip();
    }
}