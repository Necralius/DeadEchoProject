using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectInspector : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Transform  _objectArea             = null;
    [SerializeField] private GameObject _inspectionScreen       = null;
    [SerializeField] private GameObject _currentInspectedItem   = null;

    [Header("Transcription")]
    [SerializeField] private ObjectDrag         _objectDragger       = null;
    [SerializeField] private TranscriptionView  _transcriptionView   = null;
    [SerializeField] private Button             _transcriptionButton = null;

    public void Inspect(ItemData data)
    {
        _inspectionScreen.SetActive(true);

        if (data is NodeItem)
        {
            _transcriptionButton.gameObject.SetActive(data is NodeItem);
            _transcriptionButton.onClick.AddListener(delegate { _transcriptionView.OpenView((NodeItem)data); });
        }

        if (_currentInspectedItem != null)
            Destroy(_currentInspectedItem);

        _currentInspectedItem = Instantiate(data.Prefab, _objectArea);

        _objectDragger.SetUp(_currentInspectedItem, data);
        _objectDragger.RecenterObject();
    }
}