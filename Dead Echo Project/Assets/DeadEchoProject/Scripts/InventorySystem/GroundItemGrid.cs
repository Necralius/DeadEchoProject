using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItemGrid : ItemGrid
{
    //[HideInInspector]
    public InventoryItemConteiner currentConteiner;

    [SerializeField] private GameSceneManager conteinerPrefab = null;
    [SerializeField] private GameObject playerObject = null;

    public List<InventoryItem> SetItems(List<ItemData> items)
    {
        List<InventoryItem> _itemsAdded = new List<InventoryItem>();

        Init(sizeWidth, sizeHeight);
        foreach (var item in items)
        {
            InventoryItem itemView = CreateRandomItem(item);

            Vector2Int? posOnGrid = FindSpaceForItem(itemView);

            if (posOnGrid == null)
                continue;

            PlaceItem(itemView, posOnGrid.Value.x, posOnGrid.Value.y);
            _itemsAdded.Add(itemView);
        }
        return _itemsAdded;
    }

    public void ResetGrid()
    {
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

        currentConteiner.RemoveItem(selected.data.GetInstanceID());

        return base.PickUpItem(x, y);
    }

    public override void PlaceItem(InventoryItem item, int posX, int posY)
    {
        if (currentConteiner == null)
        {
            Instantiate(conteinerPrefab, playerObject.transform.position, Quaternion.identity);
        }

        if (currentConteiner != null && item != null) 
            currentConteiner.AddItem(item.data);
        base.PlaceItem(item, posX, posY);
    }
}