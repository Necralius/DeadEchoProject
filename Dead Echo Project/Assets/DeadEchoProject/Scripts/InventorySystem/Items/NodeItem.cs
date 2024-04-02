using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New Node Item", menuName = "Dead Echo/Inventory/New Node Item")]
public class NodeItem : ItemData
{
    [Header("Node Settings")]
    public Sprite           NodeSprite;
    public TMP_FontAsset    Font;
    public Color            TextColor;

    [Header("Content")]
    [TextArea] public string Transcription;
}