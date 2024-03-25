using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Node Item", menuName = "Dead Echo/Inventory/New Node Item")]
public class NodeItem : ItemData
{
    public Sprite NodeSprite;

    [TextArea] public string Transcription;
}