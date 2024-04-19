using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundItemGrid : ItemGrid
{
    //[HideInInspector]
    public InventoryItemConteiner currentConteiner;

    [SerializeField] private GameObject _conteinerPrefab    = null;
    [SerializeField] private GameObject _conteinersContent  = null;
    [SerializeField] private GameObject _playerObject       = null;

    public void SetItems(List<ItemSave> items) //Apenas preenche o inventário de chão com o inventário passado pelo conteiner.
    {
        ResetGrid();
        foreach (var item in items)
        {
            InventoryItem itemView = CreateNewItem(item.Data);

            PlaceItem(itemView, item.Position.Value.x, item.Position.Value.y, false);
        }
    }

    public void ResetGrid()
    {
        Debug.LogWarning("Reseting ground grid!");
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
            Debug.Log("Spawning a new ground item conteiner!");
            GameObject spawnedConteiner = Instantiate(_conteinerPrefab, _playerObject.transform.position, Quaternion.identity, _conteinersContent.transform);
            currentConteiner = spawnedConteiner.GetComponent<InventoryItemConteiner>();
        }

        if (currentConteiner != null && item != null)
        {
            currentConteiner.AddItemInPosition(new ItemSave(item.data, posX, posY));
            base.PlaceItem(item, posX, posY);
        }
    }
}