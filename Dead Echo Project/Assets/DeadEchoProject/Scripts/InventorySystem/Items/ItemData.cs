using UnityEngine;

[CreateAssetMenu(fileName ="New Base Item", menuName = "Dead Echo/Inventory/New Base Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Data")]
    public string Name;
    [TextArea] public string Description;

    public Sprite       Icon    = null;
    public GameObject   Prefab  = null;

    [Header("Item Settings")]
    public int Width    = 1;
    public int Height   = 1;
}