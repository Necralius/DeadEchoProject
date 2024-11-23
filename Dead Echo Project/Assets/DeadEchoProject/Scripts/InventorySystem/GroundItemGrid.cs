using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItemGrid : ItemGrid
{
    public InventoryItemConteiner currentConteiner;

    [SerializeField] private GameObject _conteinerPrefab    = null;
    [SerializeField] private GameObject _conteinersContent  = null;
    [SerializeField] private GameObject _playerObject       = null;

    private bool _gridSynced = false;

    public void SetItems(ref List<ItemSave> items, bool overrideFromSrach = false)
    {
        if (_gridSynced)
            return;
        ResetGrid();
        foreach (var item in items)
        {
            InventoryItem itemView = CreateNewItem(item.Data);

            if (item.Position == null)
            {
                Vector2Int? pos = FindSpaceForItem(itemView);
                if (pos == null)
                    return;
                item.Position = pos;
                base.PlaceItem(itemView, pos.Value.x, pos.Value.y); 
            }
            else
                base.PlaceItem(itemView, item.Position.Value.x, item.Position.Value.y);
        }
        _gridSynced = true;
    }

    public void OverrideItems(List<ItemSave> items)
    {
        foreach (var item in items)
        {
            InventoryItem itemView = CreateNewItem(item.Data);


            base.PlaceItem(itemView, item.Position.Value.x, item.Position.Value.y);
        }



    }

    public void ResetGrid()
    {
        _gridSynced = false;
        Init(sizeWidth, sizeHeight);
        foreach (Transform trans in transform)
        {
            if (trans.CompareTag("Highlighter"))
                continue;
            else
                Destroy(trans.gameObject);
        }
    }

    public override InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem selected = _inventoryItemSlot[x, y];

        if (selected == null)
            return null;

        currentConteiner.RemoveItem(selected);

        return base.PickUpItem(x, y);
    }

    public override void PlaceItem(InventoryItem item, int posX, int posY)
    {
        base.PlaceItem(item, posX, posY);

        if (currentConteiner == null)
        {
            GameObject spawnedConteiner = Instantiate(
                _conteinerPrefab,
                _playerObject.transform.position,
                Quaternion.identity,
                _conteinersContent.transform);

            currentConteiner = spawnedConteiner.GetComponent<InventoryItemConteiner>();
        }

        if (currentConteiner != null && item != null)
            currentConteiner.AddItemInPosition(new ItemSave(item.data, posX, posY));
    }
}