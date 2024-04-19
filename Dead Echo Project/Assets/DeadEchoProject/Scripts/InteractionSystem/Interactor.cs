using System;
using UnityEngine;
using UnityEngine.Events;

public class Interactor : MonoBehaviour
{
    [Tooltip("Needs an collider of any type, also the collider needs to be trigger type.")]
    public InteractionModel model;

    [Header("Interaction Actions")]
    [Tooltip("This action is called when the player press the interaction button on the interaction start")]
    [SerializeField] private UnityEvent onInteractStart         = new UnityEvent();

    [Tooltip("This action is called when the player press the interaction button more than one time")]
    [SerializeField] private UnityEvent onInteractPersistent    = new UnityEvent();

    [Tooltip("This action is called when the player enter the interaction area")]
    [SerializeField] private UnityEvent onInteractEnter         = new UnityEvent();

    [Tooltip("This action is called when the player exit the interaction area")]
    [SerializeField] private UnityEvent onInteractExit          = new UnityEvent();

    [SerializeField] private bool _debug = false;

    public UnityEvent OnEnter   { get => onInteractEnter;   }
    public UnityEvent OnStart   { get => onInteractStart;   }
    
    public UnityEvent OnExit    { get => onInteractExit;    }

    public virtual void InteractStart()
    {
        try
        {
            onInteractStart.Invoke();
        }
        catch(Exception ex)
        {
            Debug.LogWarning(ex);
        }

        if (_debug) 
            Debug.Log($"Interactor: {gameObject.name} started a interaction.");
    }

    public virtual void InteractionPersistent()
    {
        onInteractPersistent.Invoke();
        if (_debug) 
            Debug.Log($"Interactor: {gameObject.name} is persisting in the interaction.");
    }

    public virtual void InteractionEnter()
    {
        onInteractEnter.Invoke();
        if (_debug)
            Debug.Log($"Interactor: {gameObject.name} is entering in the interaction area.");
    }

    public virtual void InteractionExit()
    {
        onInteractExit.Invoke();
        if (_debug)
            Debug.Log($"Interactor: {gameObject.name} is getting out of the interaction area.");
    }
}