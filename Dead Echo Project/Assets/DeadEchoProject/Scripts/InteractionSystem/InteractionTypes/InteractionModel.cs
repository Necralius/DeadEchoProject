using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Interaction Model", menuName = "Dead Echo/Interaction/New Basic Intreaction")]
public class InteractionModel : ScriptableObject
{
    [SerializeField] private string Label   = "interact with";
    [SerializeField] private string Key     = "E";


    public string GetLabel()
    {
        string completeLabel = $"Press {Key} to {Label}";
        return completeLabel;
    }
}