using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderToText : MonoBehaviour
{
    private Slider _sld => GetComponent<Slider>();

    [SerializeField] private TextMeshProUGUI _txt = null;

    [SerializeField] private bool _updateOnModify = true;

    private void Start()
    {
        if (_updateOnModify)
            _sld.onValueChanged.AddListener(delegate { _txt.text = _sld.value.ToString("F1"); });
    }
}