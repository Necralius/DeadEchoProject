using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DropsideOption : MonoBehaviour
{
    public SimpleScrollSnap _snapController = null;

    [SerializeField] private int _value;

    [SerializeField] private List<string> options;
    [SerializeField] private GameObject optionPrefab;

    public int Value
    {
        get => _value = _snapController.CenteredPanel;
        set
        {
            _value = value;
            _snapController.GoToPanel(value);
        }
    }

    public string ContentValue { get => options[Value]; }

    public void AddCostumOptions(List<string> panels)
    {
        if (_snapController == null) return;
        options = panels;

        for (int i = 0; i < options.Count; i++)
        {
            GameObject option = Instantiate(optionPrefab, _snapController.Content.transform);
            option.GetComponent<TextMeshProUGUI>().text = panels[i];

            _snapController.Add(option, i);
        }
    }

    public void ClearOptions()
    {
        if (_snapController == null) 
            return;
        if (_snapController.NumberOfPanels == 0) 
            return;

        for (int i = 0; i < _snapController.NumberOfPanels; i++) 
            _snapController.Remove(i);
        options.Clear();
    }
}