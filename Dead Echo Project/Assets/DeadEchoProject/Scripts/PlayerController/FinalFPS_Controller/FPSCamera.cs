using UnityEngine;
using static NekraByte.Core.DataTypes;

public class FPSCamera : MonoBehaviour, IDataPersistence
{
    [HideInInspector]   public  Camera          mainLookCamera  = null;
    [SerializeField]    private InputManager    _inputManager   = null;
    [HideInInspector]   public  PlayerManager   PlayerManager   = null;

    [Header("Camera Settings")]
    [SerializeField, Range(1, 40)]      private float _sensX = 10f;
    [SerializeField, Range(1, 40)]      private float _sensY = 10f;

    [SerializeField, Range(10f, 200f)]  private float _cameraFollowSpeed = 90f;

    [SerializeField, Range(0f, 180f)]   private float _verticalRotationClamp    = 75f;
    [SerializeField, Range(0f, 50f)]    private float _bodyRotationSpeed        = 15f;
    [SerializeField, Range(0.1f, 10f)]  private float _heightChangeSpeed         = 3f;

    //Private Data
    private float mouseX    = 0;
    private float mouseY    = 0;

    private float rotX      = 0;
    private float rotY      = 0;

    private Transform playerTransform = null;

    float currentYOffest    = 0f;
    float targetYOffset     = 0f;

    [SerializeField] private Transform      _camera     = null;
    [SerializeField] private BodyController _controller = null;

    private Vector3             _startPos;

    private void Awake()
    {
        if (GameStateManager.Instance != null) 
            RegisterDataSaver();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerManager   = playerTransform.GetComponent<PlayerManager>();
        mainLookCamera  = GetComponentInChildren<Camera>();

        _startPos       = _camera.localPosition;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (GameSceneManager.Instance._gameIsPaused)     return;
        if (GameSceneManager.Instance.inventoryIsOpen)   return;
        if (GameSceneManager.Instance._isInspectingItem) return;
        if (CharacterManager.Instance.isDead)            return;

        //Input gethering
        mouseX = _inputManager.LookAction.Vector.y;
        mouseY = _inputManager.LookAction.Vector.x;

        //Rotation calculation and clamping
        rotX -= mouseX * _sensX * Time.deltaTime;
        rotY += mouseY * _sensY * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, -_verticalRotationClamp, _verticalRotationClamp);

        //Rotation apply
        transform.rotation          = Quaternion.Euler(rotX, rotY, 0);
        playerTransform.rotation    = Quaternion.Lerp(playerTransform.rotation, Quaternion.Euler(0, rotY, 0), _bodyRotationSpeed * Time.deltaTime);

        currentYOffest  = PlayerManager.BodyController._currentBodyState.CameraY_Offset;
        targetYOffset   = Mathf.Lerp(targetYOffset, currentYOffest, _heightChangeSpeed * Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (GameSceneManager.Instance.inventoryIsOpen) return;

        transform.position = Vector3.Lerp(transform.position, 
            playerTransform.position + playerTransform.up * targetYOffset, 
            _cameraFollowSpeed * Time.deltaTime);
    }

    public void RegisterDataSaver()
    {
        GameStateManager.Instance.RegisterSavableInstance(this);
    }

    public void Load(SaveData gameData)
    {
        playerTransform.localPosition   = gameData.saveBaseData.playerSave.transformSave.Position;
        playerTransform.rotation        = gameData.saveBaseData.playerSave.transformSave.Rotation;
        transform.localPosition         = gameData.saveBaseData.playerSave.cameraSave.Position;
        transform.rotation              = gameData.saveBaseData.playerSave.cameraSave.Rotation;
    }

    public void Save(SaveData gameData)
    {
        gameData.saveBaseData.playerSave.transformSave  = new ObjectSave(playerTransform);
        gameData.saveBaseData.playerSave.cameraSave     = new ObjectSave(transform);
    }
}