using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemGrid))]
public class GridIteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController _invController;
    ItemGrid _grid;

    private void Awake()
    {
        _invController  = FindFirstObjectByType(typeof(InventoryController)) as InventoryController;
        _grid           = GetComponent<ItemGrid>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _invController.SelectedItemGrid = _grid;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _invController.SelectedItemGrid = null;
    }
}