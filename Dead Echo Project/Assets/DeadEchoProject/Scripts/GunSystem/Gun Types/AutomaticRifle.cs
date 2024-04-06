using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NekraByte.Core.Behaviors;

public class AutomaticRifle : Mode_Auto
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