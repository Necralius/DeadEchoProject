using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectInspector : MonoBehaviour
{
    [SerializeField] private Transform  _objectArea             = null;
    [SerializeField] private GameObject _inspectionScreen       = null;
    [SerializeField] private GameObject _currentInspectedItem   = null;

    [SerializeField] private ObjectDrag _objectDragger = null;

    [SerializeField] private Button _transcriptNode = null;

    public void Inspect(ItemData data)
    {
        _inspectionScreen.SetActive(true);

        _transcriptNode.gameObject.SetActive(data is NodeItem);

        if (_currentInspectedItem != null)
            Destroy(_currentInspectedItem);

        _currentInspectedItem = Instantiate(data.Prefab, _objectArea);

        _objectDragger.SetUp(_currentInspectedItem);
        _objectDragger.RecenterObject();
    }
}