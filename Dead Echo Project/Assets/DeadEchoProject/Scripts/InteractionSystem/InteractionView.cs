using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionView : MonoBehaviour
{
    [SerializeField] private GameObject         _interactionObject  = null;
    [SerializeField] private TextMeshProUGUI    _interactionLabel   = null;

    public void SetUpInteraction(Interactor interaction)
    {
        _interactionObject.SetActive(true);
        _interactionLabel.text = interaction.model.GetLabel();
    }

    public void ResetInteractionUI()
    {
        _interactionObject.SetActive(false);
    }
}