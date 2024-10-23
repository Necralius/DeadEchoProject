using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DeathController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _bodyParts = new List<GameObject>();
    [SerializeField] private List<GameObject> _objectsToDeactivate = new List<GameObject>();
    [SerializeField] private Animator _playerAnim = null;

    private List<Rigidbody> _ragdollRigidbodies = new List<Rigidbody>();

    [SerializeField] private Rigidbody _objectExample = null;

    public List<Rigidbody> RagdollRigidbodies
    {
        get
        {
            _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>().ToList();
            return _ragdollRigidbodies;
        }
    }

    public void SetUpRigidBodyConfigs()
    {
        if (_objectExample == null)
            return;
        RagdollRigidbodies.ForEach(e =>
        {
            e.mass                      = _objectExample.mass;
            e.isKinematic               = _objectExample.isKinematic;
            e.excludeLayers             = _objectExample.excludeLayers;
            e.detectCollisions          = _objectExample.detectCollisions;
            e.collisionDetectionMode    = _objectExample.collisionDetectionMode;
            e.freezeRotation            = _objectExample.freezeRotation;
        });

    }

    public void CallDeath()
    {
        _bodyParts.ForEach(e => e.transform.localScale = Vector3.one);
        _objectsToDeactivate.ForEach(e => e.SetActive(false));

        ActivateRagdoll();
    }

    public void ActivateRagdoll()
    {
        RagdollRigidbodies.ForEach(e => e.isKinematic = false);
        _playerAnim.enabled = false;
    }

    public void DeactiveRagdoll()
    {
        RagdollRigidbodies.ForEach(e => e.isKinematic = true);
        _playerAnim.enabled = true;
    }
}