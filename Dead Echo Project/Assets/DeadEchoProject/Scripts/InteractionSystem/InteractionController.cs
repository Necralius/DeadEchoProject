using UnityEngine;

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

            if (_interacting && _inputManager.E_Action.Action.inProgress)
            {
                _interactionInArea.InteractionPersistent();
                if (_inputManager.E_Action.Action.WasReleasedThisFrame())
                    _interacting = false;
            }

            if (_inputManager.E_Action.Action.WasPressedThisFrame())
            {
                _interactionInArea.InteractStart();
                _interacting = true;
            }        
        }
        else _interactionView.ResetInteractionUI();
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