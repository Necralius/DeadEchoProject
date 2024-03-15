using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    #region - Singleton Pattern -
    public static TooltipManager Instance;
    private void Awake()
    {
        if (Instance != null) Destroy(Instance.gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
        HideTooltip();
    }
    #endregion

    private TextMeshProUGUI             _contentText            => GetComponentInChildren<TextMeshProUGUI>();
    private RectTransform               _backgroundTransform    => GetComponentInChildren<RectTransform>();
    private Canvas                      _canvas                 => GetComponentInParent<Canvas>();
    [SerializeField] private GameObject _tootipObject;

    [SerializeField] private Vector2    _padding                = new Vector2(60, 30);

    private void Update()
    {
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, Input.mousePosition, _canvas.worldCamera, out mousePos);
        mousePos += _padding;

        transform.position = _canvas.transform.TransformPoint(mousePos);
    }

    public void ShowTooltip(string tooltipContent)
    {
        _contentText.text               = tooltipContent;
        float   textPaddingSize         = 4f;
        Vector2 backgroundSize          = new Vector2(_contentText.preferredWidth + textPaddingSize * 2f, 
                                                      _contentText.preferredHeight + textPaddingSize * 2f);

        _backgroundTransform.sizeDelta  = backgroundSize;
        gameObject.SetActive(true);
    }
    public void HideTooltip() => gameObject.SetActive(false);
}