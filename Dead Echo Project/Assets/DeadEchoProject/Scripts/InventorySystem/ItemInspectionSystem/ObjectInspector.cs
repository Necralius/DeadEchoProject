using UnityEngine;
using UnityEngine.UI;

public class ObjectInspector : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Transform          _objectArea             = null;
    [SerializeField] private GameObject         _currentSpawnedItem     = null;
    [SerializeField] private TranscriptionView  _transcriptionView      = null;

    [SerializeField] private BodyController     _bodyController         = null;

    private ItemData currentInspectedItem = null;

    public void Inspect(ItemData data)
    {
        GameSceneManager.Instance.OpenInspectionView();
        GameSceneManager.Instance._isInspectingItem = true;

        currentInspectedItem = data;
    }

    public void Transcript()
    {
        _transcriptionView.OpenView(currentInspectedItem as NodeItem);
    }

    public void EndInspection()
    {
        GameSceneManager.Instance._isInspectingItem = false;
    }
}