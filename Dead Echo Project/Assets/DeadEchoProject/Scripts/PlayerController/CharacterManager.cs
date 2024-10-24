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
    [SerializeField] private    float _timeToCure       = 4f;
    private                     float _cureTimer        = 0f;

    public bool isDead = false;

    [Header("Survival Data")]
    [SerializeField] private SurvivalData _survivalData = null;

    [SerializeField, Range(0f, 100f)] private float _hunger  = 0f;
    [SerializeField, Range(0f, 100f)] private float _thirst  = 0f;
    [SerializeField, Range(0f, 100f)] private float _stamina = 0f;

    [SerializeField] private Image _hungerFill  = null;
    [SerializeField] private Image _thirstFill  = null;
    [SerializeField] private Image _staminaFill = null;

    [SerializeField] private float _damageTimer  = 0f;
    [SerializeField] private float _timeToDamage = 4f;

    public float Hunger
    {
        get => _hunger;
        set
        {
            if (value > 100f)
                _hunger = 100f;
            else _hunger = value;
        }
    }

    public float Thirst
    {
        get => _thirst;
        set
        {
            if (value > 100f)
                _thirst = 100f;
            else _thirst = value;
        }
    }

    public float Stamina
    {
        get => _stamina;
        set
        {
            if (value > 100f)
                _stamina = 100f;
            else _stamina = value;
        }
    }

    // ------------------------------------------ Methods ------------------------------------------ //

    public float CurrentHealth
    {
        get => _currentHealth;
        set
        {
            if (value > _maxHealth) 
                _currentHealth = _maxHealth;
            else if (value < 0) 
                _currentHealth = 0;
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
        if (isDead || GameSceneManager.Instance._gameIsPaused) 
            return;

        if (CurrentHealth < _maxHealth)
        {
            if (_cureTimer >= _timeToCure)
            {
                CurrentHealth += _cureValuePerSec / 1000;
                _inGameUI_Manager.UpdatePlayerState(_fpsController, this);
            }
            else _cureTimer += Time.deltaTime;
        }

        if (_survivalData != null)
        {
            Hunger += _survivalData.hungerFactor * Time.deltaTime;
            Thirst += _survivalData.thirstFactor * Time.deltaTime;

            if (_hungerFill != null)
                _hungerFill.fillAmount   = Hunger / 100f;

            if (_thirstFill != null)
                _thirstFill.fillAmount   = Thirst / 100f;

            if (_staminaFill != null)
                _staminaFill.fillAmount = Stamina / 100f;

            if (Hunger >= 99f || Thirst >= 99f)
            {
                if (_damageTimer >= _timeToDamage)
                {
                    TakeDamage(_survivalData.DamageOnMax);
                    _damageTimer = 0f;
                }
                else _damageTimer += Time.deltaTime;
            }
            else _damageTimer = 0f;
        }
    }
    #endregion

    #region - Damage System -
    public void TakeDamage(float value)
    {
        DamageEffect.Instance.ScreenDamageEffect(0.3f, 5f, 0.3f);

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