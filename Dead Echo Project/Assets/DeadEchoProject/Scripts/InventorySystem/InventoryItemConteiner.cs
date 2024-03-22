using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemConteiner : MonoBehaviour
{
    [SerializeField] private List<ItemData> _items      = new List<ItemData>();

    [SerializeField] private GroundItemGrid _groundGrid = null;

    public void FillConteiner()
    {
        _groundGrid?.SetItems(_items);
        _groundGrid.currentConteiner = this;
    }

    public void ResetGroundGrid()
    {
        _groundGrid?.ResetGrid();
        _groundGrid.currentConteiner = null;
    }

    public void RemoveItem(int instanceID)  => _items.Remove(_items.Find(e => e.GetInstanceID() == instanceID));
    public void AddItem(ItemData item)      => _items.Add(item);


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player")) 
            FillConteiner();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player")) 
            ResetGroundGrid();
    }
}