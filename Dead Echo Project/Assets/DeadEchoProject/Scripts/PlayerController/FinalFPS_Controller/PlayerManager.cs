using System.Collections.Generic;
using System.Linq;
using TMPro.Examples;
using UnityEngine;
using static NekraByte.Core.DataTypes;
using static NekraByte.Core.Enumerators;

[RequireComponent(typeof(InputManager), typeof(InteractionController), typeof(PlayerAudioManager))]
[RequireComponent(typeof(CharacterController), typeof(PlayerAudioManager), typeof(BodyController))]
public class PlayerManager : MonoBehaviour, IDataPersistence
{
    //Direct Dependencies
    private InputManager            _inptManager            => GetComponent<InputManager>();
    private InteractionController   _interactionController  => GetComponent<InteractionController>();
    private CharacterController     _controller             => GetComponent<CharacterController>();
    private PlayerAudioManager      _playerAudioManager     => GetComponent<PlayerAudioManager>();
    private BodyController          _bodyController         => GetComponent<BodyController>();
    private CapsuleCollider         _playerCollider         => GetComponent<CapsuleCollider>();

    private DynamicUI_Manager       _dynamicUI_Manager  = null;
    private FPSCamera               _cameraController   = null;

    //Public encapsulated data
    public InputManager             InputManager            { get => _inptManager;           }
    public CharacterController      Controller              { get => _controller;            }
    public BodyController           BodyController          { get => _bodyController;        }
    public FPSCamera                CameraController        { get => _cameraController;      }
    public InteractionController    InteractionController   { get => _interactionController; }
    public PlayerAudioManager       PlayerAudioManager      { get => _playerAudioManager;    }
    public CapsuleCollider          Collider                { get => _playerCollider;        }
    public DynamicUI_Manager        DynamicUI_Manager       { get => _dynamicUI_Manager;     }

    public List<AnimationLayer> _animLayers = new List<AnimationLayer>();

    [HideInInspector] public RockThrower    _rockThrower    = null;
    [HideInInspector] public Animator       _armsAnimator   = null;

    [Header("Gun Public Sounds")]
    public AudioClip gunShootJam        = null;
    public AudioClip changeGunMode      = null;
    public AudioClip flashlightSound    = null;

    public GameObject ShootPoint = null;

    private void Awake()
    {
        _cameraController = GameObject.FindGameObjectWithTag("CameraObject").GetComponent<FPSCamera>();

        AnimationLayer[] layers = _cameraController.GetComponentsInChildren<AnimationLayer>();
        _animLayers             = layers.ToList();

        _armsAnimator       = AnimationLayer.GetAnimationLayer("AnimationsLayer",   _animLayers).animator;
        _rockThrower        = AnimationLayer.GetAnimationLayer("RockThrowerLayer",  _animLayers).layerObject?.GetComponent<RockThrower>();
        _dynamicUI_Manager  = AnimationLayer.GetAnimationLayer("PlayerCanvas",      _animLayers).layerObject?.GetComponent<DynamicUI_Manager>();
    }

    private void Start()
    {
        //InGame_UIManager.Instance.UpdatePlayerState(this);
        Cursor.lockState = CursorLockMode.Locked;

    }

    private void Update()
    {

    }

    public void RegisterDataSaver()
    {
        throw new System.NotImplementedException();
    }

    public void Load(SaveData gameData)
    {
        throw new System.NotImplementedException();
    }

    public void Save(SaveData gameData)
    {
        throw new System.NotImplementedException();
    }
}