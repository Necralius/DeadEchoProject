using Assets.SimpleLocalization.Scripts;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static NekraByte.Core.DataTypes;

public class MenuSystem : MonoBehaviour
{
    //Volume Data
    [Header("Audio System")]
    [SerializeField] private List<SliderFloatField> _volumesSliders = new List<SliderFloatField>();
    [SerializeField] private Transform _volumeSlidersContent = null;
    [SerializeField] private GameObject SliderFloatFieldPrefab = null;

    #region - Graphics Data -
    [Header("Graphics System")]
    [SerializeField] Toggle vSyncActive                         = null;
    [SerializeField] Toggle fullscreenActive                    = null;

    private List<Resolution> resolutions                            = new List<Resolution>();
    private int currentResolutionIndex                              = 0;
    private Resolution currentResolution;
    [SerializeField] private DropsideOption resolutionDrp             = null;

    [SerializeField] private DropsideOption vSyncDrp                 = null;
    [SerializeField] private TMP_Dropdown   presetQualityDrp         = null;
    [SerializeField] private DropsideOption textureQualityDrp        = null;
    [SerializeField] private DropsideOption antialisingQualityDrp    = null;
    [SerializeField] private DropsideOption anisotropicQualityDrp    = null;
    [SerializeField] private DropsideOption shadowQualityDrp         = null;
    [SerializeField] private DropsideOption shadowResolutionDrp      = null;
    #endregion

    #region - Gameplay Data -
    [Header("Gameplay Settings")]
    [SerializeField] private Toggle_OnOFF   _tgl_InvertedX       = null;
    [SerializeField] private Toggle_OnOFF   _tgl_InvertedY       = null;
    [SerializeField] private Toggle_OnOFF   _subtitleState       = null;

    [SerializeField] private DropsideOption _aimDropside         = null;
    [SerializeField] private DropsideOption _crouchDropside      = null;
    [SerializeField] private DropsideOption _subtitleLanguage    = null;
    [SerializeField] private DropsideOption _audioLanguage       = null;

    [SerializeField] private SliderFloatField _sensitivityX          = null;
    [SerializeField] private SliderFloatField _sensitivityY          = null;
    #endregion

    [SerializeField] private bool _debug;

    GameStateManager _gameStateManager = null;
    
    private void Awake()
    {
        _gameStateManager = GameStateManager.Instance;
    }

    private void Start()
    {
        resolutions         = Screen.resolutions.ToList();

        currentResolution.width     = 1920;
        currentResolution.height    = 1080;

        Screen.SetResolution(currentResolution.width, currentResolution.height, fullscreenActive.isOn);
      
        LoadSettings();
    }

    #region - Volume System -
    // ----------------------------------------------------------------------
    // Name: LoadVolumeSettings (Method)
    // Desc: This method
    // ----------------------------------------------------------------------
    public void LoadVolumeSettings()
    {
        if (_gameStateManager == null || _gameStateManager.currentApplicationData == null) 
            return;

        if (_gameStateManager.currentApplicationData._volumes.Count <= 0)
        {
            _gameStateManager.currentApplicationData._volumes.Clear();
            foreach (Transform child in _volumeSlidersContent) 
                Destroy(child.gameObject);

            foreach (var track in AudioManager.Instance._tracks)
            {
                SliderFloatField obj = Instantiate(SliderFloatFieldPrefab, _volumeSlidersContent).GetComponentInChildren<SliderFloatField>();
                AudioTrackVolume trackVolumeInfo = new AudioTrackVolume(track.Key, DecibelsToVolume(AudioManager.Instance.GetTrackVolume(obj._fieldName)));
                trackVolumeInfo.Volume = 1f;

                obj.SetUp(trackVolumeInfo);
                _volumesSliders.Add(obj);

                _gameStateManager.currentApplicationData._volumes.Add(trackVolumeInfo);
                _gameStateManager.SaveApplicationData();
            }
        }
        else if (_volumesSliders.Count <= 0)
        {
            foreach (Transform child in _volumeSlidersContent) Destroy(child.gameObject);

            for (int i = 0; i < _gameStateManager.currentApplicationData._volumes.Count; i++)
            {
                SliderFloatField obj = Instantiate(SliderFloatFieldPrefab, _volumeSlidersContent).GetComponentInChildren<SliderFloatField>();
                obj.SetUp(_gameStateManager.currentApplicationData._volumes[i]);
                _volumesSliders.Add(obj);
            }
        }

        SetVolumeData();
    }

