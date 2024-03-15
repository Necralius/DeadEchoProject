using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InventoryHighlight : MonoBehaviour
{
    [SerializeField] private RectTransform highlighter;

    public void SetState(bool state) => highlighter.gameObject.SetActive(state);

    public void SetSize(InventoryItem item)
    {
        Vector2 size = new Vector2();

        size.x = item.WIDTH     * ItemGrid.tileSizeWidth;
        size.y = item.HEIGHT    * ItemGrid.tileSizeHeight;

        highlighter.sizeDelta = size;
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem item)
    {
        Vector2 pos = targetGrid.GetPosOnGrid(item, item.onGridPosX, item.onGridPosY);
        highlighter.localPosition = pos;
    }

    public void SetParent(ItemGrid targetGrid)
    {
        if (targetGrid == null)
            return;
        highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem item, int posX, int posY)
    {
        Vector2 pos = targetGrid.GetPosOnGrid(item, posX, posY);

        highlighter.localPosition = pos;
    }

}