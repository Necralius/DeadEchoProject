using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static NekraByte.Core.Enumerators;
using static NekraByte.Core.DataTypes;
using static NekraByte.Core.DataTypes.IDataPersistence;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody))]
public class ControllerManager : MonoBehaviour, IDataPersistence
{

    #region - Singleton Pattern -
    public static ControllerManager Instance { get; private set; }
    #endregion

    #region - Movment Settings -
    [Header("Camera Look")]
    [SerializeField, Range(1, 40)] private float    _sensX          = 10f;
    [SerializeField, Range(1, 40)] private float    _sensY          = 10f;

    [SerializeField] private Transform      _orientation    = null;
    public                   Transform      _cameraObject   = null;
    private                  float          _xRotation      = 0;
    private                  float          _yRotation      = 0;

    [Header("Player Movment")]
    [SerializeField, Range(1f, 100f)] private float     _walkSpeed      = 7f;
    [SerializeField, Range(1f, 100f)] private float     _sprintSpeed    = 10f;
    [SerializeField, Range(0f, 20f)]  private float     _crouchSpeed    = 4f;
    [SerializeField, Range(0f, 100f)] private float     _groundDrag     = 2f;

    [Header("Crouching")]
    [SerializeField] private    float _crouchYScale    = 1.2f;
    private                     float _startYScale     = 0f;

    private Vector3     _moveDirection              = Vector3.zero;
    private float       _targetSpeed                = 7f;

    [Header("Jump Settings")]
    [SerializeField, Range(1f, 20f)] private float jumpForce        = 12f;
    [SerializeField, Range(0f, 20f)] private float jumpCooldown     = 0.25f;
    [SerializeField, Range(0f, 20f)] private float airMultiplier    = 0.4f;

    [Header("Ground Check")]
    private                     float              _playerHeight   = 2f;
    [HideInInspector] public    CapsuleCollider    _playerCol      = null;
    [SerializeField]  private   LayerMask          _groundMask;

    [Header("Slope Handling")]
    public  float       _maxSlopeAngle = 40f;
    private RaycastHit  _slopeHit;

    [Header("Sliding System")]
    [SerializeField] private float  _maxSlideTime   = 0f;
    [SerializeField] private float  _slideForce     = 0f;
    private float                   _slideTimer     = 0f;

    [SerializeField] private float  _slideYScale    = 0.8f;
    #endregion

    #region - Player State -
    [Header("Player State")]
    public bool             _canMove            = true;
    public bool             _isWalking          = false;
    public bool             _isSprinting        = false;
    public bool             _isCrouching        = false;
    public bool             _sliding            = false;

    public MovementState    _currentState       = MovementState.Walking;

    [Header("Movement Limiters")]
    public bool     _canCrouch          = false;
    public bool     _duringCrouch       = false;
    public bool     _isMoving           = false;
    public bool     _isGrounded         = true;
    public bool     _inAir              = false;
    public bool     _canJump            = true;
    public bool     _onSlope            = false;
    private bool    _walkingBackwards   = false;
    private bool    _exitingSlope       = false;

    [Header("Gun Handler States")]
    public bool _changingWeapon     = false;
    public bool _isThrowingObject   = false;
    public bool _flashlightActive   = false;

    #endregion

    #region - Gun System -
    [Header("Gun System Dependencies")]
    public Animator     _armsAnimator   = null;
    public GameObject   _shootPoint     = null;

    [Header("Gun System")]
    public GunBase _equippedGun;
    public List<GunBase> _allGuns    = new List<GunBase>();
    #endregion

    #region - Animation Hashes -
    private int objectThrowingHash      = Animator.StringToHash("ThrowObject");
    private int objectThrowCancelHash   = Animator.StringToHash("ObjectThrowCancel");
    private int objectInstantThrow      = Animator.StringToHash("ObjectInstantThrow");
    private int armWalkHash             = Animator.StringToHash("isWalking");
    private int armRunningHash          = Animator.StringToHash("isRunning");
    #endregion

    #region - Rock Throwing System -
    [Header("Rock Throwing Forces")]
    public float _objectThrowForce = 5f;
    public float _objectThrowUpForce = 5f;
    public Transform _rockThrowPosition = null;
    [SerializeField] private RockThrower _rockThrower;
    #endregion

