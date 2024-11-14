using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.Examples;
using UnityEngine;
using static NekraByte.Core.DataTypes;
using static NekraByte.Core.Enumerators;

[RequireComponent(typeof(Animator), typeof(InputManager), typeof(CharacterController))]
public class BodyController : MonoBehaviour, IDataPersistence
{
    //Main System Dependencies
    private InputManager                _inputManager       => GetComponent<InputManager>();
    private Animator                    _animator           => GetComponent<Animator>(); 
    private CharacterController         _controller         => GetComponent<CharacterController>();
    private PlayerManager               _playerManager      => GetComponent<PlayerManager>();
    [HideInInspector] public FPSCamera  _camController      = null;

    [SerializeField] private Animator   _shadowBodyAnimator = null;

    [Header("Player Settings")]
    [SerializeField]            private SpeedSettings _speedSettings    = new SpeedSettings();
    [SerializeField, Range(0f, 100f)]   private float _groundDrag       = 2f;
    [SerializeField, Range(-20f, 20f)]  private float _gravity          = -9.84f;
    [SerializeField, Range(0f, 10f)]    private float jumpHeight        = 3f;
    [SerializeField, Range(0f, 10f)]    private float jumpCooldown      = 3f;

    //Private Data
    private float _targetSpeed          = 7f;
    private float _targetSpeedModifier  = 0f;


    [Header("Player State")]
    public bool _isWalking          = false;
    public bool _isWalkingBackwards = false;
    public bool _isWalkingSidewards = false;
    public bool _isSprinting        = false;
    public bool _isCrouching        = false;
    public bool _isGrounded         = false;
    public bool _canCrouch          = true;
    public bool _canJump            = true;
    public bool _inAir              = false;

    [SerializeField] private MovementState _currentState = MovementState.Idle;

    private Vector3     _velocity       = Vector3.zero;
    private float       _targetX        = 0f;
    private float       _targetY        = 0f;
    private const float _animBlendSpeed = 8.9f; //Animation blend speed constant

    //Aniamtion Hashes
    private int xHash                   = Animator.StringToHash("X");
    private int yHash                   = Animator.StringToHash("Y"); 
    private int objectThrowingHash      = Animator.StringToHash("ThrowObject");
    private int objectThrowCancelHash   = Animator.StringToHash("ObjectThrowCancel");
    private int objectInstantThrow      = Animator.StringToHash("ObjectInstantThrow");
    private int armWalkHash             = Animator.StringToHash("isWalking");
    private int armRunningHash          = Animator.StringToHash("isRunning");
    private int isCrouchingHash         = Animator.StringToHash("IsCrouching");
    private int isRunningHash           = Animator.StringToHash("IsRunning");

    [Header("AI Stickness")]
    [Tooltip("When the AI agent touch the player, the player receive an speed nerf, to make the AI more lethal.")]
    [SerializeField, Range(0f, 1f)] float _npcStickiness = 0.5f;
    private float   _dragMultiplier       = 1f;
    private float   _dragMultiplierLimit  = 1f;
    public Collider playerCollider        = null;

    [Header("Ground Check")]
    [SerializeField] private Transform  _feetChecker;
    [SerializeField] private float      _floorDistance;
    [SerializeField] private LayerMask  _groundMask;

    [Header("Foot IK")]
    [SerializeField] private bool   _footIK = true;
    [SerializeField] private float  _distanceToGround = 1f;

    //Encapsulated Data
    public float TargetSpeed            { get => _targetSpeed; }
    public float dragMultiplierLimit    { get => _dragMultiplierLimit;  set => _dragMultiplierLimit = Mathf.Clamp01(value);                 }
    public float dragMultiplier         { get => _dragMultiplier;       set => _dragMultiplier = Mathf.Min(value, _dragMultiplierLimit);    }
    public Vector3 Velocity             { get => _velocity; }
    public MovementState CurrentState   { get => _currentState; }

    [Header("Gun Handler States")]
    public bool _changingWeapon     = false;
    public bool _isThrowingObject   = false;
    public bool _flashlightActive   = false;

    #region - Rock Throwing -
    [Header("Rock Throwing System")]
    [Range(1f, 20f)] public float           _objectThrowForce    = 5f;
    [Range(1f, 20f)] public float           _objectThrowUpForce  = 5f;
    public Transform                        _rockThrowPosition   = null;
    [SerializeField] private RockThrower    _rockThrower         = null;
    #endregion

    [Header("Gun System")]
    public GameObject       _shootPoint     = null;
    public GunBase          _equippedGun    = null;
    public List<GunBase>    _gunsInHand     = new List<GunBase>();

