using UnityEngine;

[CreateAssetMenu(fileName ="New Grid Item", menuName = "Dead Echo/Inventory/New Item")]
public class ItemData : ScriptableObject
{
    public int Width = 1;
    public int Height = 1;

    public Sprite Icon = null;
}