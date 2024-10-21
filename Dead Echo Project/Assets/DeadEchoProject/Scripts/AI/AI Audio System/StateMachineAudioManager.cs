using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NekraByte.Core.DataTypes;

public class StateMachineAudioManager : MonoBehaviour
{
    protected AiStateMachine        _machineInstance    = null;
    protected AudioManager          _audioManager       = null;

    [SerializeField] private bool   _humanoid           = true;
    [SerializeField] private bool   _stepBehavior       = true;

    [SerializeField] private    List<AudioCollection>  _collectionBases    = new List<AudioCollection>();
    [SerializeField] protected  AudioCollection _onRagdoll;
    
    [SerializeField] private FloorData              _floorData          = null;

    // ----------------------------------------------------------------------
    // Name: Start (Method)
    // Desc: This method is called on the scene start, mainly this method is
    //       getting all the class dependencies.
    // ----------------------------------------------------------------------
    protected virtual void Start()
    {
        _machineInstance    = GetComponent<AiStateMachine>();
        _audioManager       = AudioManager.Instance;
        _floorData          = new FloorData(_machineInstance.transform.gameObject, _machineInstance._collider);
    }

    protected virtual void Update()
    {
        
    }

    // ----------------------------------------------------------------------
    // Name: OnStepBehavior (Method)
    // Desc: This method uses the FloorData instance to detect the ground
    //       type that the GameObject is on top, and plays an equivalent
    //       audio clip on the passed position, this way representing an
    //       footstep behavior on an IA agent.
    // ----------------------------------------------------------------------
    public virtual void OnStepBehavior(Vector3 feetPos)
    {
        if (!_humanoid)     return;
        if (!_stepBehavior) return;
        if (_audioManager   == null) 
            return;

        AudioClip           clipToPlay          = null;
        AudioCollection     selectedCollection  = null;

        selectedCollection  = _collectionBases.Find(e => e.floorTag == _floorData.Type.ToString());
             
        if (selectedCollection == null) 
            return;

        clipToPlay = selectedCollection.audioClip;

        if (clipToPlay      == null) return;

        _audioManager.PlayOneShotSound(clipToPlay, feetPos, selectedCollection);
    }
}