    #region - Flash Light System -
    [Header("Flashlight System")]
    public GameObject _flashLightObject;
    #endregion

    #region - Dependencies -
    //Dependencies
    [HideInInspector] public InputManager           _inptManager    = null;
    [HideInInspector] public InteractionController  _interactionController = null;
    [HideInInspector] public DynamicUI_Manager      _dynamicUI_Manager = null;
    [HideInInspector] public Rigidbody              _rb             = null;

    [Header("Player Instance")]
    [SerializeField] private GameObject         _playerInstance = null;

    private                  PlayerAudioManager _playerAudioManager = null;
    #endregion

    #region - Audio System -
    [Header("Gun Public Sounds")]
    public AudioClip gunShootJam;
    public AudioClip changeGunMode;
    public AudioClip flashlightSound;
    #endregion

    #region - Drag System -
    [Header("Drag Multiplier")]
    float _dragMultiplier = 1f;
    float _dragMultiplierLimit = 1f;
    [SerializeField, Range(0f, 1f)] float _npcStickiness = 0.5f;

    public float dragMultiplierLimit { get => _dragMultiplierLimit; set => _dragMultiplierLimit = Mathf.Clamp01(value); }
    public float dragMultiplier { get => _dragMultiplier; set => _dragMultiplier = Mathf.Min(value, _dragMultiplierLimit); }
    #endregion

    public List<AnimationLayer> _animLayers;

    #region - Gun Change System -
    private int _gunIndex = 0;
    #endregion

    #region - UI Elements -
    [Header("UI Elements")]
    public TextMeshProUGUI txt_magAmmo;
    public TextMeshProUGUI txt_bagAmmo;
    public TextMeshProUGUI txt_GunName;
    #endregion

    public bool isDebugMode = true;

    // ---------------------------- Methods ----------------------------//

    #region - BuiltIn Methods -
    // ----------------------------------------------------------------------
    // Name: Awake (Method)
    // Desc: This method is called on the very first frame of the scene,
    //       mainly the method get all the class dependencies, declares some
    //       values and declares the singleton value instance of this class.
    // ----------------------------------------------------------------------
    private void Awake()
    {
        if (Instance != null) Destroy(Instance);
        Instance = this;

        AnimationLayer[] layers     = _playerInstance.GetComponentsInChildren<AnimationLayer>();
        _animLayers                 = layers.ToList();

        _rb                     = GetComponent<Rigidbody>();
        _playerCol              = GetComponentInChildren<CapsuleCollider>();
        _playerAudioManager     = GetComponent<PlayerAudioManager>();
        _inptManager            = GetComponent<InputManager>();
        _interactionController  = GetComponent<InteractionController>();

        _playerHeight       = _playerCol.height;
        
        _armsAnimator       = AnimationLayer.GetAnimationLayer("AnimationsLayer", _animLayers).animator;
        _rockThrower        = AnimationLayer.GetAnimationLayer("RockThrowerLayer", _animLayers).layerObject.GetComponent<RockThrower>();
        _dynamicUI_Manager  = AnimationLayer.GetAnimationLayer("PlayerCanvas", _animLayers).layerObject.GetComponent<DynamicUI_Manager>();

        _startYScale        = transform.localScale.y;
    }

    // ----------------------------------------------------------------------
    // Name: Start (Method)
    // Desc: This method is called on the game start, mainly the method get
    //       some values, updates the player state, and register the data
    //       saver object.
    // ----------------------------------------------------------------------
    private void Start()
    {
        //InGame_UIManager.Instance.UpdatePlayerState(this);
        Cursor.lockState    = CursorLockMode.Locked;


        if (GameStateManager.Instance != null) RegisterDataSaver();
    }

    // ----------------------------------------------------------------------
    // Name: Update (Method)
    // Desc: This method is called every frame, mainly the method handle the
    //       input actions, the movment complete system, the flashlight and
    //       the object throwing system.
    // ----------------------------------------------------------------------
    private void Update()
    {
        if (CharacterManager.Instance.isDead)
            return;

        if (_orientation == null) return;
        if (_inptManager == null) return;
        if (_rb          == null) return;
        if (GameSceneManager.Instance._gameIsPaused)
            return;

        CameraHandler();
        UpdateCalls();
    }

