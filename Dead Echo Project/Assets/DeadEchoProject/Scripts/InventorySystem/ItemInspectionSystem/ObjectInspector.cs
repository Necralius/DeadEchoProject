using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ObjectInspector : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Transform  _objectArea             = null;
    [SerializeField] private GameObject _currentInspectedItem   = null;

    //[SerializeField] private GameObject _inspectionScreen       = null;

    //[Header("Transcription")]
    //[SerializeField] private ObjectDrag         _objectDragger       = null;
    //[SerializeField] private TranscriptionView  _transcriptionView   = null;
    //[SerializeField] private Button             _transcriptionButton = null;

    [SerializeField] private BodyController _bodyController = null;

    public void Inspect(ItemData data)
    {
        GameSceneManager.Instance.OpenInspectionView();
        //GameSceneManager.Instance.ChangeInventoryState(false);

        //_inspectionScreen.SetActive(true);

        //if (data is NodeItem)
        //{
        //    _transcriptionButton.gameObject.SetActive(data is NodeItem);
        //    _transcriptionButton.onClick.AddListener(delegate { _transcriptionView.OpenView((NodeItem)data); });
        //}

        if (_currentInspectedItem != null)
            Destroy(_currentInspectedItem);

        _currentInspectedItem = Instantiate(data.Prefab, _objectArea);
    }
}