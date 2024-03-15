using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static NekraByte.Core.DataTypes;


public class SaveConteiner : MonoBehaviour
{
    #region - Singleton Pattern -
    public static SaveConteiner Instance;
    private void Awake()
    {
        if (Instance != null) Destroy(Instance);
        Instance = this;
    }
    #endregion

    [Header("Saves Data")]
    public List<SaveData>   _saves          = new List<SaveData>();

    public GameObject _savePrefab = null;

    [SerializeField] private Transform _saveListContent = null;

    [SerializeField] private SaveView _selectedSaveView = null;
    private SaveData _selectedSave;

    private void Start()
    {
        LoadGameSaves();
    }

    public void SelectSave(SaveData save)
    {
        _selectedSaveView?.SetUp(save);
        _selectedSave = save;
    }

    public void LoadGameSaves()
    {
        foreach (Transform child in _saveListContent)
            Destroy(child.gameObject);

        GameStateManager.Instance?.GetAllGameSaves();

        if (GameStateManager.Instance?._allSaves != null && GameStateManager.Instance?._allSaves.Count > 0)
            foreach (var save in GameStateManager.Instance._allSaves)
                Instantiate(_savePrefab, _saveListContent).GetComponent<SaveView>().SetUp(save);
    }

    public void LoadSelectedSave()
    {
        LoadScreen.Instance?.LoadScene(_selectedSave.saveSceneIndex);

        GameStateManager.Instance?.LoadGame(_selectedSave);
    }

    public void DeleteSelectedSave()
    {
        GameStateManager.Instance.DeleteGameSave(_selectedSave.ID);

        _selectedSave = null;
        _selectedSaveView.SetUp(null);

        LoadGameSaves();
    }
}