    private int _gunIndex = 0;

    [Header("Player State")]
    public BodyState _currentBodyState = new BodyState();
    [SerializeField] private List<BodyState>    _bodyStates = new List<BodyState>();

    [SerializeField] private GameObject _flashlight     = null;
    [SerializeField] private AudioClip  _flashlightClip = null;

    private void Start()
    {
        if (GameStateManager.Instance != null)
            RegisterDataSaver();

        _camController  = GameObject.FindGameObjectWithTag("CameraObject").GetComponent<FPSCamera>();
        _rockThrower    = AnimationLayer.GetAnimationLayer("RockThrowerLayer", _playerManager._animLayers).layerObject.GetComponent<RockThrower>();
    }

    private void Update()
    {
        if (CharacterManager.Instance.isDead)
            return;

        //Nulables checks, if any crucial component is null, the system will return without any execution.
        if (_playerManager                  == null)        return;
        if (_playerManager._armsAnimator    == null)        return;
        if (_playerManager.InputManager     == null)        return;
        if (GameSceneManager.Instance._gameIsPaused)        return;
        if (GameSceneManager.Instance._isInspectingItem)    return;
        if (GameSceneManager.Instance.inventoryIsOpen)      return;

        UpdateCalls();
        MovementController();
    }

    private void UpdateCalls()
    {
        _isGrounded         = Physics.CheckSphere(_feetChecker.position, _floorDistance, _groundMask);     
        _isWalking          = _inputManager.MoveAction.Vector != Vector2.zero;
        _isWalkingBackwards = _inputManager.MoveAction.Vector.y < 0;
        _isWalkingSidewards = _inputManager.MoveAction.Vector.x != 0;
        _isSprinting        = _playerManager.InputManager.LeftShiftAction.State 
            && _isWalking 
            && !_isWalkingBackwards 
            && !_isCrouching;

        StateHandler();
        InputHandler();
        AnimationController();
    }

    private void MovementController()
    {
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        Vector3 move = transform.right * _inputManager.MoveAction.Vector.x + transform.forward * _inputManager.MoveAction.Vector.y; // Movement calculation.

        _controller?.Move(move * _targetSpeed * Time.deltaTime);// Movement direction apply.

        _velocity.y += _gravity * Time.deltaTime;// Gravity calculation.

        _controller?.Move(_velocity * Time.deltaTime); //Gravity movement apply.

        _dragMultiplier = Mathf.Min(_dragMultiplier + Time.deltaTime, _dragMultiplierLimit);
    }

    private void AnimationController()
    {
        _playerManager._armsAnimator.SetBool(armWalkHash, _isWalking);
        _playerManager._armsAnimator.SetBool(armRunningHash, _isSprinting);

        _targetX = Mathf.Lerp(_targetX, _inputManager.MoveAction.Vector.x, _animBlendSpeed * Time.deltaTime);
        _targetY = Mathf.Lerp(_targetY, _isSprinting ? _inputManager.MoveAction.Vector.y * 2 : _inputManager.MoveAction.Vector.y, _animBlendSpeed * Time.deltaTime);

        _animator.SetFloat(xHash, _targetX);
        _animator.SetFloat(yHash, _targetY);

        _shadowBodyAnimator.SetFloat(xHash, _targetX);
        _shadowBodyAnimator.SetFloat(yHash, _targetY);

        _animator.SetBool(isRunningHash, _isSprinting);
        _animator.SetBool(isCrouchingHash, _isCrouching);

        _shadowBodyAnimator.SetBool(isRunningHash, _isSprinting);
        _shadowBodyAnimator.SetBool(isCrouchingHash, _isCrouching);
        _shadowBodyAnimator.speed = _speedSettings.AnimationSpeed(this);

        _animator.speed                     = _speedSettings.AnimationSpeed(this);
        _playerManager._armsAnimator.speed  = _speedSettings.AnimationSpeed(this);
    }

