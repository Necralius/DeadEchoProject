using System;
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
    public NodeContent Content;
}

[Serializable]
public class NodeContent
{
    public CostumTextContent Title         = new CostumTextContent();
    public CostumTextContent Subtitle      = new CostumTextContent();

    public CostumTextContent Content       = new CostumTextContent();
    public CostumTextContent Signature     = new CostumTextContent();

    public CostumTextContent Footer        = new CostumTextContent();
}

[Serializable]
public class CostumTextContent
{
    [TextArea]
    public string           Content = string.Empty;
    public Color            Color   = Color.white;
    public TMP_FontAsset    Font    = null;

    public void ChangeText(ref TextMeshProUGUI text)
    {
        text.color  = Color;
        text.font   = Font;
        text.text   = Content;
    }
}