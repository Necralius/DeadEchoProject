using UnityEngine;
using static NekraByte.Core.Enumerators;

[CreateAssetMenu(fileName = "New Interaction Model", menuName = "Dead Echo/Interaction/New Basic Intreaction")]
public class InteractionModel : ScriptableObject
{
    [SerializeField] private string Label       = "interact with";
    public InteractionButton Button;

    [SerializeField] private string ButtonTag   = string.Empty;

    public string GetLabel()
    {
        if (Label == "" || Label == string.Empty)
            return "";
        string completeLabel = $"Press {Button} to {Label}";
        return completeLabel;
    }
}