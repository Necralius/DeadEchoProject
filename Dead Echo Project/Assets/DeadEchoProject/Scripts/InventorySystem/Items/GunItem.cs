using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Item", menuName = "Dead Echo/Inventory/New Gun Item")]
public class GunItem : EquipableItem
{
    public override void Equip()
    {
        if (!BodyController.Instance.GunIsEquiped(Name))
            BodyController.Instance.EquipGunByName(this.Name);
        base.Equip();
    }

    public override void Dequip()
    {
        if (BodyController.Instance.GunIsEquiped(Name))
            BodyController.Instance.GunPermanentHolst();
        base.Dequip();
    }
}