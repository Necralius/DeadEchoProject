using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInfo
{
    public Collider             collider            = null;
    public CharacterManager     characterManager    = null;
    public Camera               camera              = null;
    public CapsuleCollider      meleeTrigger        = null;
}

public class GameSceneManager : MonoBehaviour
{
    #region - Singleton Pattern -
    private static GameSceneManager _instance = null;
    public static GameSceneManager Instance
    {
        get
        {
            if (_instance == null) _instance = (GameSceneManager)FindFirstObjectByType(typeof(GameSceneManager));
            return _instance;
        }
    }
    #endregion

    public InputManager inputManager = null;

    //Private
    private Dictionary<int, AiStateMachine> _stateMachines  = new Dictionary<int, AiStateMachine>();
    private Dictionary<int, PlayerInfo>     _playerInfos    = new Dictionary<int, PlayerInfo>();

    [SerializeField] private ParticleSystem _bloodParticles = null;

    [SerializeField] private GameObject _deathScreenObj  = null;
    [SerializeField] private float      _animationsSpeed = 2f;
    [SerializeField] private GameObject _pauseMenuObj    = null;

    [SerializeField]    private GameObject  _inventoryObj   = null;
    public bool         inventoryIsOpen                     = false;

    [Header("Object Inspection System")]
    [SerializeField] private GameObject _inpsectionView;
    [SerializeField] private GameObject _playerView;

    public bool _gameIsPaused;
    public bool _isInspectingItem;

    [SerializeField] private DeathController _deathController = null;

    public ParticleSystem bloodParticles { get => _bloodParticles; }

    private void Awake()
    {
        Time.timeScale = 1f;
    }

    private void OnEscapePerformed(InputAction.CallbackContext context)
    {
        if (inputManager != null && !CharacterManager.Instance.isDead)
            PauseMenu();
    }

    private void OnEnable()
    {
        inputManager.Escape_Action.Action.performed += OnEscapePerformed;
    }

    private void OnDisable()
    {
        inputManager.Escape_Action.Action.performed += OnEscapePerformed;
    }

    public void PauseMenu()
    {
        if (_pauseMenuObj == null) return;

        _gameIsPaused       = !_gameIsPaused;

        CanvasGroup cg = _pauseMenuObj.GetComponent<CanvasGroup>();

        if (cg != null)
        {
            cg.DOFade(_gameIsPaused ? 1f : 0f, _animationsSpeed).SetUpdate(true).onComplete += delegate { PauseActions(); };
            cg.interactable     = _gameIsPaused;
            cg.blocksRaycasts   = _gameIsPaused;
        }
    }

    private void PauseActions()
    {
        Cursor.lockState    = _gameIsPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Time.timeScale      = _gameIsPaused ? 0f : 1f;
    }

    public void ChangeInventoryState()
    {
        inventoryIsOpen = !inventoryIsOpen;

        ChangeInventoryState(inventoryIsOpen);
    }

    public void ChangeInventoryState(bool state)
    {
        inventoryIsOpen     = state;
        Cursor.lockState    = inventoryIsOpen ? CursorLockMode.None : CursorLockMode.Locked;

        InventoryController.Instance.ChangeInventoryState(state);
    }

    public void OpenInspectionView()
    {
        _inpsectionView.SetActive(true);
        _playerView.SetActive(false);

        _inventoryObj.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseInspectionView()
    {
        _inpsectionView.SetActive(false);
        _playerView.SetActive(true);

        _inventoryObj.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void ForcedSaveGame()
    {
        if (GameStateManager.Instance != null)
            _pauseMenuObj.GetComponentInChildren<MenuSystem>().SaveGameData();
    }

    // ----------------------------------------------------------------------
    // Name : RegisterAIStateMachine
    // Desc : Stores the passed state machine in the dictionary with the
    //        instance ID as an key.
    // ----------------------------------------------------------------------
    public void RegisterAIStateMachine(int key, AiStateMachine stateMachine) { if (!_stateMachines.ContainsKey(key)) _stateMachines[key] = stateMachine; }


    // ----------------------------------------------------------------------
    // Name : GetAIStateMachine
    // Desc : Returns an AI State Machine reference searched on by the
    //        intrance ID of an object.
    // ----------------------------------------------------------------------
    public AiStateMachine GetAIStateMachine(int key)
    {
        AiStateMachine machine = null;
        
        if (_stateMachines.TryGetValue(key, out machine)) return machine;
        return null;
    }

    // ----------------------------------------------------------------------
    // Name : RegisterPlayerInfo
    // Desc : Stores the passed PlayerInfo in the dictionary with the
    //        instance ID as an key.
    // ----------------------------------------------------------------------
    public void RegisterPlayerInfo(int key, PlayerInfo playerInfo) { if (!_playerInfos.ContainsKey(key)) _playerInfos[key] = playerInfo; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y)) 
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (inputManager != null && !CharacterManager.Instance.isDead)
        {
            if (inputManager.Escape_Action.Action.WasPressedThisFrame()) 
                PauseMenu();

            if (inputManager.Tab_Action.Action.WasPressedThisFrame() && !_gameIsPaused) 
                ChangeInventoryState();
        }
    }

    // ----------------------------------------------------------------------
    // Name : GetAIStateMachine
    // Desc : Returns an PlayerInfo reference searched on by the
    //        instance ID of an object.
    // ----------------------------------------------------------------------
    public PlayerInfo GetPlayerInfo(int key)
    {
        PlayerInfo info = null;

        if (_playerInfos.TryGetValue(key, out info)) return info;
        return null;
    }

    public void DeathScreen(bool state)
    {
        _deathScreenObj.SetActive(state);

        if (state)
        {
            //SceneLoader.Instance.PreLoadScene("Scene_Level1");
            _deathController.CallDeath();
        }
    }
}