using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Interaction", menuName = "Dead Echo/Interaction/New Dialogue Interaction")]
public class DialogueInteraction : InteractionModel
{
    [TextArea] public List<string> _phrases = new List<string>();
}