    // ----------------------------------------------------------------------
    // Name: FixedUpdate (Method)
    // Desc: This method is called an certain times on an frame, mainly the
    //       method handle the physics system, like the movement system and
    //       any physics update.
    // ----------------------------------------------------------------------
    private void FixedUpdate()
    {
        if (_orientation == null) return;
        if (_inptManager == null) return;
        if (_rb          == null) return;
        if (GameSceneManager.Instance._gameIsPaused)
            return;

        MovementHandler();

        if (_sliding) 
            SlidingMovement();
    }
    #endregion

    #region - Update Actions -
    // ----------------------------------------------------------------------
    // Name : UpdateCalls (Method)
    // Desc : This method manage all data tha need to be updated
    //        every frame call.
    // ----------------------------------------------------------------------
    private void UpdateCalls()
    {
        _isMoving       = _rb.linearVelocity != Vector3.zero;
        _isSprinting    = _inptManager.LeftShiftAction.State && _isMoving && !_walkingBackwards && !_isCrouching;
        _isWalking      = _inptManager.MoveAction.Vector != Vector2.zero;
        _isGrounded     = Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.3f, _groundMask);
        _onSlope        = OnSlope();
        _inAir          = !_isGrounded;

        if (_isGrounded) _rb.linearDamping = _groundDrag;
        else _rb.linearDamping = 0f;

        //ReticleManager.Instance.DataReceiver(this);

        _armsAnimator.SetBool(armWalkHash, _isWalking);
        _armsAnimator.SetBool(armRunningHash, _isSprinting);

