using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NekraByte.Core.DataTypes;

public class CharacterManager : MonoBehaviour, IDataPersistence
{
    #region - Singleton Pattern -
    public static CharacterManager Instance;
    private void Awake() => Instance = this;
    #endregion

    //Inspector assinged
    [Header("Dependencies")]
    public                      ScreenDamageManager        _damageManager       = null;
    [SerializeField] private    CapsuleCollider            _meleeTrigger        = null;
    [SerializeField] private    Camera                     _fpsCamera           = null;
    [SerializeField] private    InGame_UIManager           _inGameUI_Manager    = null;

    [Header("Health System")]
    private                 float      _currentHealth  = 100f;
    [Range(0f, 300)] public float      _maxHealth      = 100f;

    //Private
    private Collider                _collider               = null;
    [SerializeField] private BodyController          _fpsController          = null;
    private GameSceneManager        _gameSceneManager       = null;

    [SerializeField] private    float _cureValuePerSec  = 1f;
    private                     float _cureTimer        = 0f;
    private                     float _timeToCure       = 4f;

    public bool isDead = false;

    // ------------------------------------------ Methods ------------------------------------------ //

    public float CurrentHealth
    {
        get => _currentHealth;
        set
        {
            if (value > _maxHealth) _currentHealth = _maxHealth;
            else if (value < 0) _currentHealth = 0;
            else _currentHealth = value;
        }
    }

    #region - BuiltIn Methods -
    private void Start()
    {
        _collider               = GetComponentInChildren<Collider>();
        _fpsController          = GetComponent<BodyController>();
        _gameSceneManager       = GameSceneManager.Instance;
        _inGameUI_Manager       = InGame_UIManager.Instance;

        RegisterDataSaver();

        if (_gameSceneManager != null)
        {
            PlayerInfo playerInfo           = new PlayerInfo();

            playerInfo.camera               = _fpsCamera;
            playerInfo.characterManager     = this;
            playerInfo.collider             = _collider;
            playerInfo.meleeTrigger         = _meleeTrigger;

            _gameSceneManager.RegisterPlayerInfo(_collider.GetInstanceID(), playerInfo);
        }
    }

    private void Update()
    {
        if (CurrentHealth < _maxHealth)
        {
            if (_cureTimer >= _timeToCure)
            {
                CurrentHealth += _cureValuePerSec / 1000;
                _inGameUI_Manager.UpdatePlayerState(_fpsController, this);
            }
            else _cureTimer += Time.deltaTime;
        }
    }
    #endregion

    #region - Damage System -
    public void TakeDamage(float value)
    {
        CurrentHealth = Mathf.Max(_currentHealth - value, 0f);

        if (_damageManager != null)
        {
            _damageManager.minBloodAmount   = (1.0f - (CurrentHealth / 100f));
            _damageManager.bloodAmount      = Mathf.Min(_damageManager.minBloodAmount + 0.3f, 1f);
        }

        if (CurrentHealth <= (_maxHealth / 4)) _damageManager.SetCriticalHealth();
        else _damageManager.SetCriticalHealth();

        _inGameUI_Manager.UpdatePlayerState(_fpsController, this);

        if (CurrentHealth <= 0) Die();
        _cureTimer = 0f;
    }
    #endregion

    private void Die()
    {
        GameSceneManager.Instance.DeathScreen(true);
        Cursor.lockState    = CursorLockMode.None;
        isDead              = true;
    }
    public void Revive()
    {
        GameSceneManager.Instance.DeathScreen(false);
        Cursor.lockState = CursorLockMode.Locked;
        isDead = false;
        CurrentHealth = _maxHealth;
    }

    public void RegisterDataSaver()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.RegisterSavableInstance(this);
    }

    public void Load(SaveData gameData)
    {
        CurrentHealth = gameData.GetPlayerData().playerHealth;
        Revive();
    }

    public void Save(SaveData gameData)
    {
        gameData.GetPlayerData().playerHealth = CurrentHealth;
    }
}