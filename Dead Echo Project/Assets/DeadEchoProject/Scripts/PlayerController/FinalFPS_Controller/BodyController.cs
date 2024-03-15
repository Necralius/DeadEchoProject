using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NekraByte.Core.DataTypes;
using static NekraByte.Core.Enumerators;

[RequireComponent(typeof(Animator), typeof(InputManager), typeof(CharacterController))]
public class BodyController : MonoBehaviour, IDataPersistence
{
    //Main System Dependencies
    private InputManager        _inputManager   => GetComponent<InputManager>();
    private Animator            _animator       => GetComponent<Animator>(); 
    private CharacterController _controller     => GetComponent<CharacterController>();
    private PlayerManager       _playerManager  => GetComponent<PlayerManager>();
    private FPSCamera           _camController  = null;

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

    public MovementState _currentState = MovementState.Walking;

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

    [Header("AI Stickness")]
    [Tooltip("When the AI agent touch the player, the player receive an speed nerf, to make the AI more lethal.")]
    [SerializeField, Range(0f, 1f)] float _npcStickiness = 0.5f;
    float _dragMultiplier       = 1f;
    float _dragMultiplierLimit  = 1f;

    //Encapsulated Data
    public float dragMultiplierLimit    { get => _dragMultiplierLimit;  set => _dragMultiplierLimit = Mathf.Clamp01(value);                 }
    public float dragMultiplier         { get => _dragMultiplier;       set => _dragMultiplier = Mathf.Min(value, _dragMultiplierLimit);    }
    public Vector3 Velocity             { get => _velocity; }

    [Header("Gun Handler States")]
    public bool _changingWeapon     = false;
    public bool _isThrowingObject   = false;
    public bool _flashlightActive   = false;

    [Header("Rock Throwing System")]
    [SerializeField, Range(1f, 20f)] private float  _objectThrowForce    = 5f;
    [SerializeField, Range(1f, 20f)] private float  _objectThrowUpForce  = 5f;
    [SerializeField] private Transform              _rockThrowPosition   = null;

    [Header("Gun System")]
    public GameObject       _shootPoint     = null;
    public GunBase          _equippedGun    = null;
    public List<GunBase>    _gunsInHand     = new List<GunBase>();

    private int _gunIndex = 0;

    [Header("Player State")]
    public BodyState _currentBodyState = new BodyState();
    [SerializeField] private List<BodyState>    _bodyStates = new List<BodyState>();

    private void Start()
    {
        if (GameStateManager.Instance != null) RegisterDataSaver();
        _camController = GameObject.FindGameObjectWithTag("CameraObject").GetComponent<FPSCamera>();
    }

    private void Update()
    {
        //Nulables checks, if any crucial component is null, the system will return without any execution.
        if (_playerManager                  == null) return;
        if (_playerManager._armsAnimator    == null) return;
        if (_playerManager.InputManager     == null) return;
        if (GameSceneManager.Instance._gameIsPaused) return;

        UpdateCalls();
        MovementController();
    }

    private void UpdateCalls()
    {
        _isGrounded         = _controller.isGrounded;
        _isWalking          = _inputManager.Move != Vector2.zero;
        _isSprinting        = _playerManager.InputManager.sprint && _isWalking && !_isWalkingBackwards && !_isCrouching;
        _isWalkingBackwards = _inputManager.Move.y < 0;
        _isWalkingSidewards = _inputManager.Move.x != 0;

        _playerManager._armsAnimator.SetBool(armWalkHash,       _isWalking);
        _playerManager._armsAnimator.SetBool(armRunningHash,    _isSprinting);

        StateHandler();
        InputHandler();
        AnimationController();
    }

    private void MovementController()
    {
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        Vector3 move = transform.right * _inputManager.Move.x + transform.forward * _inputManager.Move.y; // Movement calculation.

        _controller?.Move(move * _targetSpeed * Time.deltaTime);// Movement direction apply.

        _velocity.y += _gravity * Time.deltaTime;// Gravity calculation.

        _controller?.Move(_velocity * Time.deltaTime); //Gravity movement apply.
    }

    private void AnimationController()
    {
        _targetX = Mathf.Lerp(_targetX, _inputManager.Move.x, _animBlendSpeed * Time.deltaTime);
        _targetY = Mathf.Lerp(_targetY, _isSprinting ? _inputManager.Move.y * 2 : _inputManager.Move.y, _animBlendSpeed * Time.deltaTime);

        _animator.SetFloat(xHash, _targetX);
        _animator.SetFloat(yHash, _targetY);
    }

