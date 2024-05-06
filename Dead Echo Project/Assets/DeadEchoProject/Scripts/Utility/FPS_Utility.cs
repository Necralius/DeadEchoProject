using System;
using UnityEngine;
using System.Collections.Generic;
using static NekraByte.Core.DataTypes;
using System.IO;
using System.Collections;
using UnityEngine.Audio;
using static NekraByte.Core.Enumerators;
using Random = UnityEngine.Random;

namespace NekraByte
{
    public static class Core
    {
        public class Behaviors
        {
            /// <summary>
            /// This method shoots a raycast forward, using the camera directions as reference.
            /// </summary>
            /// <param name="originTransform"> The origint transform that will shoot the raycast. </param>
            /// <param name="gunData"></param>
            /// <returns></returns>
            public static RaycastHit? ShootUsingRaycast(PlayerManager originTransform, GunDataConteiner gunData)
            {
                Vector3 direction = originTransform.CameraController.mainLookCamera.transform.forward;
                return ShootUsingRaycast(originTransform, gunData, direction);
            }

            public static RaycastHit? ShootUsingRaycast(PlayerManager originTransform, GunDataConteiner gunData, Vector3 direction)
            {
                Transform cameraTransform = originTransform.CameraController.mainLookCamera.transform;

                AudioCollectionTag collection = gunData.gunBulletSettings._collection;

                if (Physics.Raycast(cameraTransform.position, direction, out RaycastHit hit, gunData.gunBulletSettings._shootRange, gunData.gunBulletSettings._collisionMask))
                {
                    //Debug.Log($"Surface -> Name: {hit.transform.name}, Tag: {hit.transform.gameObject.tag}, ")

                    ObjectPooler.Instance.SpawnFromPool(hit.collider.gameObject.tag + "Hit", hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));
                    ObjectPooler.Instance.SpawnFromPool(hit.collider.gameObject.tag + "Decal", hit.point + hit.normal * 0.001f, Quaternion.LookRotation(hit.normal));

                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water")) Debug.Log("True Water");
                    if (collection[hit.transform.tag] != null)
                        AudioManager.Instance.PlayOneShotSound(collection[hit.transform.tag], hit.point, collection);

                    Vector3 impulseForce = -hit.normal * gunData.gunBulletSettings._bulletImpactForce;

                    if (hit.transform.GetComponent<Rigidbody>())
                        hit.transform.GetComponent<Rigidbody>().AddForce(impulseForce, ForceMode.Impulse);

                    if (hit.transform.GetComponentInParent<Rigidbody>())
                        hit.transform.GetComponentInParent<Rigidbody>().AddForce(impulseForce, ForceMode.Impulse);

                    if (hit.transform.GetComponentInChildren<Rigidbody>())
                        hit.transform.GetComponentInChildren<Rigidbody>().AddForce(impulseForce, ForceMode.Impulse);

                    // In this code section the bullet verifies if the hit has finded an object of type AI Body Part and executes an aditional actions on the hit.
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("AI Body Part"))
                    {
                        int damage = (int)Random.Range(gunData.gunBulletSettings._shootDamageRange.x, gunData.gunBulletSettings._shootDamageRange.y);

                        // The method tries to get an valid AI Instance acessing the GameSceneManager and execute an hit action
                        AiStateMachine stateMachine = GameSceneManager.Instance.GetAIStateMachine(hit.rigidbody.GetInstanceID());
                        if (stateMachine)
                            stateMachine.TakeDamage(hit.point, -hit.normal * gunData.gunBulletSettings._bulletImpactForce, damage, hit.rigidbody,
                                originTransform.GetComponent<BodyController>().transform, 0);
                    }
                    return hit;
                }
                else return null;
            }

