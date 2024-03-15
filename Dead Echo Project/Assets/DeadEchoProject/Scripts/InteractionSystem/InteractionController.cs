using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(InteractionView))]
public class InteractionController : MonoBehaviour
{
    //Direct Dependencies
    private InteractionView _interactionView => GetComponent<InteractionView>();

    [Header("Current Interaction")]
    [SerializeField] private Interactor _interactionInArea = null;
    [SerializeField] private bool _hasInteractionAround = false;

    [SerializeField] private bool _interacting = false;

    private void Update()
    {
        if (_interactionInArea) 
            _interactionView.SetUpInteraction(_interactionInArea);
        else _interactionView.ResetInteractionUI();
    }

    public void InteractWith()
    {
        if (_interactionInArea)
        {
            if (_interacting)
            {
                _interactionInArea.InteractionPersistent();
                return;
            }
            _interactionInArea.InteractStart();
            _interacting = true;
        }
        else return;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interaction"))
        {
            if (!other.GetComponent<Interactor>()) return;

            _interactionInArea      = other.GetComponent<Interactor>();
            _hasInteractionAround   = true;

            _interactionInArea.InteractionEnter();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Interaction"))
        {
            if (!other.GetComponent<Interactor>()) return;

            _interactionInArea      = other.GetComponent<Interactor>();
            _hasInteractionAround   = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (_interactionInArea != null)
            _interactionInArea.InteractionExit();

        _interactionInArea      = null;
        _hasInteractionAround   = false;
        _interacting            = false;
    }
}