using UnityEngine;

[CreateAssetMenu(fileName = "New Food Item", menuName = "Dead Echo/Inventory/New Food Item")]
public class FoodItem : ConsumableItem
{
    [Range(0f, 100f)] public float foodToGain     = 0f;
    [Range(0f, 100f)] public float thrityToGain   = 0f;

    public override void Use()
    {
        CharacterManager.Instance.Hunger += foodToGain;
        CharacterManager.Instance.Thirst += thrityToGain;
        base.Use();
    }
}
