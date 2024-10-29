using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveScreen : MonoBehaviour
{
    [SerializeField] private List<SaveData> _saves = new List<SaveData>();

    public GameObject _savePrefab = null;

    [SerializeField] private Transform _saveListContent = null;

    [SerializeField] private SaveView _selectedSaveView = null;

    private SaveData _selectedSave;

    [SerializeField] private MenuManager    _menuManager   = null;
    [SerializeField] private CameraManager  _cameraManager = null;
    [SerializeField] private GameObject     _loadingLogo = null;
    [SerializeField] private Slider         _loadingSlider = null;

    public void SelectSave(SaveData save)
    {
        _selectedSaveView?.SetUp(save);
        _selectedSave = save;
    }

    public void UpdateSaves()
    {
        _saves = GameStateManager.Instance?.GetAllGameSaves();

        foreach (Transform child in _saveListContent)
            Destroy(child.gameObject);

        if (_saves != null && _saves.Count > 0)
            _saves.ForEach(e => { Instantiate(_savePrefab, _saveListContent).GetComponent<SaveView>().SetUp(e); });
    }

    public void LoadSelectedSave()
    {
        _menuManager.OpenMenu("LoadingScreen", delegate { _loadingLogo.SetActive(true); });
        _cameraManager.SetCameraPriority("LoadingCam");

        StartCoroutine(FinishLoading(10f));
    }

    IEnumerator FinishLoading(float maxTime)
    {
        float duration = maxTime;

        float elapsed = 0f;
        _loadingSlider.maxValue = duration;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _loadingSlider.value = elapsed;
            yield return null;
        }

        //yield return new WaitForSeconds(maxTime);
        FadeSystemManager.Instance.CallFadeAction(delegate { FinishSaveLoading(); });
    }

    private void FinishSaveLoading()
    {
        LoadScreen.Instance?.LoadScene(_selectedSave.saveSceneIndex);
        GameStateManager.Instance?.LoadGame(_selectedSave);

    }

    public void DeleteSelectedSave()
    {
        GameStateManager.Instance.DeleteGameSave(_selectedSave.ID);

        _selectedSave = null;
        _selectedSaveView.SetUp(null);

        UpdateSaves();
    }
}
