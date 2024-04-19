using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using static NekraByte.Core.Enumerators;
using static NekraByte.Core.DataTypes;
using TMPro;

[RequireComponent(typeof(Animator))]
public abstract class GunBase : MonoBehaviour
{
    #region - Dependencies -
    [HideInInspector] protected Animator                _animator           = null;
    [SerializeField] protected PlayerManager           _playerInstance     = null;
    [HideInInspector] protected CharacterManager        _characterManager   = null;
    [SerializeField]  protected GunDataConteiner        _gunDataConteiner   = null;
    [SerializeField]  protected AudioAsset              _gunAudioAsset      = new AudioAsset();
    protected                   InputManager            _inputManager       = null;
    protected                   GunProceduralRecoil     _recoilAsset        = null;
    private                     Transform               _aimHolder          = null;
    public                      ParticleSystem          _muzzleFlash        = null;
    #endregion

    //UI Assets
    [SerializeField] ColorFader ammoTextFader;
    [SerializeField] private TextMeshPro _ammoText = null;
    [SerializeField] private TextMeshPro _gunName = null;

    #region - Gun State -
    [Header("Gun State"), Tooltip("Gun current states")]
    public bool _isEquiped      = false;
    public bool _isReloading    = false;
    public bool _isShooting     = false;
    public bool _isAiming       = false;
    public bool _aimOverride    = false;
    public bool _canShoot       = true;
    #endregion

    #region - Animation Hashes -
    private     int _isWalkingHash      = Animator.StringToHash("isWalking");
    private     int _isRunningHash      = Animator.StringToHash("isRunning");
    protected   int _isReloadingHash    = Animator.StringToHash("isReloading");
    protected   int _reloadFactorHash   = Animator.StringToHash("ReloadFactor");
    protected   int _holstWeaponHash    = Animator.StringToHash("HolstWeapon");
    protected   int _shootHash          = Animator.StringToHash("Shoot");
    #endregion

    #region - Gun Mode System -
    //[Header("Gun Mode System")]
    private List<GunMode> gunModes = new List<GunMode>();
    int _gunModeIndex = 0;
    #endregion

    #region - Gun Clipping Prevetion -
    [Header("Gun Clipping Prevention")]
    [SerializeField, Range(0.1f, 3f)]  private float    _checkDistance  = 1f;
    [SerializeField, Range(0.1f, 20f)] private float    _lerpSpeed      = 2f;
    [SerializeField] private Vector3                    _newDirection   = Vector3.zero;
    [SerializeField] private bool                       _isClipped      = false;
    [SerializeField] private bool                       _forcedClip     = false;
    [SerializeField] private LayerMask                  _clipingMask;
    private Transform                                   _clipProjector  = null;

    private float       _lerpPos;
    private RaycastHit  _hit;
    #endregion

    [SerializeField] private float _smoothTime = 10f;

    private Vector3     _originalWeaponPosition;
    private Camera      _camera;

    private bool permanentHolst = false;

    public GunDataConteiner GunData { get => _gunDataConteiner; set => _gunDataConteiner = value; }

    //----------------------------------- Methods -----------------------------------//

