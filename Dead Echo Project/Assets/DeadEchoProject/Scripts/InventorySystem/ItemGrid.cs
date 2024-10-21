using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(GridIteract))]
public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth   = 100;
    public const float tileSizeHeight  = 100;

    protected InventoryItem[,] _inventoryItemSlot;

    protected RectTransform rect => GetComponent<RectTransform>();

    [SerializeField] protected int sizeWidth  = 8;
    [SerializeField] protected int sizeHeight = 9;


    private void Start()
    {
        Init(sizeWidth, sizeHeight);
    }

    protected void Init(int width, int height)
    {
        _inventoryItemSlot = new InventoryItem[width, height];

        Vector2 size    = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rect.sizeDelta  = size;
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        Vector2 localMousePosition      = new Vector2(mousePosition.x, Screen.height - mousePosition.y);
        Vector2 localPositionOnGrid     = localMousePosition - new Vector2(rect.position.x, Screen.height - rect.position.y);

        float normalizedWidth       = Screen.width / 1920f;
        float normalizedHeight      = Screen.height / 1080f;

        float tileSizeWidthNormalized   = tileSizeWidth * normalizedWidth;
        float tileSizeHeightNormalized  = tileSizeHeight * normalizedHeight;

        Vector2Int tileGridPosition     = new Vector2Int((int)(localPositionOnGrid.x / tileSizeWidthNormalized), (int)(localPositionOnGrid.y / tileSizeHeightNormalized));

        return tileGridPosition;
    }

    public virtual InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem selected = _inventoryItemSlot[x, y];

        if (selected == null) 
            return null;

        CleanItem(selected);

        return selected;
    }

    private void CleanItem(InventoryItem selected)
    {
        for (int ix = 0; ix < selected.WIDTH; ix++)
            for (int iy = 0; iy < selected.HEIGHT; iy++)
                _inventoryItemSlot[selected.onGridPosX + ix, selected.onGridPosY + iy] = null;
    }

    public bool PlaceItem(InventoryItem item, int posX, int posY, ref InventoryItem overlapItem)
    {
        if (!BoundryCheck(posX, posY, item))
            return false;

        if (!OverlapItemCheck(posX, posY, item.WIDTH, item.HEIGHT, ref overlapItem))
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
            CleanItem(overlapItem);

        PlaceItem(item, posX, posY);

        return true;
    }

    public virtual void PlaceItem(InventoryItem item, int posX, int posY)
    {
        RectTransform rectTrans = item.GetComponent<RectTransform>();
        rectTrans.SetParent(this.rect);

        for (int x = 0; x < item.WIDTH; x++)
            for (int y = 0; y < item.HEIGHT; y++)
                _inventoryItemSlot[posX + x, posY + y] = item;

        item.onGridPosX = posX;
        item.onGridPosY = posY;

        Vector2 position = GetPosOnGrid(item, posX, posY);

        rectTrans.localPosition = position;
    }

    public virtual void PlaceItem(InventoryItem item, int posX, int posY, bool ready)
    {
        RectTransform rectTrans = item.GetComponent<RectTransform>();
        rectTrans.SetParent(this.rect);

        for (int x = 0; x < item.WIDTH; x++)
            for (int y = 0; y < item.HEIGHT; y++)
                _inventoryItemSlot[posX + x, posY + y] = item;

        item.onGridPosX = posX;
        item.onGridPosY = posY;

        Vector2 position = GetPosOnGrid(item, posX, posY);

        rectTrans.localPosition = position;


    }


    public Vector2 GetPosOnGrid(InventoryItem item, int posX, int posY)
    {
        Vector2 position = new Vector2();

        position.x = posX * tileSizeWidth + tileSizeWidth * item.WIDTH / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * item.HEIGHT / 2);
        return position;
    }

    private bool OverlapItemCheck(int posX, int poxY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (_inventoryItemSlot[posX + x, poxY + y] != null)
                {
                    if (overlapItem == null) overlapItem = _inventoryItemSlot[posX + x, poxY + y];
                    else
                    {
                        if (overlapItem != _inventoryItemSlot[posX + x, poxY + y]) return false;
                    }
                }
            }
        }
        return true;
    }

    private bool ValidPosition(int posX, int posY)
    {
        if (posX < 0 || posY < 0) 
            return false;
        if (posX >= sizeWidth || posY >= sizeHeight) 
            return false;

        return true;
    }

    public bool BoundryCheck(int posX, int posY, InventoryItem item)
    {
        if (!ValidPosition(posX, posY)) return false;

        posX += item.WIDTH - 1;
        posY += item.HEIGHT - 1;

        if (!ValidPosition(posX, posY)) return false;

        return true;
    }

    public InventoryItem GetItem(int x, int y)
    {
        if (x < 0 || y < 0 || x > sizeWidth - 1 || y > sizeHeight - 1) 
            return null;
        return _inventoryItemSlot[x, y];
    }

    private bool FindAvaliableSpace(int posX, int poxY, InventoryItem itemToCheck)
    {
        for (int x = 0; x < itemToCheck.WIDTH; x++) 
            for (int y = 0; y < itemToCheck.HEIGHT; y++) 
                if (_inventoryItemSlot[posX + x, poxY + y] != null) return false;
        return true;
    }

    public Vector2Int? FindSpaceForItem(InventoryItem itemToInsert)
    {
        int height  = sizeHeight - itemToInsert.HEIGHT + 1;
        int width   = sizeHeight - itemToInsert.WIDTH  + 1;

        for (int y = 0; y < height; y++) 
            for (int x = 0; x < width; x++) 
                if (FindAvaliableSpace(x, y, itemToInsert)) return new Vector2Int(x, y);
        return null;
    }

    protected InventoryItem CreateNewItem(ItemData item)
    {
        InventoryItem inventoryItem = Instantiate(InventoryController.Instance.itemPrefab).GetComponent<InventoryItem>();
        inventoryItem.gameObject.transform.localScale = Vector3.one;
        RectTransform itemRect = inventoryItem.GetComponent<RectTransform>();

        itemRect.SetParent(InventoryController.Instance.canvasTrans);
        itemRect.SetAsLastSibling();

        inventoryItem.Set(item);
        return inventoryItem;
    }
}