    private void InputHandler()
    {
        if (_inputManager.SpaceAction.Action.WasPressedThisFrame() && _canJump && _isGrounded)
        {
            _playerManager.PlayerAudioManager.JumpAudioAction();
            _inAir      = true;
            _canJump    = false;
            JumpAction();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        _inAir = !_isGrounded;

        if (_inputManager.C_Action.Action.WasPressedThisFrame())
            GunHolstingBehavior();

        if (GameStateManager.Instance != null)
        {
            if (GameStateManager.Instance.currentApplicationData.crouchType == 0) //Crouch Type -> Hold
            {
                if (_inputManager.CtrlAction.Action.WasPerformedThisFrame() && _isGrounded && !_isSprinting) 
                    _isCrouching = true;
                else if (_inputManager.CtrlAction.Action.WasReleasedThisFrame()) 
                    _isCrouching = false;
            }
            else if (GameStateManager.Instance.currentApplicationData.crouchType == 1)//Crouch Type -> Toggle
            {
                if (_inputManager.CtrlAction.Action.WasPerformedThisFrame() && _isGrounded && !_isSprinting) 
                    _isCrouching = true;
                else if (_inputManager.CtrlAction.Action.WasPerformedThisFrame() && _isCrouching) 
                    _isCrouching = false;
            }
        }
        else
        {
            if (_inputManager.CtrlAction.Action.WasPerformedThisFrame() && _isGrounded && !_isSprinting) 
                _isCrouching = true;

            if (_inputManager.CtrlAction.Action.WasReleasedThisFrame() && _isGrounded) 
                _isCrouching = false;
        }

        if (_gunsInHand.Count > 0)
        {
            if (_inputManager.One_Action.Action.WasPressedThisFrame() && !_changingWeapon)
            {
                EquipGun(0);
                CancelThrowRock();
            }

            if (_inputManager.Two_Action.Action.WasPressedThisFrame() && !_changingWeapon)
            {
                EquipGun(1);
                CancelThrowRock();
            }
        }

        if (_inputManager.T_Action.Action.WasPressedThisFrame() && !_isSprinting)
        {
            if (_equippedGun != null && _equippedGun._isReloading)
                return;
            else 
                StartThrowing();
        }

        if (_isThrowingObject)
            if (_inputManager.T_Action.Action.WasReleasedThisFrame())
                EndRockThrow();

        //if (_equippedGun != null)
        //{
        //    if (!_equippedGun._isReloading && !_isSprinting)
        //    {
        //        if (_inputManager.T_Action.Action.WasPressedThisFrame()) 
        //            StartThrowing();
        //        if (_isThrowingObject) 
        //            if (_inputManager.T_Action.Action.WasReleasedThisFrame()) 
        //                EndRockThrow();
        //    }
        //}

        if (_flashlight != null)
            if (_inputManager.F_Action.Action.WasPressedThisFrame()) ChangeFlashlightState();       
    }

    private void JumpAction() => _velocity.y = Mathf.Sqrt(jumpHeight * -2f * _gravity);

    private void ResetJump()        => _canJump = true;

    #region - Game Player Saving - 
    public void RegisterDataSaver() => GameStateManager.Instance.RegisterSavableInstance(this);

    public void Save(SaveData gameData)
    {
        Debug.Log("CM -> Saving Data"); //-> Debug Line

        gameData.GetPlayerData().GunID = _gunIndex;

        gameData.GetPlayerData()._guns.Clear();
        foreach (var gun in _gunsInHand)
            gameData.GetPlayerData()._guns.Add(gun.GunData.ammoData);
    }

    public void Load(SaveData gameData)
    {
        StartCoroutine(LoadWithTime(gameData));

        _gunIndex = gameData.GetPlayerData().GunID;

        //_controller?.Move(gameData.GetPlayerData().transformSave.Position);
        //_controller.transform.rotation = transform.rotation = gameData.GetPlayerData().transformSave.Rotation;

        EquipGun(_gunIndex);

        for (int i = 0; i < gameData.GetPlayerData()._guns.Count; i++)
        {
            if (_gunsInHand[i].Equals(null))
            {
                Debug.Log("Gun is Null!");
                continue;
            }
            _gunsInHand[i].GunData.LoadData(gameData.GetPlayerData()._guns[i]);
        }
    }

    public IEnumerator LoadWithTime(SaveData gameData)
    {
        yield return new WaitForSeconds(0.05f);

        gameObject.transform.position = gameData.GetPlayerData().transformSave.Position;
        gameObject.transform.rotation = gameData.GetPlayerData().transformSave.Rotation;
        _velocity                     = gameData.GetPlayerData().transformSave.Velocity;
    }
    #endregion

    private void StateHandler()
    {
        SetBodyState(_isCrouching ? "Crouching" : "Standing");

        if (_isGrounded && _isWalking)                      SetMovementState(MovementState.Walking);
        else if (_isCrouching)                              SetMovementState(MovementState.Crouching);
        else if (_isGrounded && _isWalking && _isSprinting) SetMovementState(MovementState.Sprinting);
        else if (!_isGrounded)                              SetMovementState(MovementState.Air);
        else                                                SetMovementState(MovementState.Idle);

        _targetSpeed = _speedSettings.GetFinalSpeed(this);
    }

    private void SetBodyState(string StateName)
    {
        foreach(var state in _bodyStates)
        {
            if (state.Name == StateName)
            {
                _currentBodyState = state;
                break;
            }
            else _currentBodyState = _bodyStates[0];
        }
    }

    private void SetMovementState(MovementState state)
    {
        _currentBodyState.MovementState = state;
        _currentState                   = state;
    }

    #region - Gun Managing -
    private void EquipGun(int gunToEquip)
    {
        if (_gunsInHand.Count <= 0)         return;
        if (_gunsInHand[0].Equals(null))    return;

        _gunIndex = gunToEquip;

        if (_gunsInHand[_gunIndex].gameObject.activeInHierarchy ||
            _equippedGun._isReloading ||
            _changingWeapon) return;

        _changingWeapon = true;

        if (_gunIndex == 0) _gunsInHand[1].GunHolst(false);//Selecting the gun and holsting it
        else _gunsInHand[0].GunHolst(false);

        _equippedGun = _gunsInHand[_gunIndex];
    }

    private void GunHolstingBehavior()
    {
        if (!_equippedGun._isEquiped) _equippedGun.DrawGun();
        else _equippedGun.GunHolst(true);
    }

    public void GunPermanentHolst() => _equippedGun.GunHolst(true);

    public void EquipCurrentGun()   => _equippedGun.DrawGun();
    #endregion

    public void DoStickiness()      => _dragMultiplier = 1f - _npcStickiness;

    #region - Flashlight System -
    private void ChangeFlashlightState()
    {
        _flashlight.SetActive(!_flashlight.activeInHierarchy);
        SS_Flashlight();
    }

    private void SS_Flashlight()
    {
        if (!_flashlightClip.Equals(null))
            AudioManager.Instance.PlayOneShotSound("Effects", _flashlightClip, transform.position, 1f, 0, 128);
    }
    #endregion

    #region - Throw Rock State -
    private void StartThrowing()
    {
        if (_rockThrower        == null) return;
        if (_rockThrowPosition  == null) return;
        if (_equippedGun        != null) _equippedGun.GunHolst(true);

        _isThrowingObject = true;
        _rockThrower.ThrowRock();
    }

    private void CancelThrowRock()
    {
        _rockThrower.CancelThrowing();
        _isThrowingObject = false;
    }

    private void EndRockThrow() => _rockThrower.FinishThrow();
    #endregion

    #region - Foot IK -
    protected virtual void OnAnimatorIK(int layerIndex)
    {
        if (_animator == null) return;

        if (_footIK)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

            RaycastHit hit;
            Ray ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftFoot), Vector3.down);
            if (Physics.Raycast(ray, out hit, _distanceToGround + 1f, _groundMask))
            {
                Vector3 footPosition = hit.point;
                footPosition.y += _distanceToGround;
                _animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                _animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
            }

            ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.RightFoot), Vector3.down);
            if (Physics.Raycast(ray, out hit, _distanceToGround + 1f, _groundMask))
            {
                Vector3 footPosition = hit.point;
                footPosition.y += _distanceToGround;
                _animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, Quaternion.LookRotation(transform.forward, hit.normal));
            }
        }
    }
    #endregion
}

