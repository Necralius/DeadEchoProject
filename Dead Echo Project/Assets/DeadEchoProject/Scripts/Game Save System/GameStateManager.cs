using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static NekraByte.Core.DataTypes;

public class GameStateManager : MonoBehaviour
{
    #region - Singleton Pattern -
    public static GameStateManager Instance;
    #endregion

    public Sprite noSaveImage;

    #region - Save Names -
    [SerializeField] private string permanentSaveName   = "ApplicationData";
    [SerializeField] private string saveName            = "GameSave";
    #endregion

    //Data Writers
    private FileDataHandler dynamicDataHandler;// -> This file handler, only is active for the game save data, he is very mutable.
    private FileDataHandler staticDataHandler; // -> This file handler, only is active for the application data file, this variable is imutable.

    //An list of all savable objects in the game scene.
    private List<IDataPersistence> dataPersistenceObjects = new List<IDataPersistence>(); 

    public SaveData currentGameData = null;

    public ApplicationData currentApplicationData = null; // -> This file stores all the application data.

    MenuSystem menuSystem;

    public List<SaveData> _allSaves;

    [SerializeField] private int defaultSceneIndex = 1;

    // ------------------------------------------ Methods ------------------------------------------ //

    #region - Built In Methods -
    private void Awake()
    {
        if (Instance != null) Destroy(Instance.gameObject);
        Instance = this;

        DontDestroyOnLoad(gameObject);

        staticDataHandler       = new FileDataHandler(Application.persistentDataPath, permanentSaveName);

        LoadApplicationData();
    }
 
    private void LoadApplicationData()
    {
        currentApplicationData = staticDataHandler.LoadApplicationData();

        if (currentApplicationData == null)
        {
            Debug.Log("Creating new application data!");
            staticDataHandler.EncapsulateApplicationData(new ApplicationData());
        }
        Debug.Log("Loading application data!");
    }

    private void Start()
    {
        currentGameData = null;
        menuSystem      = GameObject.FindGameObjectWithTag("MenuSystem").GetComponent<MenuSystem>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) 
            SaveGameData();

        if (Input.GetKeyDown(KeyCode.V)) 
            LoadGame();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += LoadSaveOnSceneLoad;
        //SceneManager.sceneLoaded += SaveGameOnSceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= LoadSaveOnSceneLoad;
        //SceneManager.sceneLoaded -= SaveGameOnSceneLoad;
    }
    #endregion

    #region - Data Load -

    private void LoadSaveOnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        //if (saveIndexer <= -1) return;
        if (menuSystem == null) return;

        LoadGame();
    }

    public void LoadGame()
    {
        if (dynamicDataHandler == null) return;
        Debug.Log("Loading Game!");
        StartCoroutine(LoadWithTime(0.5f));
    }

    public void LoadGame(SaveData save)
    {
        if (dynamicDataHandler == null) return;

        currentGameData = save;

        StartCoroutine(LoadWithTime(0.5f));
    }
    #endregion

    IEnumerator LoadWithTime(float time)
    {
        yield return new WaitForSeconds(time / 2);

        LoadAllEntities();

        yield return new WaitForSeconds(time / 2);
    }

    #region - Data Save -
    public void SaveGameData()
    {
        SaveAllEntities();

        if (currentGameData.saveDirectory.savePath is null || currentGameData.saveDirectory.savePath == string.Empty) return;

        dynamicDataHandler = new FileDataHandler(Application.persistentDataPath, currentGameData.saveName);

        dynamicDataHandler.EncapsulateData(currentGameData);

        Debug.Log("Saving Game!");
    }

    public void NewGameSave()
    {
        int     generatedID         = Random.Range(0, 10000);
        string  currentSaveName     = saveName + generatedID;

        dynamicDataHandler      = new FileDataHandler(Application.persistentDataPath, currentSaveName);
        
        currentGameData                 = new SaveData(saveName, generatedID);
        currentGameData.saveSceneIndex  = defaultSceneIndex;

        SaveAllEntities();
        if (dynamicDataHandler  == null) return;
        if (currentGameData     == null) return;

        SaveDirectory saveDir   = dynamicDataHandler.EncapsulateData(currentGameData);
        saveDir.saveID          = generatedID;

        currentApplicationData.StartNewSave(saveDir);

        LoadScreen.Instance.LoadScene(currentGameData.saveSceneIndex);

        SaveApplicationData();
    }

    private void LoadAllEntities()
    {
        if (dataPersistenceObjects.Count <= 0 || dataPersistenceObjects == null)
        {
            //Debug.Log("Null game data save!");
            return;
        }

        if (currentGameData == null)
        {
            //Debug.Log("Null game data save!");
            return;
        }

        foreach (IDataPersistence dataPersistence in dataPersistenceObjects)
            dataPersistence.Load(currentGameData);
    }

    private void SaveAllEntities()
    {
        if (dataPersistenceObjects.Equals(null) || dataPersistenceObjects.Count == 0)
        {
            //Debug.Log("Don't exist persistence objects!"); -> Debug Line
            return;
        }

        if (currentGameData == null)
        {
            //Debug.Log("Null game data save!");
            return;
        }

        foreach (IDataPersistence dataPersistence in dataPersistenceObjects)
            dataPersistence.Save(currentGameData);
    }

    public List<SaveData> GetAllGameSaves()
    {
        LoadApplicationData();

        _allSaves.Clear();
        dynamicDataHandler      = new FileDataHandler();

        if (currentApplicationData == null) 
            currentApplicationData = staticDataHandler.LoadApplicationData(); 

        if (currentApplicationData.savesDatas == null || currentApplicationData.savesDatas.Count <= 0)
        {
            //Debug.Log("Null saves, or not founded any saves!");
            return null;
        }
        else foreach (var dataDir in currentApplicationData.savesDatas)
                _allSaves.Add(dynamicDataHandler.LoadGameState(dataDir.savePath));
        return _allSaves;
    }

    public void SaveApplicationData()
    {
        currentApplicationData.UpdateSave();
        staticDataHandler.EncapsulateApplicationData(currentApplicationData);

        staticDataHandler.LoadApplicationData();
    }

    public void DeleteGameSave(int ID)
    {
        currentApplicationData.DeleteSave(GetSaveByID(ID));

        _allSaves.Remove(GetSaveByID(ID)); //Remove save from list

        SaveApplicationData();
        GetAllGameSaves();
    }

    private SaveData GetSaveByID(int ID)
    {
        foreach (var save in _allSaves)
            if (save.ID == ID) return save;
        return null;
    }
    #endregion

    public void RegisterSavableInstance(IDataPersistence dataPersistence) => dataPersistenceObjects.Add(dataPersistence);

}