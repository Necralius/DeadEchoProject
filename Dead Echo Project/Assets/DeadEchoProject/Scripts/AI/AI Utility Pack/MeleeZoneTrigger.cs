using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        AiStateMachine machine = GameSceneManager.Instance.GetAIStateMachine(other.GetInstanceID());
        if (machine) machine.inMeleeRange = true;
    }
    private void OnTriggerExit(Collider other)
    {
        AiStateMachine machine = GameSceneManager.Instance.GetAIStateMachine(other.GetInstanceID());
        if (machine) machine.inMeleeRange = false;
    }
}