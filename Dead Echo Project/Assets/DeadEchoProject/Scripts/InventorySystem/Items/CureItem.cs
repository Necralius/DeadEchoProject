using UnityEngine;

[CreateAssetMenu(fileName = "New Cure Item", menuName = "Dead Echo/Inventory/New Cure Item")]
public class CureItem : ConsumableItem
{
    [Range(0f, 100f)]
    public float healthToCure = 75f;

    public override void Use()
    {
        BodyController.Instance?.UseSyringe(this);
    }
}