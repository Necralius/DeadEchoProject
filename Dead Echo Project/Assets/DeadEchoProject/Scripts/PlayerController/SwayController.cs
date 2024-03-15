using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static NekraByte.Core.DataTypes;
using static NekraByte.Core.Procedural;

public class SwayController : MonoBehaviour
{
    private PlayerManager   _playerInstance     = null;
    private InputManager    _inptManager        = null;

    private GameObject      _swayObject         = null;

    [SerializeField] private SwayData _swayData         = new SwayData();

    #region - Input Sway -
    private Vector3 newWeaponRotation;
    private Vector3 newWeaponRotationVelocity;

    private Vector3 targetWeaponRotation;
    private Vector3 targetWeaponRotationVelocity;
    #endregion

    #region - Movement Sway -
    private Vector3 newWeaponMovementRotation;
    private Vector3 newWeaponMovementRotationVelocity;

    private Vector3 targetWeaponMovementRotation;
    private Vector3 targetWeaponMovementRotationVelocity;
    #endregion

    private bool    _isAiming       = false;
    private Vector3 _swayPosition   = Vector3.zero;

    Vector2 _look = Vector2.zero;
    Vector2 _move = Vector2.zero;

    private void Start()
    {
        _playerInstance     = GetComponent<PlayerManager>();
        _inptManager        = _playerInstance.InputManager;
        _swayObject         = AnimationLayer.GetAnimationLayer("SwayLayer", _playerInstance._animLayers).layerObject;

        newWeaponRotation   = _swayObject.transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        _look = _inptManager.Look;
        _move = _inptManager.Move;

        _isAiming = _playerInstance.BodyController._equippedGun._isAiming;

        SwayHandler();
    }

    private void SwayHandler()
    {
        if (_swayData._inputSway)
        {

            float xValue = _isAiming ? _swayData.inpt_amountX * _swayData.move_AimEffector : _swayData.inpt_amountX;
            float yValue = _isAiming ? _swayData.inpt_amountY * _swayData.move_AimEffector : _swayData.inpt_amountY;

            targetWeaponRotation.y  -= xValue * _look.x * Time.deltaTime;
            targetWeaponRotation.x  += yValue * _look.y * Time.deltaTime;

            targetWeaponRotation.x  = Mathf.Clamp(targetWeaponRotation.x, -_swayData.inpt_clampX, _swayData.inpt_clampX);
            targetWeaponRotation.y  = Mathf.Clamp(targetWeaponRotation.y, -_swayData.inpt_clampY, _swayData.inpt_clampY);
            targetWeaponRotation.z  = targetWeaponRotation.y;

            targetWeaponRotation    = Vector3.SmoothDamp(targetWeaponRotation, Vector3.zero, ref targetWeaponRotationVelocity, _swayData.inpt_swayResetSmooth);
            newWeaponRotation       = Vector3.SmoothDamp(newWeaponRotation, targetWeaponRotation, ref newWeaponRotationVelocity, _swayData.inpt_smoothAmount);
        }

        if (_swayData._movementSway)
        {
            float xValue = _isAiming ? _swayData.move_amountX * _swayData.move_AimEffector : _swayData.move_amountX;
            float yValue = _isAiming ? _swayData.move_amountY * _swayData.move_AimEffector : _swayData.move_amountY;

            targetWeaponMovementRotation.z  = xValue * _move.x;
            targetWeaponMovementRotation.x  = yValue * -_move.y;

            targetWeaponMovementRotation    = Vector3.SmoothDamp(targetWeaponMovementRotation, Vector3.zero, ref targetWeaponMovementRotationVelocity, _swayData.inpt_swayResetSmooth);
            newWeaponMovementRotation       = Vector3.SmoothDamp(newWeaponMovementRotation, targetWeaponMovementRotation, ref newWeaponMovementRotationVelocity, _swayData.move_SmoothAmount);
        }

        if (_swayData._idleSway)
        {
            float amountA       = _isAiming ? _swayData.swayAmountA * _swayData.idle_AimEffector : _swayData.swayAmountA;
            float amountB       = _isAiming ? _swayData.swayAmountB * _swayData.idle_AimEffector : _swayData.swayAmountB;

            float swayScale     = _isAiming ? _swayData.swayScale * (_swayData.idle_AimEffector * 1000) : _swayData.swayScale;

            var targetPosition  = LissajousCurve(_swayData.swayTime, amountA, amountB) / swayScale;
            
            _swayPosition       = Vector3.Lerp(_swayPosition, targetPosition, Time.smoothDeltaTime * _swayData.swayLerpSpeed);
            _swayData.swayTime  += Time.deltaTime;

            if (_swayData.swayTime > 6.3f) _swayData.swayTime = 0f;

            _swayObject.transform.localPosition = _swayPosition;
        }

        _swayObject.transform.localRotation = Quaternion.Euler(newWeaponRotation + newWeaponMovementRotation);
    }

    private Vector3 LissajousCurve(float Time, float A, float B) => new Vector3(Mathf.Sin(Time), A * Mathf.Sin(B * Time + Mathf.PI));
}