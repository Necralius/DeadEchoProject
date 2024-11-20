using UnityEngine;
using static NekraByte.Core.Enumerators;
using UnityEngine.InputSystem;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(InteractionView))]
public class InteractionController : MonoBehaviour
{
    //Direct Dependencies
    private InteractionView _interactionView    => GetComponent<InteractionView>();
    private InputManager    _inputManager       => InputManager.Instance;

    [Header("Current Interaction")]
    [SerializeField] private Interactor _interactionInArea      = null;
    [SerializeField] private bool       _hasInteractionAround   = false;

    [SerializeField] private bool       _interacting            = false;

    private void Update()
    {      
        if (_interactionInArea)
        {
            _interactionView.SetUpInteraction(_interactionInArea);

            if (_interacting && GetAction(_interactionInArea.model.Button).inProgress)
            {
                _interactionInArea.InteractionPersistent();
                if (GetAction(_interactionInArea.model.Button).WasReleasedThisFrame())
                    _interacting = false;
            }

            if (GetAction(_interactionInArea.model.Button).WasPressedThisFrame())
            {
                _interactionInArea.InteractStart();
                _interacting = true;
            }        
        }
        else 
            _interactionView.ResetInteractionUI();
    }

    private InputAction GetAction(InteractionButton buttton)
    {
        InputAction action = null;
        switch (buttton)
        {
            case InteractionButton.TAB:     action = _inputManager.Tab_Action.Action;   break;
            case InteractionButton.E:       action = _inputManager.E_Action.Action;     break;
            case InteractionButton.Q:       action = _inputManager.Q_Action.Action;     break;
            case InteractionButton.ENTER:   action = _inputManager.Enter_Action.Action; break;
            default:                        action = _inputManager.E_Action.Action;     break;
        }

        return action;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interaction"))
        {
            if (!other.GetComponent<Interactor>()) return;

            _interactionInArea = other.GetComponent<Interactor>();
            _hasInteractionAround = true;

            _interactionInArea.InteractionEnter();
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Interaction"))
    //    {
    //        if (!other.GetComponent<Interactor>()) return;

    //        _interactionInArea = other.GetComponent<Interactor>();
    //        _hasInteractionAround = true;
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (_interactionInArea != null)
            _interactionInArea.InteractionExit();

        _interactionInArea = null;
        _hasInteractionAround = false;
        _interacting = false;
    }
}