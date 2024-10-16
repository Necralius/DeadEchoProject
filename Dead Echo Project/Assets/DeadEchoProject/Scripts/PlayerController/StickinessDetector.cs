using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickinessDetector : MonoBehaviour
{
    BodyController _controller = null;

    private void Start() => _controller = GetComponentInParent<BodyController>();

    private void OnTriggerStay(Collider other)
    {
        AiStateMachine machine = GameSceneManager.Instance.GetAIStateMachine(other.GetInstanceID());
        if (machine != null && _controller != null)
        {
            _controller.DoStickiness();
            machine.VisualThreat.Set(AITargetType.Visual_Player, 
                _controller.playerCollider, 
                _controller.transform.position, 
                Vector3.Distance(machine.transform.position, _controller.transform.position));
            machine.SetStateOverride(AIStateType.Attack);
        }
    }
}