using UnityEngine;

public class ConsumableItem : ItemData
{
    public virtual void Use()
    {
        Debug.Log("Using consumable!");
        InventoryController.Instance.UseSelectedConsumableItem();
    }
}