        SpeedLimit();
        InputHandler();
        StateHandler();
    }

    // ----------------------------------------------------------------------
    // Name: InputHandler
    // Desc: This method handle some extra input actions that are not handle
    //       on the input manager.
    // ----------------------------------------------------------------------
    private void InputHandler()
    {
        if (GameSceneManager.Instance._gameIsPaused || GameSceneManager.Instance.inventoryIsOpen) return;

        if (_inptManager.SpaceAction.Action.WasPressedThisFrame() && _canJump && _isGrounded)
        {
            _playerAudioManager.JumpAudioAction();
            _inAir = true;
            _canJump = false;
            JumpHandler();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (GameStateManager.Instance != null)
        {
            if (GameStateManager.Instance.currentApplicationData.crouchType == 0)//Crouch Type -> Hold
            {
                if (_inptManager.CtrlAction.Action.WasPerformedThisFrame() && _isGrounded && !_isSprinting)
                {
                    transform.localScale = new Vector3(transform.localScale.x, _crouchYScale, transform.localScale.z);
                    _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
                    _isCrouching = true;
                }

                if (_inptManager.CtrlAction.Action.WasReleasedThisFrame() && _isGrounded)
                {
                    transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
                    _isCrouching = false;
                }
            }
            else if (GameStateManager.Instance.currentApplicationData.crouchType == 1)//Crouch Type -> Toggle
            {
                if (_inptManager.CtrlAction.Action.WasPerformedThisFrame() && _isGrounded && !_isSprinting )
                {
                    transform.localScale = new Vector3(transform.localScale.x, _crouchYScale, transform.localScale.z);
                    _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
                    _isCrouching = true;
                }
                else if (_inptManager.CtrlAction.Action.WasPerformedThisFrame() && _isCrouching)
                {
                    transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
                    _isCrouching = false;
                }
            }
        }
        else
        {
            if (_inptManager.CtrlAction.Action.WasPerformedThisFrame() && _isGrounded && !_isSprinting)
            {
                transform.localScale = new Vector3(transform.localScale.x, _crouchYScale, transform.localScale.z);
                _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
                _isCrouching = true;
            }

            if (_inptManager.CtrlAction.Action.WasReleasedThisFrame() && _isGrounded)
            {
                transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
                _isCrouching = false;
            }
        }

        if (_isSprinting && _inptManager.CtrlAction.Action.WasPerformedThisFrame() && _isGrounded) 
            StartSlide();

        //if (_gunsInHand.Count > 0)
        //{
        //    if (_inptManager.One_Action.Action.WasPressedThisFrame()   && !_changingWeapon) EquipGun(0);
        //    if (_inptManager.Two_Action.Action.WasPressedThisFrame() && !_changingWeapon) EquipGun(1);
        //}

        if (_equippedGun != null)
        {
            if (!_equippedGun._isReloading && !_isSprinting)
            {
                if (_inptManager.T_Action.Action.WasPressedThisFrame()) StartThrowing();
                if (_isThrowingObject)
                {
                    if (_inptManager.T_Action.Action.WasReleasedThisFrame()) EndRockThrow();

                    if (_inptManager.One_Action.Action.WasPressedThisFrame() || _inptManager.Two_Action.Action.WasPressedThisFrame())
                    {
                        if (!_changingWeapon)
                        {
                            if (_inptManager.One_Action.Action.WasPressedThisFrame())
                            {
                                CancelThrowRock();
                                EquipGun(0);
                            }
                            else if (_inptManager.Two_Action.Action.WasPressedThisFrame())
                            {
                                CancelThrowRock();
                                EquipGun(1);
                            }

                        }
                    }                   
                }
            }
        }
        if (_flashLightObject != null)
            if (_inptManager.F_Action.Action.WasPressedThisFrame()) ChangeFlashlightState();

        _dragMultiplier = Mathf.Min(_dragMultiplier + Time.deltaTime, _dragMultiplierLimit);
    }
    #endregion  

    #region - Camera Movement Behavior -
    // ----------------------------------------------------------------------
    // Name: CameraHandler
    // Desc: This method handle the camera look system, using the mouse input
    //       delta vector, and multipling it by an sesistivity value in each
    //       vector value, also, the method clamp the vertical rotation,
    //       later applying the values on the camera object and player body.
    // ----------------------------------------------------------------------
    void CameraHandler()
    {
        float mouseX;
        float mouseY;

        if (GameStateManager.Instance != null)
        {
            mouseX = (GameStateManager.Instance.currentApplicationData.invertX ? -_inptManager.LookAction.Vector.x : _inptManager.LookAction.Vector.x) * Time.deltaTime * _sensX;
            mouseY = (GameStateManager.Instance.currentApplicationData.invertY ? -_inptManager.LookAction.Vector.y : _inptManager.LookAction.Vector.y) * Time.deltaTime * _sensY;
        }
        else
        {
            mouseX = _inptManager.LookAction.Vector.x * Time.deltaTime * _sensX;
            mouseY = _inptManager.LookAction.Vector.y * Time.deltaTime * _sensY;
        }

        _yRotation  += mouseX;

        _xRotation  -= mouseY;
        _xRotation  = Mathf.Clamp(_xRotation, -90f, 90f);

        _cameraObject.transform.rotation    = Quaternion.Euler(_xRotation, _yRotation, 0);
        _orientation.rotation               = Quaternion.Euler(0, _yRotation, 0);
    }
    #endregion

    #region - General Movement Behavior -
    // ----------------------------------------------------------------------
    // Name: MovementHandler
    // Desc: This method handle the movment forces using an direction vector
    //       and applying this as an force in the rigidbody.
    // ----------------------------------------------------------------------
    void MovementHandler()
    {
        _moveDirection = _orientation.forward * _inptManager.MoveAction.Vector.y + _orientation.right * _inptManager.MoveAction.Vector.x;

        if (_onSlope && !_exitingSlope)
        {
            _rb.AddForce(GetSlopeMoveDirection() * _targetSpeed * 10f, ForceMode.Force);

            if (_rb.linearVelocity.y > 0)
                _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
            
        _rb.useGravity = !_onSlope;

        if (_isGrounded)
            _rb.AddForce(_moveDirection.normalized * _targetSpeed * 10f * _dragMultiplier, ForceMode.Force);
        else if (!_isGrounded)
            _rb.AddForce(_moveDirection.normalized * _targetSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    #region- Slope Movment -
    // ----------------------------------------------------------------------
    // Name: OnSlope
    // Desc: This method verifies if the player is steping in an slope plane,
    //       basically the method produces an angle between the up angle and
    //       the raycast hit normal angle, later comparing it to the max
    //       slope angle set on the inspector variable, and finally, the
    //       method returns the result.
    // ----------------------------------------------------------------------
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < _maxSlopeAngle && angle != 0;
        }
        return false;
    }

    // ----------------------------------------------------------------------
    // Name: GetSlopeMoveDirection
    // Desc: As the name sugest, this method find the current character
    //       movment direction, based on an plane projection, later, the
    //       method returns it normalized.
    // ----------------------------------------------------------------------
    private Vector3 GetSlopeMoveDirection() => Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
    #endregion

    // ----------------------------------------------------------------------
    // Name: SpeedLimit
    // Desc: This method limit the player speed on an maximum value, the
    //       method takes the current player velocity magnitude and compares
    //       it to an target speed, if this velicity magnitude is greater
    // ----------------------------------------------------------------------
    private void SpeedLimit()
    {
        if (_onSlope && !_exitingSlope)
        {
            if (_rb.linearVelocity.magnitude > _targetSpeed) 
                _rb.linearVelocity = _rb.linearVelocity.normalized * _targetSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

            if (flatVel.magnitude > _targetSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * _targetSpeed;
                _rb.linearVelocity = new Vector3(limitedVel.x, _rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    public void DoStickiness() => _dragMultiplier = 1f - _npcStickiness;
    #endregion

    #region - Jump Behavior -
    // ----------------------------------------------------------------------
    // Name: JumpHandler
    // Desc: This method handle the jump system, basically the method reset
    //       the player velocty on Y axis, later the method set an impulse
    //       force on the up direction, simulation an jump impulse.
    // ----------------------------------------------------------------------
    private void JumpHandler()
    {
        _canJump        = false;
        _exitingSlope   = true;

        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // ----------------------------------------------------------------------
    // Name: ResetJump
    // Desc: This method only reset the jump functionality.
    // ----------------------------------------------------------------------
    private void ResetJump()
    {
        _canJump = true;
        _exitingSlope = false;
    }
    #endregion

    #region - State Handler -
    // ----------------------------------------------------------------------
    // Name: StateHandler
    // Desc: This method mainly handle the current player movment state.
    // ----------------------------------------------------------------------
    private void StateHandler()
    {
        if (_isCrouching)
        {
            _currentState   = MovementState.Crouching;
            _targetSpeed    = _crouchSpeed;
            
        }
        if (_isGrounded && _inptManager.LeftShiftAction.State)
        {
            _currentState   = MovementState.Sprinting;
            _targetSpeed    = _sprintSpeed;
        }
        else if (_isGrounded)
        {
            _currentState   = MovementState.Walking;
            _targetSpeed    = _walkSpeed;
        }
        else _currentState  = MovementState.Air;
    }
    #endregion

    #region - Sliding Handler -
    // ----------------------------------------------------------------------
    // Name: StartSlide
    // Desc: This method handle the slide start action, modifying the player
    //       scale, and adding an down force to prevent the character from
    //       being suspended in the air.
    // ----------------------------------------------------------------------
    private void StartSlide()
    {
        _sliding = true;
     
        transform.localScale = new Vector3(transform.localScale.x, _slideYScale, transform.localScale.z);
        _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        _slideTimer = _maxSlideTime;
    }

    // ----------------------------------------------------------------------
    // Name: SlidingMovement
    // Desc: This method handle the sliding movement, mainly the method apply
    //       an force on the movement, and limit the action on a timer, also,
    //       if the character is an a slope, the timer is disconsidered, and
    //       he slides until the slope end.
    // ----------------------------------------------------------------------
    private void SlidingMovement()
    {
        if (!_onSlope || _rb.linearVelocity.y > -0.1f)
        {
            _rb.AddForce(_moveDirection.normalized * _slideForce, ForceMode.Force);
            _slideTimer -= Time.deltaTime;
        }
        else 
            _rb.AddForce(GetSlopeMoveDirection() * _slideForce, ForceMode.Force);

        if (_slideTimer <= 0) 
            StopSlide();
    }

    // ----------------------------------------------------------------------
    // Name: StopSlide
    // Desc: This method only reset the character scale to the default scale.  
    // ----------------------------------------------------------------------
    private void StopSlide()
    {
        _sliding = false;

        transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
    }

    #endregion

    #region - Throw Rock State -
    private void StartThrowing()
    {
        if (_rockThrower == null) return;
        if (_equippedGun != null) 
            _equippedGun.GunHolst(true);

        _isThrowingObject = true;
        _rockThrower.ThrowRock();
    }
    private void EndRockThrow() => _rockThrower.FinishThrow();
    private void CancelThrowRock()
    {
        _rockThrower.CancelThrowing();
        _isThrowingObject = false;
    }
    #endregion

    #region - Gun Equip System -
    public void EquipGunByName(string name)
    {
        Debug.Log("Gun Name: " + name);
        GunBase gun = GetGunByName(name);

        if (gun == null)
            return;

        int index = _allGuns.IndexOf(gun);

        EquipGun(index);
    }

    public bool GunIsEquiped(string gunName) => GetGunByName(gunName)._isEquiped;
    public GunBase GetGunByName(string name) => _allGuns.Find(e => e.GunData.gunData.gunName == name);

    private void EquipGun(int gunToEquip)
    {
        Debug.Log("Searching gun!");
        if (_allGuns.Count <= 0) return;
        if (_allGuns[0].Equals(null)) return;

        _gunIndex = gunToEquip;

        if (_allGuns[_gunIndex].gameObject.activeInHierarchy ||
            _equippedGun._isReloading ||
            _changingWeapon) return;

        _changingWeapon = true;

        if (_gunIndex == 0) 
            _allGuns[1].GunHolst(false);//Selecting the gun and holsting it
        else 
            _allGuns[0].GunHolst(false);

        _equippedGun = _allGuns[_gunIndex];
    }

    // ----------------------------------------------------------------------
    // Name: GunPermanentHolst
    // Desc: This method holst the current gun permanently
    // ----------------------------------------------------------------------
    public void GunPermanentHolst() => _equippedGun.GunHolst(true);

    public void EquipCurrentGun() => _equippedGun.DrawGun();
    #endregion

    #region - Sound System -
    private void SS_Flashlight()
    {
        if (!flashlightSound.Equals(null))
            AudioManager.Instance.PlayOneShotSound("Effects", flashlightSound, transform.position, 1f, 0, 128);
    }
    #endregion

    #region - Flashlight System -
    private void ChangeFlashlightState()
    {
        _flashLightObject.SetActive(!_flashLightObject.activeInHierarchy);
        SS_Flashlight();
    }
    #endregion

    #region - Saving and Load System -
    public void RegisterDataSaver()
    {
        GameStateManager.Instance.RegisterSavableInstance(this);
    }

    public IEnumerator LoadWithTime(SaveData gameData)
    {
        _rb.linearVelocity    = Vector3.zero;

        yield return new WaitForSeconds(0.05f);

        //gameObject.transform.position       = gameData.playerPosition;
        //_rb.velocity                        = gameData.currentVelocity;
        //_orientation.rotation               = gameData.playerRotation;
        //_cameraObject.transform.rotation    = gameData.cameraRotation;

        //_moveDirection = gameData.currentVelocity;

        //_xRotation = gameData.cameraRotation.eulerAngles.x;
        //_yRotation = gameData.cameraRotation.eulerAngles.y;
    }

    public void Load(SaveData gameData)
    {
        //Debug.Log("CM -> Loading Data"); -> Debug Line

        StartCoroutine(LoadWithTime(gameData));

        if (_gunIndex != 99)
        {
            _equippedGun.GunHolst(true);
            return;
        }

        _gunIndex = gameData.GetPlayerData().GunID;
        EquipGun(_gunIndex);

        _equippedGun.GunData.LoadData(gameData.GetPlayerData()._guns[_gunIndex]);   
    }

    public void Save(SaveData gameData)
    {
        //Debug.Log("CM -> Saving Data"); -> Debug Line

        //gameData.playerPosition     = gameObject.transform.position;
        //gameData.currentVelocity    = _rb.velocity;
        //gameData.playerRotation     = _orientation.rotation;
        //gameData.cameraRotation     = _cameraObject.transform.rotation;

        gameData.GetPlayerData().GunID = _gunIndex;
        gameData.GetPlayerData()._guns.Clear();
        gameData.GetPlayerData()._guns.Add(_equippedGun.GunData.ammoData);
    }
    #endregion
}