[Serializable]
public class SpeedSettings
{
    [Header("Base Speeds")]
    [Range(0.1f, 10f)] public float BaseSpeed    = 4f;
    [Range(0.1f, 10f)] public float SprintSpeed  = 6f;
    [Range(0.1f, 10f)] public float CrouchSpeed  = 2f;

    [Header("Speed Modifiers")]
    [Range(0.1f, 1f)] public float BackwardModifier     = 4f;
    [Range(0.1f, 1f)] public float SidewardModifier     = 4f;

    [Header("Animation Modifiers")]
    [Range(0.1f, 1f)] public float WalkModifier   = 4f;
    [Range(0.1f, 1f)] public float CrouchModifier = 4f;

    [SerializeField] private float _targetSpeed         = 0f;
    [SerializeField] private float _targetModifier      = 0f;
    [SerializeField] private float _targetAnimModifier  = 0f;

    public float GetFinalSpeed(BodyController controller)
    {
        _targetSpeed        = controller._isSprinting           ? SprintSpeed       : controller._isCrouching           ? CrouchSpeed       : BaseSpeed;
        _targetModifier     = controller._isWalkingBackwards    ? BackwardModifier  : controller._isWalkingSidewards    ? SidewardModifier  : 1;
        _targetAnimModifier = controller._isCrouching           ? CrouchModifier    : WalkModifier;

        return _targetSpeed * _targetModifier;
    }

    public float AnimationSpeed(BodyController controller) => 
        _targetAnimModifier = controller._isCrouching ? CrouchModifier : WalkModifier;
}