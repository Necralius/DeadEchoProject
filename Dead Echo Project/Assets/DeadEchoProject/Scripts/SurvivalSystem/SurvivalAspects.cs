using System.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;

public class SurvivalAspects : MonoBehaviour
{
    [SerializeField] private float _health = 100;
    //private float 

    [SerializeField] private SurvivalAspect _hunger  = new SurvivalAspect();
    [SerializeField] private SurvivalAspect _thirst  = new SurvivalAspect();
    [SerializeField] private SurvivalAspect _stamina = new SurvivalAspect();

    [SerializeField] private bool isRunning  = false;
    [SerializeField] private bool isWalking  = false;
    [SerializeField] private bool isCrouched = false;

    private void Update()
    {
        if (_hunger is not null) 
            _hunger.Update(ref _health, isRunning, isWalking);

        if (_thirst is not null) 
            _thirst.Update(ref _health, isRunning, isWalking);

        if (_stamina is not null)
            _stamina.Update(ref _health, isRunning, isWalking);
    }
}

[Serializable]
public class SurvivalAspect
{
    [Range(0, 400)] public float Value              = 0f;
    [Range(0, 400)] public float MaxValue           = 0f;
    [Range(0, 20)]  public float HealthLossByTime   = 1f;
    [Range(0, 20)]  public float AspectLossByTime   = 1f;
    [Range(1f, 5f)] public float SprintMultiplier   = 1f;
    [Range(1f, 5f)] public float WalkMultiplier     = 1f;

    [Range(0, 40)]  public float LossLimit = 0;

    public bool HealthLossActive { get => Value <= LossLimit; }

    /// <summary>
    /// This method updates this
    /// </summary>
    /// <param name="currentHealth"></param>
    /// <param name="effectors"></param>
    public void Update(ref float currentHealth, bool isRunning = false, bool isWalking = false)
    {
        Value -= (AspectLossByTime / 1000) * (isRunning ? SprintMultiplier : WalkMultiplier);
        if (HealthLossActive) 
            currentHealth -= HealthLossByTime / 1000;
    }
}