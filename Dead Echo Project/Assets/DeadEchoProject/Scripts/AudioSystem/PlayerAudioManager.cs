using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NekraByte.Core.DataTypes;

public class PlayerAudioManager : MonoBehaviour
{
    private Animator _animator => GetComponent<Animator>();

    [SerializeField] private float _airTime     = 0f;

    [SerializeField] private float _minStepTime = 0.1f;
    private float _stepTimer = 0.1f;

    private float StepTimer
    {
        get => _stepTimer; 
        set
        {
            if (value >= _minStepTime)
                _stepTimer = _minStepTime;
            else _stepTimer = value;
        }    
    } 

    //Dependencies
    [Header("Dependencies")]
    [SerializeField] private    FloorData                   _floorData                  = null;
    [SerializeField] private    Transform                   _footstepArea               = null;
    [SerializeField] private    List<AudioCollection>       _footstepCollection         = new List<AudioCollection>();
    private                     PlayerManager               _playerInstance             = null;
    private                     AudioManager                _audioManager               = null;

    private int _stepHash = Animator.StringToHash("StepHash");

    // ------------------------------------------ Methods ------------------------------------------ //

    #region - BuiltIn Methods -
    // ----------------------------------------------------------------------
    // Name: Start (Method)
    // Desc: This method is called on the game start, mainly the method get
    //       all the class depedencies, and neclare the class floor data.
    // ----------------------------------------------------------------------
    private void Start()
    {
        _playerInstance     = GetComponent<PlayerManager>();
        _audioManager       = AudioManager.Instance;
        _floorData          = new FloorData(_playerInstance.gameObject, _playerInstance.Collider);
    }

    // ----------------------------------------------------------------------
    // Name: Update (Method)
    // Desc: This method is called on every unity frame update.
    // ----------------------------------------------------------------------
    private void Update()
    {
        if (StepTimer >= _minStepTime)
        {
            if (_animator.GetFloat(_stepHash) >= 0.1)
            {
                FootstepAudioBehavior(_floorData.Type, ActionType.Footstep);
                StepTimer = 0f;
            }
        }
        else StepTimer += Time.deltaTime;

        
        if (_playerInstance.BodyController._inAir) _airTime += Time.deltaTime;
        else if (_airTime >= 0.01f && _playerInstance.BodyController._isGrounded)
        {
            _airTime = 0f;
            FootstepAudioBehavior(_floorData.Type, ActionType.Land);
        }
    }
    #endregion

    #region - Audio Management -
    // ----------------------------------------------------------------------
    // Name: AudioBehavior (Method)
    // Desc: This method call an audio using as base the ground type, and the
    //       action type, basically, the floor data verify what ground the
    //       player is stepping, and the action type, represents the action
    //       that will be shoted (Exp: Jump, Land, Footstep).
    // ----------------------------------------------------------------------
    private void FootstepAudioBehavior(FloorData.FloorType groundType, ActionType actionType)
    {        
        AudioClip       clipToPlay          = null;
        AudioCollection selectedCollection  = null;

        selectedCollection = _footstepCollection.Find(e => e.floorTag == groundType.ToString());

        if (selectedCollection == null)
            return;

        switch (actionType)
        {
            case ActionType.Footstep:   clipToPlay = selectedCollection[0]; break;
            case ActionType.Jump:       clipToPlay = selectedCollection[1]; break;
            case ActionType.Land:       clipToPlay = selectedCollection[2]; break;
            default: break;
        }

        if (_audioManager == null || clipToPlay == null) 
            return;

        _audioManager?.PlayOneShotSound(selectedCollection.audioGroup, 
            clipToPlay, 
            _footstepArea.position, 
            _playerInstance.BodyController._isCrouching ? 0.2f : 0.6f, 0);
    }

    // ----------------------------------------------------------------------
    // Name: JumpAudioAction (Method)
    // Desc: This method plays the jump action, the method is public, so is
    //       called in the player jump action.
    // ----------------------------------------------------------------------
    public void JumpAudioAction()
    {
        //Debug.Log("PAM: Started Jump."); -> Debug Line
        FootstepAudioBehavior(_floorData.Type, ActionType.Jump);
    }
    #endregion
}

//Enumeration that is used to identify an audio action.
public enum ActionType { Footstep, Land, Jump }