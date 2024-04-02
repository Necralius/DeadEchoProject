using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ObjectDrag : MonoBehaviour, IDragHandler
{
    private InputManager inptManager = null;
    [SerializeField] private GameObject _draggedObject = null;
    [SerializeField] private ItemData   _selectedItem  = null;

    float deltaRotationX;
    float deltaRotationY;

    private float currentXAngle = 0f;
    private float currentYAngle = 0f;

    [SerializeField, Range(0.1f, 100f)] private float _rotationSpeed = 0.2f;

    private void Start() => inptManager = InputManager.Instance;
    public void SetUp(GameObject go, ItemData item)
    {
        _draggedObject  = go;
        _selectedItem   = item;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggedObject  == null) return;
        if (inptManager     == null) inptManager = InputManager.Instance;

        currentXAngle -= inptManager.Look.x;
        currentYAngle -= inptManager.Look.y;

        currentXAngle = Mathf.Clamp(currentXAngle, _selectedItem.xAngleLimit.x, _selectedItem.xAngleLimit.y);
        currentYAngle = Mathf.Clamp(currentYAngle, _selectedItem.yAngleLimit.x, _selectedItem.yAngleLimit.y);

        _draggedObject.transform.rotation = Quaternion.Euler(currentXAngle, currentYAngle, 0f);

        //deltaRotationX = -inptManager.Look.x;
        //deltaRotationY = -inptManager.Look.y;

        //_draggedObject.transform.rotation =
        //    Quaternion.AngleAxis(deltaRotationX * _rotationSpeed, transform.up) *
        //    Quaternion.AngleAxis(deltaRotationY * _rotationSpeed, transform.right) * _draggedObject.transform.rotation;
    }

    public void RecenterObject()
    {
        if (_draggedObject == null) return;

        _draggedObject.transform.rotation       = Quaternion.Euler(Vector3.zero);
        _draggedObject.transform.localPosition  = Vector3.zero;
    }
}