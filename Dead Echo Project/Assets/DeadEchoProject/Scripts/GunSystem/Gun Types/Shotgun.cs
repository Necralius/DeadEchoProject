using System.Collections;
using UnityEngine;
using static NekraByte.Core.Behaviors;

public class Shotgun : Mode_Semi
{
    //Private Data
    private int endReloadHash       = Animator.StringToHash("EndReload");
    private int cancelReloadHash    = Animator.StringToHash("CancelReload");

    protected override void Update()
    {
        if (_isReloading && _inputManager.mouseLeftAction.WasPressedThisFrame())
        {
            _animator.SetTrigger(cancelReloadHash);
            _isReloading = false;
            UI_Update();
        }
        base.Update();
    }

    protected override IEnumerator Shoot()
    {
        if (_isShooting || !_canShoot) yield return null;

        _isShooting = true;
        _canShoot   = false;

        for (int i = 0; i < _gunDataConteiner.gunBulletSettings._bulletsPerShoot; i++)
        {
            float x = Random.Range(-_gunDataConteiner.gunBulletSettings._bulletSpread, -_gunDataConteiner.gunBulletSettings._bulletSpread);
            float y = Random.Range(-_gunDataConteiner.gunBulletSettings._bulletSpread, -_gunDataConteiner.gunBulletSettings._bulletSpread);

            Vector3 direction = _playerInstance.CameraController.mainLookCamera.transform.forward + new Vector3(x, y, 0);

            ShootUsingRaycast(_playerInstance, _gunDataConteiner, direction);
        }

        StartCoroutine(base.Shoot());
    }

    protected override void Reload()
    {
        _isReloading = true;
        SS_Reload();

        _animator.SetTrigger(_isReloadingHash);
        _animator.SetFloat(_reloadFactorHash, 0);
    }

    public override void EndReload()
    {
        UI_Update();

        _isReloading = false;
    }

    public void TriggerBullet()
    {
        if (_gunDataConteiner.ammoData._magAmmo < (_gunDataConteiner.ammoData._magMaxAmmo - 1)) _gunDataConteiner.ammoData._magAmmo++;
        UI_Update();
    }

    public void VerifyNeed()
    {
        if (!(_gunDataConteiner.ammoData._magAmmo < (_gunDataConteiner.ammoData._magMaxAmmo - 1))) _animator.SetTrigger(endReloadHash);
    }

    public void TriggerLastBullet()
    {
        _gunDataConteiner.ammoData._magAmmo++;
        UI_Update();
    }

    private void SS_Reload()
    {
        if (_gunAudioAsset.ReloadClip != null)
            AudioManager.Instance.PlayOneShotSound("Effects", _gunAudioAsset.AimClip, transform.position, 1f, 0f, 128);
    }

    public void SS_BulletTrigger()
    {
        if (_gunAudioAsset.ReloadClipVar1 != null)
            AudioManager.Instance.PlayOneShotSound("Effects", _gunAudioAsset.ReloadClipVar1, transform.position, 1f, 0f, 128);
    }

    public void SS_PumpAction()
    {
        if (_gunAudioAsset.BoltActionClip != null)
            AudioManager.Instance.PlayOneShotSound("Effects", _gunAudioAsset.BoltActionClip, transform.position, 1f, 0f, 128);
    }
}