    // ----------------------------------------------------------------------
    // Name: SetVolumeData (Method)
    // Desc: This method sets the volume on the mixer, gettting the saved
    //       volume data from the current application data.
    // ----------------------------------------------------------------------
    public void SetVolumeData()
    {
        for (int i = 0; i < _volumesSliders.Count; i++) 
            AudioManager.Instance.SetTrackVolume(_volumesSliders[i]._fieldName, ConvertToDecibels(_volumesSliders[i].Value));
    }

    public void LoadVolumeDataFromFile()
    {
        if (_gameStateManager == null || _gameStateManager.currentApplicationData == null) return;

        ApplicationData currentAppData = _gameStateManager.currentApplicationData;

        for (int i = 0; i < currentAppData._volumes.Count; i++) 
            AudioManager.Instance.SetTrackVolume(currentAppData._volumes[i].Name, ConvertToDecibels(currentAppData._volumes[i].Volume));

        if (_debug)
            Debug.Log("Loading volume settings!");

        SetVolumeData();
    }

    // ----------------------------------------------------------------------
    // Name: SaveVolume (Method)
    // Desc: this method saves the current volumes in the volumes sliders,
    //       encapsulating them in the the current application data.
    // ----------------------------------------------------------------------
    public void SaveVolume()
    {
        for (int i = 0; i < _volumesSliders.Count; i++) 
            _gameStateManager.currentApplicationData._volumes[i].Volume = _volumesSliders[i].Value;
       
        _gameStateManager.SaveApplicationData();

        SetVolumeData();
    }

    // ----------------------------------------------------------------------
    // Name: ConvertToDecibels (Method)
    // Desc: This method convert an range of 1.0f to 0.1f to decibels from an
    //       range that starts from -80 DB to +10 DB.
    // ----------------------------------------------------------------------
    public float ConvertToDecibels(float Volume)
    {
        if (Volume < 0 || Volume > 1) return 0;

        int decibels = Mathf.FloorToInt((Volume * 100) - 100);
        //Debug.Log($" Converting to decibels: Volume: {Volume}, Decibels: {decibels}"); -> Debug Line
        return decibels;
    }

    // ----------------------------------------------------------------------
    // Name: DecibelsToVolume (Method)
    // Desc: This method converts decibels to an range of 1.0 to 0.1f.
    // ----------------------------------------------------------------------
    public float DecibelsToVolume(float Decibels)
    {
        float volume = (Decibels + 100) / 100;
        //Debug.Log($" DTV -> Volume: {volume} , Decibels: {Decibels}"); -> Debug Line
        return volume;
    }

    // ----------------------------------------------------------------------
    // Name: ResetVolumeSettings (Methods)
    // Desc: This method resets the volume settings on the game application
    //       data, and on the UI.
    // ----------------------------------------------------------------------
    public void ResetVolumeSettings()
    {
        _volumesSliders.Clear();
        foreach (Transform child in _volumeSlidersContent) 
            Destroy(child.gameObject);

        _gameStateManager.currentApplicationData.ResetVolumeSettings();

        LoadVolumeSettings();
    }
    #endregion

    #region - Graphics Settings -

    #region - Resolution Settings -
    private void UpdateResolutionDrpd()
    {
        resolutionDrp.ClearOptions();

        List<string> options = new();

        for (int i = 0; i < resolutions.Count; i++)
        {
            string newOption = resolutions[i].width + "x" + resolutions[i].height;
          
            options.Add(newOption);
        }

        resolutionDrp.AddCostumOptions(options);
    }