    #region - BuiltIn Methods -
    // ----------------------------------------------------------------------
    // Name : Start
    // Desc : This method is called on the script start, the script mainly
    //        get all the system dependencies an set the default values of
    //        the class.
    // ----------------------------------------------------------------------
    protected virtual void Start()
    {
        _animator           = GetComponent<Animator>();
        _recoilAsset        = GetComponent<GunProceduralRecoil>();
        _playerInstance     = GetComponentInParent<FPSCamera>().PlayerManager;
        _ammoText           = GetComponentInChildren<TextMeshPro>();
        _inputManager       = _playerInstance.InputManager;

        _aimHolder          = AnimationLayer.GetAnimationLayer("AimLayer",      _playerInstance._animLayers).layerObject.transform;
        _clipProjector      = AnimationLayer.GetAnimationLayer("GunLayer",      _playerInstance._animLayers).layerObject.transform;
        _camera             = AnimationLayer.GetAnimationLayer("CameraLayer",   _playerInstance._animLayers).layerObject.GetComponent<Camera>();
        _recoilAsset.SetUp(   AnimationLayer.GetAnimationLayer("RecoilLayer",   _playerInstance._animLayers).layerObject);

        ammoTextFader = _playerInstance?.DynamicUI_Manager?.GetFader(_gunDataConteiner.gunData.gunName + " Ammo");
        ammoTextFader?.FadeIn(4f);

        _originalWeaponPosition = _aimHolder.localPosition;
        
        if (_gunDataConteiner is null)
            return;

        _recoilAsset?.InitializeData(_gunDataConteiner.recoilData);

        gunModes = new List<GunMode>();
        switch (_gunDataConteiner.gunData.shootType)
        {
            case ShootType.Semi_Shotgun:
                gunModes.Add(GunMode.Locked);
                gunModes.Add(GunMode.Semi);
                break;
            case ShootType.Semi_Rifle:
                gunModes.Add(GunMode.Locked);
                gunModes.Add(GunMode.Semi);
                break;
            case ShootType.Semi_Pistol:
                gunModes.Add(GunMode.Locked);
                gunModes.Add(GunMode.Semi);
                break;
            case ShootType.Auto_Shotgun:
                gunModes.Add(GunMode.Locked);
                gunModes.Add(GunMode.Semi);
                gunModes.Add(GunMode.Auto);
                break;
            case ShootType.Auto_Rifle:
                gunModes.Add(GunMode.Locked);
                gunModes.Add(GunMode.Semi);
                gunModes.Add(GunMode.Auto);
                break;
            case ShootType.Auto_Pistol:
                gunModes.Add(GunMode.Locked);
                gunModes.Add(GunMode.Semi);
                gunModes.Add(GunMode.Auto);
                break;
            case ShootType.Sniper:
                gunModes.Add(GunMode.Locked);
                gunModes.Add(GunMode.Semi);
                break;
        }

        UI_Update();
    }

    // ----------------------------------------------------------------------
    // Name : Update
    // Desc : This method its called at every frame rendered, the method
    //        explanation its inside it.
    // ----------------------------------------------------------------------
    protected virtual void Update()
    {
        //First, the method verifies if all the class dependencies is valid, if its not the program behavior is returned, otherwise,
        //the program continue to his default behavior.
        if (!_isEquiped)                                    return;
        if (_playerInstance                     == null)    return;
        if (_inputManager                       == null)    return;
        if (_playerInstance._armsAnimator       == null)    return;
        if (_animator                           == null)    return;
        if (_recoilAsset                        == null)    return;
        if (GameSceneManager.Instance.inventoryIsOpen)      return;

        _ammoText.color = ammoTextFader.currentColor;

        _forcedClip = _gunDataConteiner.gunData.gunMode == GunMode.Locked;

        _recoilAsset?.InitializeData(_gunDataConteiner.recoilData);

        _isAiming = _isClipped ? false : _playerInstance.BodyController._isSprinting ? false : _inputManager.mouseRightAction.State;

        //The class focus on limiting the actions, using ifs to limit the actions based in expressions.
        if (!_playerInstance.BodyController._isSprinting)
        {
            if (!_aimOverride)
            { 
                if (GameStateManager.Instance != null)
                {
                    if (GameStateManager.Instance.currentApplicationData.aimType == 0)
                        _isAiming = _inputManager.mouseRightAction.State;
                    else if (GameStateManager.Instance.currentApplicationData.aimType == 1)
                    {
                        if (_inputManager.mouseRightAction.Action.WasPerformedThisFrame()) _isAiming = true;
                        else if (_inputManager.mouseRightAction.Action.WasPerformedThisFrame() && _isAiming) _isAiming = false;
                    }
                }
            }

            _recoilAsset._isAiming = _isAiming;

            if (_playerInstance.BodyController._isThrowingObject) 
                return;

            /* The below statements verifies if the player triggered the reload button
             * and if is not reloading, if the current mag ammo is different  from its
             * maximum and if has any ammo in the inventory.
             */
            if (_inputManager.R_Action.Action.WasPressedThisFrame() && !_isReloading && !(_gunDataConteiner.ammoData._magAmmo == _gunDataConteiner.ammoData._magMaxAmmo) && _gunDataConteiner.ammoData._bagAmmo > 0) Reload();

            /* The  below  statements execute  the  main gun  behavior  action,  first
             * verifing  if  the  isn't  reloading  and if  can shoot,  later  also is 
             * verified if the mag ammo is greater than zero, if it is, the  statement 
             * enters in an switch structure, that defies the shoot behavior  based on 
             * the gun state.
            */

            if (_inputManager.mouseLeftAction.Action.WasPressedThisFrame() && _gunDataConteiner.ammoData._magAmmo <= 0) 
                AudioManager.Instance.PlayOneShotSound("Effects", _playerInstance.gunShootJam, transform.position, 1f, 0f, 128);
            if (!_isReloading && _canShoot && !_isClipped)
            {
                if (_gunDataConteiner.ammoData._magAmmo > 0)
                {
                    switch (_gunDataConteiner.gunData.gunMode)
                    {
                        case GunMode.Auto:
                            if (_inputManager.mouseLeftAction.Action.IsPressed()) StartCoroutine(Shoot());
                            break;
                        case GunMode.Semi:
                            if (_inputManager.mouseLeftAction.Action.WasPressedThisFrame()) StartCoroutine(Shoot());
                            break;
                        case GunMode.Locked:
                            if (_inputManager.mouseLeftAction.Action.WasPressedThisFrame()) 
                                AudioManager.Instance.PlayOneShotSound("Effects", _playerInstance.gunShootJam, transform.position, 1f, 0f, 128);
                            break;
                    }
                }

                if (_inputManager.mouseLeftAction.Action.WasPressedThisFrame()) UI_Update();
                /* The below statement verifies if the user pressed the change gun mode
                 * buttons, if it is, the gun mode is changed.
                 */
            }
            if (_inputManager.B_Action.Action.WasPressedThisFrame()) ChangeGunMode();
        }
        if (_inputManager.mouseRightAction.Action.WasPressedThisFrame()) 
            AudioManager.Instance.PlayOneShotSound("Effects", _gunAudioAsset.AimClip, transform.position, 1f, 0f, 128);
        Aim(); //-> This statement calls the aim position calculation method.

        //The below statements set the animations on the main arms animator controler.
        _playerInstance._armsAnimator.SetBool(_isWalkingHash, _playerInstance.BodyController._isWalking);
        _playerInstance._armsAnimator.SetBool(_isRunningHash, _playerInstance.BodyController._isSprinting);

        ClipPrevetionBehavior();

        //NOTE: On override, always mantain the base code using the base.Update();
        //-> Otherwise, every systems of the gun base will be broken!!
    }
    #endregion