    private void InputHandler()
    {
        if (GameSceneManager.Instance._inventoryIsOpen) return;

        if (_inputManager.jumpAction.WasPressedThisFrame() && _canJump && _isGrounded)
        {
            _playerManager.PlayerAudioManager.JumpAudioAction();
            _inAir      = true;
            _canJump    = false;
            JumpAction();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        _inAir = !_isGrounded;

        if (_inputManager.E_Action.WasPressedThisFrame())
            _playerManager.InteractionController.InteractWith();

        if (_inputManager.permaHolstAction.WasPressedThisFrame())
            GunHolstingBehavior();

        if (GameStateManager.Instance != null)
        {
            if (GameStateManager.Instance.currentApplicationData.crouchType == 0)//Crouch Type -> Hold
            {
                if (_inputManager.crouchAction.WasPerformedThisFrame() && _isGrounded && !_isSprinting) 
                    _isCrouching = true;
                else if (_inputManager.crouchAction.WasReleasedThisFrame()) 
                    _isCrouching = false;
            }
            else if (GameStateManager.Instance.currentApplicationData.crouchType == 1)//Crouch Type -> Toggle
            {
                if (_inputManager.crouchAction.WasPerformedThisFrame() && _isGrounded && !_isSprinting) 
                    _isCrouching = true;
                else if (_inputManager.crouchAction.WasPerformedThisFrame() && _isCrouching) 
                    _isCrouching = false;
            }
        }
        else
        {
            if (_inputManager.crouchAction.WasPerformedThisFrame() && _isGrounded && !_isSprinting) 
                _isCrouching = true;

            if (_inputManager.crouchAction.WasReleasedThisFrame() && _isGrounded) 
                _isCrouching = false;
        }

        if (_gunsInHand.Count > 0)
        {
            if (_inputManager.primaryGun.WasPressedThisFrame()      && !_changingWeapon) EquipGun(0);
            if (_inputManager.secondaryGun.WasPressedThisFrame()    && !_changingWeapon) EquipGun(1);
        }
        _animator.SetBool(isCrouchingHash, _isCrouching);
    }

    private void JumpAction() // Executes an calculation of an jump height value, and set is to the new Y direction vector considering the gravity.
    {
        _velocity.y = Mathf.Sqrt(jumpHeight * -2f * _gravity);
    }

    private void ResetJump()
    {
        _canJump = true;
    }

    public void RegisterDataSaver()
    {
        GameStateManager.Instance.RegisterSavableInstance(this);
    }

    public void Save(SaveData gameData)
    {
        Debug.Log("CM -> Saving Data"); //-> Debug Line

        //gameData.GetPlayerData().transformSave = new(gameObject.transform);

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
        //_rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(0.05f);

        gameObject.transform.position = gameData.GetPlayerData().transformSave.Position;
        gameObject.transform.rotation = gameData.GetPlayerData().transformSave.Rotation;
        _velocity                     = gameData.GetPlayerData().transformSave.Velocity;
    }

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
    private void StateHandler()
    {
        SetBodyState(_isCrouching ? "Crouching" : "Standing");

        if (_isCrouching)                                   SetMovementState(MovementState.Crouching);
        else if (_isGrounded && _isWalking && _isSprinting) SetMovementState(MovementState.Sprinting);
        else if (_isGrounded && _isWalking)                 SetMovementState(MovementState.Walking);
        else                                                SetMovementState(MovementState.Air);

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
        if (!(_currentBodyState is null)) 
            _currentBodyState.MovementState = state;
    }

    private void GunHolstingBehavior()
    {
        if (!_equippedGun._isEquiped) _equippedGun.DrawGun();
        else _equippedGun.GunHolst(true);
    }

    public void GunPermanentHolst() => _equippedGun.GunHolst(true);

    public void EquipCurrentGun()   => _equippedGun.DrawGun();

    public void DoStickiness()      => _dragMultiplier = 1f - _npcStickiness;
}

[Serializable]
public class SpeedSettings
{
    [Range(1f, 30f)] public float BaseSpeed    = 4f;
    [Range(1f, 30f)] public float SprintSpeed  = 6f;
    [Range(1f, 30f)] public float CrouchSpeed  = 2f;

    [Range(0.1f, 1f)] public float BackwardModifier     = 4f;
    [Range(0.1f, 1f)] public float SidewardModifier     = 4f;

    public float GetFinalSpeed(BodyController controller)
    {
        float targetSpeed;
        float targetModifier;

        targetSpeed     = controller._isSprinting ? SprintSpeed : controller._isCrouching ? CrouchSpeed : BaseSpeed;
        targetModifier  = controller._isWalkingBackwards ? BackwardModifier : controller._isWalkingSidewards ? SidewardModifier : 1;

        return targetSpeed * targetModifier;
    }
}