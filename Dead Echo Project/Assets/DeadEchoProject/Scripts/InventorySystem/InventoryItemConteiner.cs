using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactor))]
public class InventoryItemConteiner : MonoBehaviour
{
    private Interactor interactor => GetComponent<Interactor>();

    [SerializeField] private List<ItemSave> _itemsSaves      = new List<ItemSave>();

    private GroundItemGrid _groundGrid;

    public bool onPlayerArea = false;

    private void Start()
    {
        _groundGrid = FindFirstObjectByType(typeof(GroundItemGrid), FindObjectsInactive.Include) as GroundItemGrid;

        interactor.OnEnter. AddListener( delegate { OnAreaEnter();  });
        interactor.OnExit.  AddListener( delegate { OnAreaExit();   });
    }

    private void OnAreaEnter()
    {
        if (_groundGrid == null) 
            return; 
        _groundGrid.currentConteiner = this;

        if (_itemsSaves.Count > 0)
            _groundGrid.SetItems(ref _itemsSaves);

        onPlayerArea = true;
    }

    private void OnAreaExit()
    {
        Debug.Log("Exiting Area!");
        onPlayerArea = false;
        ResetGroundGrid();
    }

    private void ResetGroundGrid()
    {
        if (_groundGrid == null) 
            return;

        _groundGrid.ResetGrid();
        _groundGrid.currentConteiner = null;
    }

    public void RemoveItem(InventoryItem itemToRemove)
    {
        Vector2Int pos          = new Vector2Int(itemToRemove.onGridPosX, itemToRemove.onGridPosY);
        ItemSave   findedItem   = _itemsSaves.Find(e => e.Position.Value.x == pos.x && e.Position.Value.y == pos.y);

        _itemsSaves.Remove(findedItem);

        if (_itemsSaves.Count <= 0) 
            Destroy(gameObject);
    }

    public void AddItemInPosition(ItemSave item) => _itemsSaves.Add(item);
}

[Serializable]
public class ItemSave
{
    public ItemData     Data;
    public Vector2Int?  Position;
    public Vector2INT   PosData;

    public Vector2 IntPosition { get => new Vector2Int(PosData.x, PosData.y); }

    public Guid identifier = Guid.NewGuid();

    public ItemSave(ItemData data, int posX, int posY)
    {
        Data        = data;
        Position    = new Vector2Int(posX, posY);
        identifier  = Guid.NewGuid();
    }
}

[Serializable]
public struct Vector2INT
{
    public int x, y;
}