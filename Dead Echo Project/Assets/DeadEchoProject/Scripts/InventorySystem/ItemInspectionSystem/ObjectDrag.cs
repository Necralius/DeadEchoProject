using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ObjectDrag : MonoBehaviour, IDragHandler
{
    private InputManager inptManager = null;
    [SerializeField] private GameObject _draggedObject = null;

    float deltaRotationX;
    float deltaRotationY;

    [SerializeField, Range(0.1f, 100f)] private float _rotationSpeed = 0.2f;

    private bool _isDragging = false;

    private void Start() => inptManager = InputManager.Instance;
    public void SetUp(GameObject go) => _draggedObject = go;

    public void OnDrag(PointerEventData eventData)
    {
        if (_draggedObject == null) return;

        deltaRotationX = -inptManager.Look.x;
        deltaRotationY = -inptManager.Look.y;

        _draggedObject.transform.rotation =
            Quaternion.AngleAxis(deltaRotationX * _rotationSpeed, transform.up) *
            Quaternion.AngleAxis(deltaRotationY * _rotationSpeed, transform.right) * _draggedObject.transform.rotation;
    }

    public void RecenterObject()
    {
        if (_draggedObject == null) return;

        _draggedObject.transform.rotation       = Quaternion.Euler(Vector3.zero);
        _draggedObject.transform.localPosition  = Vector3.zero;
    }
}