    #region - Clip Prevetion Behavior -
    // ----------------------------------------------------------------------
    // Name: ClipPreventionBehavior
    // Desc: This method prevent the gun clipping in surfaces, basically the
    //       method detects an surface collision in front of the gund,
    //       later, the gun rotates towards the new rotation seted on the
    //       _newDirection.
    // ----------------------------------------------------------------------
    private void ClipPrevetionBehavior()
    {
        if (_clipProjector is null)
        {
            Debug.Log("Clip projector is Null");
            return;
        }

        _lerpPos    = _isClipped || _forcedClip ? 1 - (_hit.distance / _checkDistance) : 0;

        _isClipped  = Physics.Raycast(_clipProjector.transform.position, _clipProjector.transform.forward, out _hit, _checkDistance, _clipingMask);

        Mathf.Clamp01(_lerpPos);

        Quaternion newRot = Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(_newDirection), _lerpPos);

        _aimHolder.transform.localRotation = Quaternion.Lerp(_aimHolder.transform.localRotation, newRot, _lerpSpeed * Time.deltaTime);
    }
    #endregion

    #region - Shoot Behavior -
    // ----------------------------------------------------------------------
    // Name : Shoot
    // Desc : This method represents an base shoot behavior that compulsorily
    //        needs to be overrided on inherited class.
    // ----------------------------------------------------------------------
    protected virtual IEnumerator Shoot()
    {
        if (_gunDataConteiner   is null)                yield break;
        if (_muzzleFlash        is null)                yield break;
        if (_recoilAsset        is null)                yield break;
        if (GameSceneManager.Instance._gameIsPaused || GameSceneManager.Instance.inventoryIsOpen)    yield break;

        AudioManager.Instance.PlayOneShotSound("Effects", _gunAudioAsset.ShootClip, transform.position, 1f, 0f, 128);

        if (_gunDataConteiner.gunData.shootType is ShootType.Semi_Shotgun ||
            _gunDataConteiner.gunData.shootType is ShootType.Semi_Pistol  ||
            _gunDataConteiner.gunData.shootType is ShootType.Sniper) _animator.SetTrigger(_shootHash);

        _recoilAsset.RecoilFire();
        _gunDataConteiner.ammoData._magAmmo--;
        _muzzleFlash.Emit(1);
        UI_Update();

        yield return new WaitForSeconds(_gunDataConteiner.gunData.rateOfFire);

        _isShooting     = false;
        _canShoot       = true;
    }
    #endregion

    #region - Aim System -
    // ----------------------------------------------------------------------
    // Name : Aim
    // Desc : This method manages the aim behavior, changing the aim target
    //        position and adding offsets based in the current gun state. 
    // ----------------------------------------------------------------------
    private void Aim()
    {
        if (_aimHolder  is null) return;
        if (_camera     is null) return;

        if (_isAiming)
        {
            // Calculate the target screen position at the center of the screen
            Vector3 targetScreenPosition = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

            // Convert the screen position to a world position based on the weapon's distance from the camera
            float distanceFromCamera    = Vector3.Distance(_aimHolder.position, _camera.transform.position);
            Vector3 targetWorldPosition = _camera.ScreenToWorldPoint(new Vector3(targetScreenPosition.x, targetScreenPosition.y, distanceFromCamera));

            // Convert the world position to a local position relative to the weapon
            Vector3 targetLocalPosition = _aimHolder.parent.InverseTransformPoint(targetWorldPosition);

            // Apply the specified offsets
            targetLocalPosition += new Vector3(_gunDataConteiner.gunData.aimOffset.x, 
                _gunDataConteiner.gunData.aimOffset.y, 
                _gunDataConteiner.gunData.aimOffset.z);

            // Apply the reload offset if applicable
            targetLocalPosition.z = _isReloading ? _gunDataConteiner.gunData.aimOffset.z + _gunDataConteiner.gunData._aimReloadOffset : 
                _gunDataConteiner.gunData.aimOffset.z;

            // Lerp the weapon position to match the target position
            _aimHolder.localPosition = Vector3.Lerp(_aimHolder.localPosition, targetLocalPosition, Time.deltaTime * _smoothTime);
        }
        else 
            _aimHolder.localPosition = Vector3.Lerp(_aimHolder.localPosition, _originalWeaponPosition, Time.deltaTime * _smoothTime);
    }
    #endregion

    #region - Reload Behavior -
    // ----------------------------------------------------------------------
    // Name : Reload
    // Desc : This method execute an reload action, playing the proper
    //        reload animation.
    // OBS  : The reload animation selection works based on the ammo needs,
    //        if the need is equals the maximum mag  ammo, the  full reload
    //        animation is played, is equals the maximum mag ammo, the full
    //        reload  animation  is  played, otherwise, one  of  two reload
    //        variations is randomly selected.
    // ----------------------------------------------------------------------
    protected virtual void Reload()
    {
        _isReloading = true;

        int reloadIndex = _gunDataConteiner.ammoData._magMaxAmmo - _gunDataConteiner.ammoData._magAmmo == _gunDataConteiner.ammoData._magMaxAmmo ? 2 : Random.Range(0, 2);

        SS_Reload(reloadIndex);

        _animator.SetTrigger(_isReloadingHash);
        _animator.SetFloat(_reloadFactorHash, reloadIndex);
    }

    // ----------------------------------------------------------------------
    // Name : EndReload
    // Desc : This method triggers the reload calculations and end the
    //        reload state boolean.
    // ----------------------------------------------------------------------
    public virtual void EndReload()
    {
        int quantityNeeded = _gunDataConteiner.ammoData._magMaxAmmo - _gunDataConteiner.ammoData._magAmmo;
        if (quantityNeeded > _gunDataConteiner.ammoData._bagAmmo)
        {
            _gunDataConteiner.ammoData._magAmmo += _gunDataConteiner.ammoData._bagAmmo;
            _gunDataConteiner.ammoData._bagAmmo = 0;
        }
        else if (quantityNeeded == _gunDataConteiner.ammoData._bagAmmo)
        {
            _gunDataConteiner.ammoData._bagAmmo = 0;
            _gunDataConteiner.ammoData._magAmmo = _gunDataConteiner.ammoData._magMaxAmmo;
        }
        else if (quantityNeeded < _gunDataConteiner.ammoData._bagAmmo)
        {
            _gunDataConteiner.ammoData._magAmmo = _gunDataConteiner.ammoData._magMaxAmmo;
            _gunDataConteiner.ammoData._bagAmmo -= quantityNeeded;
        }

        UI_Update();

        _isReloading = false;
    }
    #endregion

    #region - UI Update -
    // ----------------------------------------------------------------------
    // Name : UI_Update
    // Desc : This method updates the gun UI system.
    // ----------------------------------------------------------------------
    public void UI_Update()
    {
        if (ammoTextFader is null)  return;
        if (_ammoText is null)  return;
        if (!_isEquiped)        return;

        ammoTextFader?.FadeIn(4f);

        _ammoText.text = $"{_gunDataConteiner.ammoData._magAmmo}/{_gunDataConteiner.ammoData._bagAmmo}";
        //_playerController.txt_GunName.text = _gunDataConteiner.gunData.gunName;

        //InGame_UIManager.Instance.UpdateMode(_gunDataConteiner.gunData.gunMode, gunModes);
    }
    #endregion

    #region - Change Gun Mode -
    // ----------------------------------------------------------------------
    // Name : ChangeGunMode
    // Desc : This method manages the gun mode change system, changing and
    //        clamping  the gun  mode represented in  an int  value, later
    //        passing the value to the gun data conteiner, also the method
    //        play the change gun mode sound and updates the UI.
    // ----------------------------------------------------------------------
    private void ChangeGunMode()
    {
        _gunModeIndex++;
        if (_gunModeIndex > gunModes.Count - 1) _gunModeIndex = 0;

        _gunModeIndex = Mathf.Clamp(_gunModeIndex, 0, gunModes.Count);

        _gunDataConteiner.gunData.gunMode = gunModes[_gunModeIndex];

        UI_Update(); // -> Updates the gun UI.
        AudioManager.Instance.PlayOneShotSound("Effects", _playerInstance.changeGunMode, transform.position, 1f, 0f, 128);
    }
    #endregion

    protected void SS_Reload(int reloadIndex)
    {
        if (_gunAudioAsset.ReloadClip       is null) return;
        if (_gunAudioAsset.ReloadClipVar1   is null) return;
        if (_gunAudioAsset.FullReloadClip   is null) return;

        switch (reloadIndex)
        {
            case 0: AudioManager.Instance.PlayOneShotSound("Effects", _gunAudioAsset.ReloadClip, transform.position, 1f, 0f, 128);      break;
            case 1: AudioManager.Instance.PlayOneShotSound("Effects", _gunAudioAsset.ReloadClipVar1, transform.position, 1f, 0f, 128);  break;
            case 2: AudioManager.Instance.PlayOneShotSound("Effects", _gunAudioAsset.FullReloadClip, transform.position, 1f, 0f, 128);  break;
        }
    }

    #region - Inventory Guns Change -
    // ----------------------------------------------------------------------
    // Name: DrawGun (Method)
    // Desc: This method draw the current weapon and playing the draw sound.
    // ----------------------------------------------------------------------
    public void DrawGun()
    {
        gameObject.SetActive(true);
        permanentHolst = false;
        AudioManager.Instance.PlayOneShotSound("Effects", _gunAudioAsset.DrawClip, transform.position, 1f, 0f, 128);
    }

    // ----------------------------------------------------------------------
    // Name: EndDraw (Animation Event)
    // Desc: This method end the draw action on the gun, activating it
    //       completly and updating the gun UI.
    // ----------------------------------------------------------------------
    public void EndDraw()
    {
        _isEquiped  = true;
        _canShoot   = true;
        UI_Update();
    }

    // ----------------------------------------------------------------------
    // Name: GunHolst (Method)
    // Desc: This method holst the gun, first deactivating it completly, also
    //       the method have an option of holst the gun permanently, without
    //       equiping the next avaliable gun later.
    // ----------------------------------------------------------------------
    public void GunHolst(bool permanent)
    {
        _animator.SetTrigger(_holstWeaponHash);
        _isEquiped  = false;
        _canShoot   = false;

        permanentHolst = permanent;

        AudioManager.Instance.PlayOneShotSound("Effects", _gunAudioAsset.HolstClip, transform.position, 1f, 0f, 128);
    }

    //Animation Event
    public void EndHolst()
    {
        gameObject.SetActive(false);
        _playerInstance.BodyController._changingWeapon = false;
        if (!permanentHolst) _playerInstance.BodyController.EquipCurrentGun();
    }
    #endregion
}