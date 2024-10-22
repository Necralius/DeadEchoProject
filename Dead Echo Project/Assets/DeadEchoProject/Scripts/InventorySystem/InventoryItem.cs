using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData data;

    public int HEIGHT { get => !rotated ? data.Height : data.Width; }
    public int WIDTH { get => !rotated ? data.Width : data.Height; }

    public int onGridPosX;
    public int onGridPosY;

    public bool rotated = false;

    public ItemGrid originGrid = null;

    internal void Rotate()
    {
        rotated = !rotated;

        RectTransform rect  = GetComponent<RectTransform>();
        rect.rotation       = Quaternion.Euler(0,0, rotated ? 90f : 0f);
    }

    internal void Set(ItemData itemData, ItemGrid originGrid)
    {
        this.data       = itemData;
        this.originGrid = originGrid;

        GetComponent<Image>().sprite = data.Icon;

        Vector2 size = new Vector2();
        size.x = data.Width     * ItemGrid.tileSizeWidth;
        size.y = data.Height    * ItemGrid.tileSizeHeight;

        GetComponent<RectTransform>().sizeDelta     = size;
        GetComponent<RectTransform>().localScale    = Vector3.one;
    }

    public void UpdateUISize()
    {
        GetComponent<RectTransform>().localScale = Vector3.one;
    }
}