    private void VerifyResolution()
    {
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex  = i;
                currentResolution       = resolutions[i];
                resolutionDrp._snapController.GoToPanel(i);
            }
        }
    }
    #endregion

    public void SetAndSaveGraphicsSettings()
    {
        #region - Set -
        Screen.SetResolution(currentResolution.width, currentResolution.height, fullscreenActive.isOn);
        QualitySettings.SetQualityLevel(presetQualityDrp.value);

        QualitySettings.shadows                     = (ShadowQuality)shadowQualityDrp.Value;
        QualitySettings.shadowResolution            = (ShadowResolution)shadowResolutionDrp.Value;
        QualitySettings.anisotropicFiltering        = (AnisotropicFiltering)anisotropicQualityDrp.Value;
        QualitySettings.antiAliasing                = antialisingQualityDrp.Value;
        QualitySettings.globalTextureMipmapLimit    = textureQualityDrp.Value;

        if (vSyncActive) QualitySettings.vSyncCount = vSyncDrp.Value;
        else QualitySettings.vSyncCount = 0;
        #endregion

        #region - Save -
        if (_gameStateManager.currentApplicationData == null) 
            return;

        _gameStateManager.currentApplicationData.currentResolution      = currentResolution;
        _gameStateManager.currentApplicationData.resolutionIndex        = currentResolutionIndex;
        _gameStateManager.currentApplicationData.isFullscreen           = fullscreenActive.isOn;

        _gameStateManager.currentApplicationData.vSyncActive            = vSyncActive.isOn;
        _gameStateManager.currentApplicationData.vSyncCount             = vSyncDrp.Value;

        _gameStateManager.currentApplicationData.shadowQuality          = shadowQualityDrp.Value;
        _gameStateManager.currentApplicationData.shadowResolution       = shadowResolutionDrp.Value;

        _gameStateManager.currentApplicationData.antialiasing           = antialisingQualityDrp.Value;
        _gameStateManager.currentApplicationData.anisotropicFiltering   = anisotropicQualityDrp.Value;

        _gameStateManager.currentApplicationData.qualityLevelIndex      = presetQualityDrp.value;

        _gameStateManager.SaveApplicationData();
        LoadSettings();
        #endregion
    }

    public void ResetGraphicsSettings()
    {
        _gameStateManager.currentApplicationData.ResetGraphicsSettings();
        _gameStateManager.SaveApplicationData();

        UpdateGraphicsUI();
    }

    private void LoadGraphicsSettings()
    {
        // Settings Load and Set
        Screen.SetResolution(_gameStateManager.currentApplicationData.currentResolution.width,
            _gameStateManager.currentApplicationData.currentResolution.height, _gameStateManager.currentApplicationData.isFullscreen);

        QualitySettings.SetQualityLevel(_gameStateManager.currentApplicationData.qualityLevelIndex);

        QualitySettings.shadows = (ShadowQuality)_gameStateManager.currentApplicationData.shadowQuality;
        QualitySettings.shadowResolution = (ShadowResolution)_gameStateManager.currentApplicationData.shadowResolution;
        QualitySettings.anisotropicFiltering = (AnisotropicFiltering)_gameStateManager.currentApplicationData.anisotropicFiltering;
        QualitySettings.antiAliasing = _gameStateManager.currentApplicationData.antialiasing;

        QualitySettings.vSyncCount = _gameStateManager.currentApplicationData.vSyncCount;

        if (_debug) 
            Debug.Log("Loading graphics settings!");
    }

    public void UpdateGraphicsUI()
    {
        UpdateResolutionDrpd();
        resolutionDrp._snapController.GoToPanel(resolutionDrp._snapController.NumberOfPanels - 1);

        if (_gameStateManager.currentApplicationData == null)
            return;

        resolutionDrp.Value         = _gameStateManager.currentApplicationData.resolutionIndex;

        vSyncActive.isOn            = _gameStateManager.currentApplicationData.vSyncActive;
        vSyncDrp.Value              = _gameStateManager.currentApplicationData.vSyncCount;

        presetQualityDrp.value      = _gameStateManager.currentApplicationData.qualityLevelIndex;

        shadowQualityDrp.Value      = _gameStateManager.currentApplicationData.shadowQuality;
        shadowResolutionDrp.Value   = _gameStateManager.currentApplicationData.shadowResolution;

        antialisingQualityDrp.Value = _gameStateManager.currentApplicationData.antialiasing;
        anisotropicQualityDrp.Value = _gameStateManager.currentApplicationData.anisotropicFiltering;      
    }
    #endregion

    #region - Load General Settings -

    // ----------------------------------------------------------------------
    // Name: LoadSettings
    // Desc: This method load all settings founded on the local serialized
    //       data archieve.
    // ----------------------------------------------------------------------
    public void LoadSettings()
    {
        if (_gameStateManager is not null && _gameStateManager.currentApplicationData is not null)
        {
            LoadGraphicsSettings();
            LoadVolumeDataFromFile();
            SetVolumeData();
            UpdateGameplaySettings();
        }
    }
    #endregion

    #region - Gameplay Settings -
    // -----------------------------------------------------------------------
    // Name: SetAndSaveGameplaySettings
    // Desc: This method set the current gameplay settings and save it in an
    //       serialized file.
    // ----------------------------------------------------------------------
    public void SetAndSaveGameplaySettings()
    {
        if (_gameStateManager == null) 
            return;

        _gameStateManager.currentApplicationData.invertX        = _tgl_InvertedX.State;
        _gameStateManager.currentApplicationData.invertY        = _tgl_InvertedY.State;

        _gameStateManager.currentApplicationData.xSensitivity   = _sensitivityX.Value;
        _gameStateManager.currentApplicationData.ySensitivity   = _sensitivityY.Value;

        _gameStateManager.currentApplicationData.aimType        = _aimDropside.Value;
        _gameStateManager.currentApplicationData.crouchType     = _crouchDropside.Value;

        _gameStateManager.currentApplicationData.subtitlesLanguage  = _subtitleLanguage.Value;
        _gameStateManager.currentApplicationData.audioLanguage      = _audioLanguage.Value;
        _gameStateManager.currentApplicationData.subtitles          = _subtitleState.State;

        LocalizationManager.Language = _subtitleLanguage.ContentValue;

        _gameStateManager.SaveApplicationData();

        UpdateGameplaySettings();
    } 

    // ----------------------------------------------------------------------
    // Name: UpdateGameplaySettings
    // Desc: This method updates the gameplay settings tab UI, using the data
    //       saved on the serialized file.
    // ----------------------------------------------------------------------
    public void UpdateGameplaySettings()
    {
        if (_gameStateManager == null)
            return;

        _tgl_InvertedX.State = _gameStateManager.currentApplicationData.invertX;
        _tgl_InvertedY.State = _gameStateManager.currentApplicationData.invertY;
        _subtitleState.State = _gameStateManager.currentApplicationData.subtitles;

        _sensitivityX.OverrideValue(_gameStateManager.currentApplicationData.xSensitivity);
        _sensitivityY.OverrideValue(_gameStateManager.currentApplicationData.ySensitivity);
        
        _aimDropside.Value          = _gameStateManager.currentApplicationData.aimType;
        _crouchDropside.Value       = _gameStateManager.currentApplicationData.crouchType;
        _subtitleLanguage.Value     = _gameStateManager.currentApplicationData.subtitlesLanguage;
        _audioLanguage.Value        = _gameStateManager.currentApplicationData.audioLanguage;

        LocalizationManager.Language = _subtitleLanguage.ContentValue;
    }

    // ----------------------------------------------------------------------
    // Name: ResetGameplaySettings (Method)
    // Desc: This method resets the gameplay settings on the application dada
    //       file.
    // ----------------------------------------------------------------------
    public void ResetGameplaySettings()
    {
        _gameStateManager.currentApplicationData.ResetGameplaySettings();
        _gameStateManager.SaveApplicationData();

        UpdateGameplaySettings();
    }
    #endregion

    #region - Game Data System -
    public void SaveGameData() => GameStateManager.Instance.SaveGameData();

    #endregion

    public void SetMusicEvent(string eventTag)
    {       
        AudioManager.Instance.CallMusicEvent(eventTag);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void QuitToMenu()
    {
        LoadScreen.Instance.LoadScene("Scene_MainMenu");
    }

    public void RespawnFromLastSave()
    {
        FadeSystemManager.Instance.CallFadeAction(delegate { FinishSaveLoading(); });
    }

    private void FinishSaveLoading()
    {
        //SceneLoader.Instance.FinishSceneLoad("Scene_Level1");
        SceneManager.LoadScene("Scene_Level1");
        GameStateManager.Instance?.LoadFromLastSave();
    }
}