using System.Collections;
using System;
using UnityEngine;
using static NekraByte.Core.Behaviors;

public class Pistol : Mode_Semi
{
    protected override IEnumerator Shoot()
    {
        if (_isShooting || !_canShoot) yield return null;      

        _isShooting     = true;
        _canShoot       = false;

        ShootUsingRaycast(_playerInstance, _gunDataConteiner);

        StartCoroutine(base.Shoot());
    }
}