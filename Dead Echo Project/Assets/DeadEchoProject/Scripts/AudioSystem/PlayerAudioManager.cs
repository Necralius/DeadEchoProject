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
                AudioBehavior(_floorData.Type, ActionType.Footstep);
                StepTimer = 0f;
            }
        }
        else StepTimer += Time.deltaTime;



        ////Get the current player speed
        //_playerSpeed = _playerInstance.BodyController.Velocity.magnitude;

        ////Verify if the player need an footstep audio, using as base the distance that the player has walked.
        //if (_playerInstance.BodyController._isGrounded && _playerInstance.BodyController._isWalking)
        //{
        //    _distanceCovered += (_playerSpeed * Time.deltaTime) * _speedModifier;
        //    if (_distanceCovered > _distacePerStep)
        //    {
        //        _distanceCovered = 0f;
        //        AudioBehavior(_floorData.Type, ActionType.Footstep);
        //    }          
        //}

        //Detects if the player is in the air.
        if (_playerInstance.BodyController._inAir) _airTime += Time.deltaTime;
        else if (_airTime >= 0.01f && _playerInstance.BodyController._isGrounded)
        {
            //If the player touch the ground and have an air time greater than 0.10 seconds, the land sound is played.
            _airTime = 0f;
            AudioBehavior(_floorData.Type, ActionType.Land);
            //Debug.Log("PAM: Landed 1."); -> Debug Line
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
    private void AudioBehavior(FloorData.FloorType groundType, ActionType actionType)
    {        
        AudioClip       clipToPlay          = null;
        AudioCollection selectedCollection  = null;

        //Try to find the correct audio collection, using as base the current action passed as argument.
        foreach (var collection in _footstepCollection)
        {
            //Debug.Log($"PAM -> Floor Finded: {groundType}, Collection Verified: {collection.floorTag}"); -> Debug Line
            if (groundType.ToString() ==  collection.floorTag)
            {
                selectedCollection = collection;
                //Debug.Log("PAM -> Finded the correct floor collection!"); -> Debug Line
                switch (actionType)
                {
                    case ActionType.Footstep    : clipToPlay = collection[0];   break;
                    case ActionType.Jump        : clipToPlay = collection[1];   break;
                    case ActionType.Land        : clipToPlay = collection[2];   break;
                    default: break;
                }
            }
        }

        //Returns the method if the collection or the audio manager are invalid objects.
        if (_audioManager       == null) return;
        if (clipToPlay          == null) return;

        //Plays the clip
        _audioManager?.PlayOneShotSound(clipToPlay, _footstepArea.position, selectedCollection);
    }

    // ----------------------------------------------------------------------
    // Name: JumpAudioAction (Method)
    // Desc: This method plays the jump action, the method is public, so is
    //       called in the player jump action.
    // ----------------------------------------------------------------------
    public void JumpAudioAction()
    {
        //Debug.Log("PAM: Started Jump."); -> Debug Line
        AudioBehavior(_floorData.Type, ActionType.Jump);
    }
    #endregion
}
//Enumeration that is used to identify an audio action.
public enum ActionType { Footstep, Land, Jump }