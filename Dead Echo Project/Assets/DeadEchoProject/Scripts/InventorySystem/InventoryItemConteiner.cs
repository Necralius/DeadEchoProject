using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactor))]
public class InventoryItemConteiner : MonoBehaviour
{
    private Interactor interactor => GetComponent<Interactor>();

    [SerializeField] private List<ItemSave> _items      = new List<ItemSave>();

    private GroundItemGrid _groundGrid;


    private void Start()
    {
        _groundGrid = FindFirstObjectByType(typeof(GroundItemGrid), FindObjectsInactive.Include) as GroundItemGrid;

        interactor.OnEnter. AddListener( delegate { OnAreaEnter();      });
        interactor.OnStart. AddListener( delegate { FillConteiner();    });
        interactor.OnExit.  AddListener( delegate { ResetGroundGrid();  });
    }

    public void FillConteiner()
    {
        if (_groundGrid == null || _items.Count <= 0)
            return;

        _groundGrid.SetItems(_items);
    }

    public void OnAreaEnter()
    {
        if (_groundGrid == null) 
            return; 
        _groundGrid.currentConteiner = this;
    }

    public void ResetGroundGrid()
    {
        if (_groundGrid == null) 
            return;

        _groundGrid.ResetGrid();
        _groundGrid.currentConteiner = null;
    }

    public void RemoveItem(int instanceID)
    {
        _items.Remove(_items.Find(e => e.Data.GetInstanceID() == instanceID));

        if (_items.Count <= 0) 
            Destroy(gameObject);
    }

    public void AddItemInPosition(ItemSave item) => _items.Add(item);
}

[Serializable]
public class ItemSave
{
    public ItemData     Data;
    public Vector2Int?  Position;

    public ItemSave(ItemData data, int posX, int posY)
    {
        Data     = data;
        Position = new Vector2Int(posX, posY);
    }
}