            /// <summary>
            /// This method returns if the passed Vector has any float as a NaN, Not a Number.
            /// </summary>
            /// <param name="inputValue"> The Vector to be verified. </param>
            /// <returns> The response if the vector is invalid or not. </returns>
            public static bool IsNaN(Vector3 inputValue) => float.IsNaN(inputValue.x) || float.IsNaN(inputValue.y) || float.IsNaN(inputValue.z);
        }

        // --------------------------------------------------------------
        // Name: DataTypes (Class)
        // Desc: This class storages all costum DataTypes from the
        //       NekraByte Unity Package.
        // --------------------------------------------------------------
        public class DataTypes
        {
            #region - Pool Model -
            [Serializable]
            public struct Pool
            {
                public string poolTag;
                public int poolSize;
                public GameObject prefab;
            }
            #endregion

            #region - Data Saving System -

            #region - Game Save Data -
            [Serializable]
            public class GameSaveData
            {
                public GameSaveData(string saveName)
                {
                    this.saveName = saveName;
                    saveTime = new DateTimeSerialized(DateTime.Now);
                }

                [Header("Save Data")]
                public string saveName = string.Empty;
                public DateTimeSerialized saveTime = null;
                public SaveDirectory saveDirectoryData = null;
                public int saveIndex = 0;

                [Header("Player Data")]
                public ObjectSave playerSave;
                public ObjectSave cameraSave;

                public List<ObjectSave> objectsSaves = new List<ObjectSave>();

                public float playerHealth = 0f;

                [Header("Gun Data")]
                public int GunID = 0;

                public List<GunDataConteiner.AmmoData> guns = new List<GunDataConteiner.AmmoData>();

            }
            #endregion

            #region - Application Permanent Data -
            [Serializable]
            public class ApplicationData
            {
                public ApplicationData()
                {
                    //Updates the save time hour.
                    saveTime = new DateTimeSerialized(DateTime.Now);

                    //Create a default resolution
                    currentResolution.width     = 1920;
                    currentResolution.height    = 1080;

                    //Reset all settings to default
                    ResetToDefault();
                }

                public List<SaveDirectory> savesDatas;

                public DateTimeSerialized saveTime;

                [Header("Game Settings")]

                [Space]

                [Header("Audio Settings")]
                public List<AudioTrackVolume> _volumes = new List<AudioTrackVolume>();

                [Space]

                [Header("Graphics Settings")]

                [Header("Resolution")]
                public Resolution currentResolution;
                public int resolutionIndex = 0;

                public bool isFullscreen = true;
                [Header("vSync")]
                public bool vSyncActive = false;
                public int vSyncCount = 0;

                [Header("Shadows")]
                public int shadowQuality = 1;
                public int shadowResolution = 1;

                [Header("Quality Settings")]
                public int qualityLevelIndex = 1;

                public int anisotropicFiltering = 1;
                public int antialiasing = 1;
                public int globalTextureQuality = 1;

                public float brightness = 1f;

                [Header("Gameplay Settings")]
                public float xSensitivity = 7f;
                public float ySensitivity = 7f;

                public bool invertX = false;
                public bool invertY = false;

                public int aimType              = 0;
                public int crouchType           = 0;

                public bool subtitles           = true;
                public int  subtitlesLanguage   = 0;
                public int  audioLanguage       = 0;

                public void StartNewSave(SaveDirectory info)
                {
                    if (savesDatas == null || savesDatas.Count <= 0) 
                        savesDatas = new List<SaveDirectory>();

                    savesDatas.Add(info);
                }

                public int GetSaveQuantity() => savesDatas.Count;

                public bool GetSavePathByIndex(int saveIndexer, out SaveDirectory directory)
                {
                    if (saveIndexer >= savesDatas.Count || saveIndexer < 0)
                    {
                        directory = null;
                        return false;
                    }
                    directory = savesDatas[saveIndexer];
                    return true;
                }

                public void DeleteSave(SaveData save)
                {
                    if (save == null) return;

                    int index = savesDatas.FindIndex(e => e.saveID == save.ID);

                    if (File.Exists(savesDatas[index].savePath))               File.Delete(savesDatas[index].savePath);
                    if (File.Exists(savesDatas[index].screenshotPath))         File.Delete(savesDatas[index].screenshotPath);
                    if (Directory.Exists(savesDatas[index].saveFolderPath))    Directory.Delete(savesDatas[index].saveFolderPath);

                    savesDatas.RemoveAt(index);
                }

                public void UpdateSave() => saveTime = new DateTimeSerialized(DateTime.Now);

                public void ResetToDefault()
                {
                    ResetVolumeSettings();
                    ResetGraphicsSettings();
                    ResetGameplaySettings();
                }

                public void ResetVolumeSettings()
                {
                    _volumes.Clear();
                }

                public void ResetGraphicsSettings()
                {
                    resolutionIndex = 0;

                    isFullscreen    = true;

                    vSyncCount      = 0;
                    vSyncActive     = false;

                    shadowQuality       = 1;
                    shadowResolution    = 1;

                    anisotropicFiltering    = 1;
                    antialiasing            = 1;
                }
                public void ResetGameplaySettings()
                {
                    xSensitivity    = 7f;
                    ySensitivity    = 7f;

                    invertX         = false;
                    invertY         = false;

                    aimType         = 0;
                    crouchType      = 0;
                }
            }

            [Serializable]
            public class DateTimeSerialized
            {
                public string Hour      = "00";
                public string Minute    = "00";

                public string Day       = "00";
                public string Month     = "00";
                public string Year      = "00";

                public DateTimeSerialized(DateTime dateTime)
                {
                    Hour    = dateTime.Hour.    ToString();
                    Minute  = dateTime.Minute.  ToString();
                    Day     = dateTime.Day.     ToString();
                    Month   = dateTime.Month.   ToString();
                    Year    = dateTime.Year.    ToString();
                }
            }

            #endregion

            #region - Save System Interaction -
            public interface IDataPersistence
            {
                void RegisterDataSaver();
                void Load(SaveData gameData);
                void Save(SaveData gameData);
            }
            #endregion

            #endregion

            #region - Gun Data Main Conteiner -       
            [Serializable]
            public class GunDataConteiner
            {
                [Header("Gun General Data")]
                public GunData          gunData             = new GunData();
                public BulletSettings   gunBulletSettings   = new BulletSettings();
                public RecoilData       recoilData          = new RecoilData();
                public AmmoData         ammoData            = new AmmoData();

                public void LoadData(AmmoData savedConteiner) => ammoData = savedConteiner;

                #region - Gun Data Model -
                [Serializable]
                public class GunData
                {
                    [Header("Gun Aspects")]
                    public string       gunName = "GunName";
                    public ShootType    shootType;
                    public GunMode      gunMode;

                    [Range(0.01f, 2f)] public float rateOfFire = 0.1f;

                    [Header("Aim Data")]
                    public Vector3  aimOffset           = new Vector3();
                    public float    _aimReloadOffset    = 0.3f;
                }
                #endregion

                #region - Bullet Settings Model -
                [Serializable]
                public class BulletSettings
                {
                    [Header("Gun Settings"), Tooltip("Gun Bullet Settings")]
                                        public  Vector2              _shootDamageRange      = new(10f, 25f);
                    [Range(10, 2000)]   public  float                _shootRange            = 100f;
                    [SerializeField]    public  string               _bulletTag             = "RifleBullet";
                    [Range(1, 1000)]    public  float                _bulletSpeed           = 200f;
                    [Range(0.1f, 50)]   public  float                _bulletGravity         = 2f;
                    [Range(0.001f, 10)] public  float                _bulletSpread          = 0.1f;
                    [Range(1f, 15f)]    public  float                _bulletLifeTime        = 5f;
                    [Range(1, 10)]      public  int                  _bulletsPerShoot       = 1;
                    [Range(1f, 30f)]    public  float                _bulletImpactForce     = 10f;
                                        public  LayerMask            _collisionMask;
                    [SerializeField]    public  AudioCollectionTag   _collection;
                }
                #endregion

                #region - Gun Recoil Asset -
                [Serializable]
                public class RecoilData
                {
                    [Header("Recoil Vertical Settings")]
                    public float _recoilX = -3;
                    public float _recoilY = 3;
                    public float _recoilZ = 2;

                    [Header("Back Recoil")]
                    public float _zKickback = 0.4f;

                    [Header("Recoil Smoothing")]
                    public float _snappiness = 6f;
                    public float _returnSpeed = 20f;

                    [Header("Aim Effector"), Tooltip("When aiming the player receive less recoil, this variable change the recoil reduction")]
                    public float _recoilReduction = 0.3f;
                }
                #endregion

                #region - Ammo Data -
                [Serializable]
                public class AmmoData
                {
                    [Header("Gun Ammo"), Tooltip("Gun ammo quantity settings")]
                    [Range(0, 500)] public int _bagMaxAmmo = 200;
                    [Range(1, 150)] public int _magMaxAmmo = 31;

                    public int _bagAmmo = 60;
                    public int _magAmmo = 31;
                }
                #endregion
            }
            #endregion

            #region - Gun Audio Asset -
            [Serializable]
            public struct AudioAsset
            {
                public AudioClip ShootClip;
                public AudioClip AimClip;
                public AudioClip DrawClip;
                public AudioClip HolstClip;
                public AudioClip ReloadClip;
                public AudioClip ReloadClipVar1;
                public AudioClip FullReloadClip;
                public AudioClip BoltActionClip;
            }
            #endregion

            #region - Sway Data Model -
            [Serializable]
            public class SwayData
            {
                [Header("Sway State")]
                public bool _inputSway      = true;
                public bool _movementSway   = true;
                public bool _idleSway       = true;

                [Header("Input Sway")]
                public float inpt_amountX;
                public float inpt_amountY;
                public float inpt_smoothAmount;
                public float inpt_swayResetSmooth;

                public float inpt_clampX = 12f;
                public float inpt_clampY = 12f;

                [Header("Weapon Movement Sway")]
                public float move_amountX;
                public float move_amountY;
                public float move_SmoothAmount;

                [Header("Idle Sway")]
                public float swayAmountA    = 1f;
                public float swayAmountB    = 2f;
                public float swayScale      = 400f;
                public float swayLerpSpeed  = 14f;

                [HideInInspector] public float swayTime;

                [Header("Aim Effectors")]
                public float idle_AimEffector = 0.3f;
                public float inpt_AimEffector = 0.3f;
                public float move_AimEffector = 0.3f;
            }
            #endregion

            #region - Player Movment Data Conteiner -
            [Serializable]
            public class PlayerMovmentData
            {
                #region - Player Data Settings -
                [Header("Player Movmennt Settings")]
                [Range(0.01f, 10f)] public float _walkSpeed         = 4.0f;
                [Range(0.01f, 30f)] public float _sprintSpeed       = 8.0f;
                [Range(0.01f, 30f)] public float _crouchSpeed       = 2.0f;
                [Range(0.01f, 50f)] public float _jumpForce         = 4.0f;
                [Range(0.01f, 100f)] public float _playerGravity    = 30f;

                [Header("Controller Settings Data")]
                [Range(0.01f, 10f)] public float _xLookSensitivity  = 2.0f;
                [Range(0.01f, 10f)] public float _yLookSensitivity  = 2.0f;
                [Range(0.01f, 100f)] public float _upperLookLimit   = 80f;
                [Range(0.01f, 100f)] public float _lowerLookLimit   = 80f;

                [Header("Crouch Settings")]
                [Range(0.1f, 2f)] public float _crouchHeight    = 0.5f;
                [Range(0.1f, 2f)] public float _standingHeight  = 2.0f;
                public float _timeToCrouch = 0.25f;
                public LayerMask _crouchUpLayer;
                public Vector3 _crouchingCenter     = new Vector3(0, 0.5f, 0);
                public Vector3 _standingCenter      = new Vector3(0, 0, 0);
                #endregion

                #region - HeadBob System -
                [Header("HeadBob Settings")]
                [Range(0, 100)] public float walkBobSpeed       = 14.0f;
                [Range(0, 100)] public float walkBobAmount      = 0.05f;

                [Range(0, 100)] public float sprintBobSpeed     = 18.0f;
                [Range(0, 100)] public float sprintBobAmount    = 0.11f;

                [Range(0, 100)] public float crouchBobSpeed     = 8.000f;
                [Range(0, 100)] public float crouchBobAmount    = 0.025f;
                #endregion
            }
            #endregion

            #region - Character UI State -
            [Serializable]
            public struct CharacterState
            {
                public StateType type;
                public Sprite stateSprite;
            }
            #endregion

            #region - Audio Data -

            #region - Audio Pool Data -
            // ----------------------------------------------------------
            // Name: AudioPoolItem (Class)
            // Desc: This class declares an data holder for an audio entity that is used in an audio pooling system.
            // ----------------------------------------------------------
            public class AudioPoolItem
            {
                public GameObject GameObject = null;
                public Transform Transform = null;
                public AudioSource AudioSource = null;
                public float Uniportance = float.MaxValue;
                public bool Playing = false;
                public IEnumerator Coroutine = null;
                public ulong ID = 0;


            }
            #endregion

            #region - Audio Track Info -
            // --------------------------------------------------------------------------
            // Name: TrackInfo (Class)
            // Desc: This class holds the AudioMixerGroup base information and holds an
            //       Coroutine that executes the track fading system.
            // --------------------------------------------------------------------------
            public class TrackInfo
            {
                public string Name = string.Empty;
                public AudioMixerGroup Group = null;
                public IEnumerator TrackFader = null;
            }

            [Serializable]
            public class AudioTrackVolume
            {
                public string Name = string.Empty;
                public float Volume = 0f;
                public AudioTrackVolume(string name, float volume)
                {
                    Name = name;
                    Volume = volume;
                }
            }
            #endregion

            #endregion

            #region - Floor Data -
            [Serializable]
            public class FloorData
            {
                //Data Types
                public enum FloorType { Grass, Dirt, Stone, Metal, Wood, Mud, Water }
                public enum FloorHolder { Terrain, GameObject }

                //Private Data
                private FloorHolder _holder = FloorHolder.GameObject;
                private GameObject _objectToFindFloor = null;
                private Collider _objectCollider = null;
                private GameObject _findedFloor = null;

                [SerializeField] private FloorType _type = FloorType.Grass;
                private FloorType _previousType = FloorType.Grass;
                [SerializeField] private TerrainTextureDetector _terrainTextureDetector = null;
                [SerializeField] private float _footDistance = 0.5f;

                //Public Data
                public FloorType Type
                {
                    get
                    {
                        _type = UpdateFloorType(_objectToFindFloor.transform);
                        _previousType = _type;
                        return _type;
                    }
                }

                public FloorData(GameObject objectToFindFloor, Collider objectCollider)
                {
                    _objectToFindFloor      = objectToFindFloor;
                    _objectCollider         = objectCollider;
                    _terrainTextureDetector = new TerrainTextureDetector(objectToFindFloor, _objectCollider);
                }

                public FloorType UpdateFloorType(Transform objectToFindFloor)
                {
                    GameObject floorFinded = null;
                    RaycastHit hit;

                    if (Physics.Raycast(objectToFindFloor.position, Vector3.down, out hit, _objectCollider.bounds.extents.y + _footDistance))
                    {
                        _holder = hit.transform.CompareTag("Terrain") ? FloorHolder.Terrain : FloorHolder.GameObject;

                        floorFinded = hit.transform.gameObject;
                        //Debug.Log($"FD -> Finded the object: {floorFinded.gameObject.name}");
                    }
                    else
                    {
                        _findedFloor = null;
                        //Debug.Log("FD -> Floor object not finded! Setting the previous type: " + _previousType.ToString());
                        return _previousType;
                    }

                    _findedFloor = floorFinded;

                    if (floorFinded == null) return _previousType;
                    else if (_holder == FloorHolder.GameObject)
                    {
                        //Debug.Log($"Floor Finded: {floorFinded.gameObject.name}, Tag: {floorFinded.tag}");
                        switch (floorFinded.tag)
                        {
                            case "Dirt":        return FloorType.Dirt;
                            case "Stone":       return FloorType.Stone;
                            case "Concrete":    return FloorType.Stone;
                            case "Metal":       return FloorType.Metal;
                            case "Wood":        return FloorType.Wood;
                            default:            return _previousType;
                        }
                    }
                    else if (_holder == FloorHolder.Terrain)
                    {
                        //Debug.Log($"FD -> Terrain Texture Finded: {_terrainTextureDetector.GetCurrentTexture()}");
                        switch (_terrainTextureDetector.GetCurrentTexture())
                        {
                            case "TerrainDirt": return FloorType.Dirt;
                            case "TerrainMud":  return FloorType.Mud;
                            case "TerrainWet":  return FloorType.Water;
                            default:            return _previousType;
                        }
                    }
                    return _previousType;
                }
            }
            #endregion

            [Serializable]
            public class BodyState
            {
                [Tooltip("State Identifier")]
                public string       Name            = "StateName";

                [Tooltip("State Type")]
                public StateType    State           = StateType.Stand;

                [Tooltip("Camera Y Offset")]
                public float        CameraY_Offset  = 1.30f;

                [Tooltip("State Sprite")]
                public Sprite       Sprite          = null;

                [Tooltip("Movement State Type")]
                public MovementState MovementState  = MovementState.Walking;
            }
        }

        // --------------------------------------------------------------
        // Name: Procedural (Class)
        // Desc: Mainly, this static class, brings some procedural tools,
        //       that can help the gun movimentation and handling.
        // --------------------------------------------------------------
        public static class Procedural
        {
            #region - Spring Lerping -
            public class SpringInterpolator
            {
                private Vector3 _currentPos;
                private Vector3 velocity;

                // ------------------------------------------------------
                // Name: SpringInterpolator (Constructor)
                // Desc: This constructor set an importat variable to
                //       this class, and is called on the object
                //       instatiation.
                // ------------------------------------------------------
                public SpringInterpolator(Vector3 currentPos) => _currentPos = currentPos;

                // ------------------------------------------------------
                // Name: SpringLerp
                // Desc: This method produces an movimentation using as
                //       base the spring interpolation math principle.
                // ------------------------------------------------------
                public Vector3 SpringLerp(Vector3 targetPos, float damping, float stiffness, float deltaTime)
                {
                    Vector3 springForce = stiffness * targetPos - _currentPos;
                    Vector3 dampingForce = -damping * velocity;

                    Vector3 totalForce = springForce + dampingForce;

                    Vector3 acceleration = totalForce;
                    velocity += acceleration * deltaTime;
                    _currentPos += velocity * deltaTime;
                    return _currentPos;
                }
            }
            #endregion

            #region - Json File Data Handler -
            public class FileDataHandler
            {
                private string dataDirPath = "";
                private string dataFileName = "";

                public GameSaveData data;

                public FileDataHandler(string dataDirPath, string dataFileName)
                {
                    this.dataDirPath = dataDirPath;
                    this.dataFileName = dataFileName;
                }

                public void SaveGunData(GameSaveData data)
                {
                    string fullPath = Path.Combine(dataDirPath, dataFileName);
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                        string dataToStore = JsonUtility.ToJson(data, true);

                        using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                        {
                            using (StreamWriter writer = new StreamWriter(stream)) writer.Write(dataToStore);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.ToString());
                    }
                }
                public GameSaveData LoadGunData()
                {
                    string fullPath = Path.Combine(dataDirPath, dataFileName);
                    GameSaveData loadedData = null;
                    if (File.Exists(fullPath))
                    {
                        try
                        {
                            string dataToLoad = "";
                            using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                            {
                                using (StreamReader reader = new StreamReader(stream)) dataToLoad = reader.ReadToEnd();
                            }

                            loadedData = JsonUtility.FromJson<GameSaveData>(dataToLoad);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e.ToString());
                        }
                    }
                    return loadedData;
                }
            }
            #endregion
        }

        // --------------------------------------------------------------
        // Name: Enumerators
        // Desc: Mainly, this static class, brings some enumerators
        //       tools, that can help to build new descriptives
        //       DataTypes.
        // --------------------------------------------------------------
        public static class Enumerators
        {
            #region - Reticle State -
            public enum ReticleState
            {
                Walking,
                Spriting,
                Crouching,
                Shooting,
                Aiming,
                Reloading,
                InAir,
                Idle
            }
            #endregion

            #region - Gun Mode -
            public enum GunMode
            {
                Auto,
                Semi,
                Locked
            }
            #endregion

            #region - ShootType -
            public enum ShootType
            {
                Auto_Shotgun,
                Semi_Shotgun,
                Auto_Rifle,
                Semi_Rifle,
                Semi_Pistol,
                Auto_Pistol,
                Sniper
            }
            #endregion

            #region - Animation Layer Type -
            public enum LayerBehavior
            {
                None,
                Additive,
                Override,
                Referencial
            }
            #endregion

            #region - Player State Types -
            public enum StateType
            {
                Stand,
                Crouch,
                Jumping
            }
            public enum InteractionButton { TAB, E, Q, ENTER }
            public enum MovementState { Idle, Walking, Sprinting, Crouching, Air, Sliding }
            #endregion

            #region - Languages - 
            public enum LanguageOption
            {
                English     = 0,
                Portuguese  = 1,
                Spanish     = 2,
                German      = 3,
                Italian     = 4,
                French      = 5
            }
            #endregion
        }
    }
}