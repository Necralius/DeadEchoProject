using UnityEngine;

[CreateAssetMenu(fileName = "New Interaction Model", menuName = "Dead Echo/Interaction/New Basic Intreaction")]
public class InteractionModel : ScriptableObject
{
    [SerializeField] private string Label       = "interact with";
    [SerializeField] private string Key         = "E";

    [SerializeField] private string ButtonTag   = string.Empty;

    public string GetLabel()
    {
        if (Label == "" || Label == string.Empty || Key == "" || Key == string.Empty)
            return "";
        string completeLabel = $"Press {Key} to {Label}";
        return completeLabel;
    }
}