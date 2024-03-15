using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Letter Interaction", menuName = "Dead Echo/Interaction/New Letter Interaction")]
public class LetterInteraction : InteractionModel
{
    public Sprite LetterTexture;
    [TextArea] public string letterContent;

}