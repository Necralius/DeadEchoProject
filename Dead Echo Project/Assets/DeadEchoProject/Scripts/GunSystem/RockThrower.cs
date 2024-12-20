using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockThrower : MonoBehaviour
{
    //Dependencies
    private                  Animator       _anim               => GetComponent<Animator>();
    [SerializeField] private BodyController _playerController   = null;
    [SerializeField] private GameObject     _rockObject         = null;

    // Private animations hashes
    private int startThrowingHash       = Animator.StringToHash("StartThrowing");
    private int endThrowingHash         = Animator.StringToHash("EndThrowing");
    private int cancelThrowingHash      = Animator.StringToHash("CancelThrowing");

    // ------------------------------------------ Methods ------------------------------------------ //

    #region - Rock Throwing Action -
    // ----------------------------------------------------------------------
    // Name: ThrowRock (Method)
    // Desc: This method start the rock throwing action.
    // ----------------------------------------------------------------------
    public void ThrowRock()
    {
        if (_playerController == null) 
            return;

        _anim.SetTrigger(startThrowingHash);
    }

    // ----------------------------------------------------------------------
    // Name: SpawnRock (Method)
    // Desc: This method spawns an rock prefab on the rock spawn postion,
    //       using the object pooler instance, also, the method applies an
    //       force to the rock object to simulat an object throw.
    // ----------------------------------------------------------------------
    public void SpawnRock()
    {
        GameObject rock = ObjectPooler.Instance.SpawnFromPool("Rock", 
            _playerController._rockThrowPosition.position, 
            _playerController._rockThrowPosition.rotation);

        if (rock.GetComponent<Rigidbody>())
        {
            Rigidbody rb = rock.GetComponent<Rigidbody>();
            Vector3 forceToAdd = _playerController._camController.transform.forward *
                _playerController._objectThrowForce +
                transform.up *
                _playerController._objectThrowUpForce;

            rb.AddForce(forceToAdd, ForceMode.Impulse);
        }
        _rockObject.gameObject.SetActive(false);
    }

    // ----------------------------------------------------------------------
    // Name: EquipPreviusGun
    // Desc: This method equip the previus gun after the rock throwing.
    // ----------------------------------------------------------------------
    IEnumerator EquipPreviusGun()
    {
        yield return new WaitForSeconds(0.5f);
        _playerController.EquipCurrentGun();
    }
    #endregion

    #region - Animation Events -
    //Animation Events
    public void ActivateArms()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        _rockObject.gameObject.SetActive(true);
    }
    public void DeactivateArms()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        _rockObject.gameObject.SetActive(false);
    }
    public void FinishThrow()       => _anim.SetTrigger(endThrowingHash);
    public void CancelThrowing()    => _anim.SetTrigger(cancelThrowingHash);
    public void FinishThrowAction() => _playerController._isThrowingObject = false;
    public void EquipCurrentGun()   => StartCoroutine(EquipPreviusGun());
    #endregion
}