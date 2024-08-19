using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NekraByte.Core.DataTypes;
using Assets.SimpleLocalization.Scripts;

[RequireComponent(typeof(Slider))]
public class SliderFloatField : MonoBehaviour
{
    Slider                  _sld        => GetComponent<Slider>();
    public string           _fieldName  = string.Empty;
    public TextMeshProUGUI  _label      = null;
    public TextMeshProUGUI  _valueLabel = null;

    public float Value { get => _sld.value; }

    private void OnEnable()
    {
        _sld.onValueChanged.AddListener     (delegate { OnSliderChange(); });

        OnSliderChange();
    }

    public void SetUp(AudioTrackVolume trackData)
    {
        _label.text = trackData.Name;
        _fieldName  = trackData.Name;

        _label.GetComponent<LocalizedText>().LocalizationKey = "Settings." + trackData.Name;
        OverrideValue(trackData.Volume);

        _valueLabel.text = _sld.value.ToString("F1");
    }

    private void OnSliderChange()
    {
        if (_label == null || _sld == null) 
            return;

        _valueLabel.text = _sld.value.ToString("F1");
    }

    public void OverrideValue(float value)
    {
        if (_sld == null || _label == null) 
            return;

        _sld.value          = value;
        _valueLabel.text    = value.ToString("F1");
    }
}