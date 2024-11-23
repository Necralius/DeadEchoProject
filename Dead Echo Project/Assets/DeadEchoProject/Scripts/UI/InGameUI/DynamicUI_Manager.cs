using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicUI_Manager : MonoBehaviour
{
    #region - Singleton Pattern -
    public static DynamicUI_Manager Instance;
    private void Awake()
    {
        if (Instance != null) 
            Destroy(Instance);
        Instance = this;
    }
    #endregion

    [SerializeField] private List<FaderItem> _faders = new List<FaderItem>();

    [SerializeField] private CanvasGroup     _cg      = null;
    [SerializeField] private TextMeshProUGUI _ammo    = null;
    [SerializeField] private TextMeshProUGUI _gunName = null;
    [SerializeField] private TextMeshProUGUI _gunMode = null;
    [SerializeField] private Image           _gunIcon = null;

    FaderItem _uiFader = null;

    public void UpdateGunStats(GunBase gunData)
    {
        if (gunData == null)
        {
            _ammo.text      = "";
            _gunName.text   = "None";
            _gunMode.text   = "";
            _gunIcon.sprite = null;
            _gunIcon.color  = new Color(0, 0, 0, 0);
            return;
        }

        _ammo.text      = gunData.GunAmmo;
        _gunName.text   = gunData.GunData.gunData.gunName;
        _gunMode.text   = gunData.GunData.gunData.gunMode.ToString();
        _gunIcon.sprite = gunData.GunData.gunData.Icon;
        _gunIcon.color  = Color.white;
    }

    private void Start()
    {
        _cg      = GetComponent<CanvasGroup>();
        GetFader("PlayerCanvas", Color.white);
    }

    private void Update()
    {
        if (_uiFader != null && _cg != null)
            _cg.alpha = GetFader("PlayerCanvas", Color.white).Progress;

        foreach (var fader in _faders) 
            fader.data.OnUpdate();
    }

    public ColorFader GetFader(string Tag, Color defaultColor)
    {
        foreach(var fader in _faders) 
            if (fader.faderTag == Tag) 
                return fader.data;

        FaderItem newItem = new FaderItem(Tag, defaultColor);
        _faders.Add(newItem);
        return newItem.data;
    }
}

[Serializable]
public class FaderItem
{
    public string faderTag;
    public ColorFader data;

    public FaderItem(string Tag, Color defaultColor)
    {
        this.faderTag       = Tag;
        data                = new ColorFader();
        data.currentColor   = defaultColor;
    }
}

[Serializable]
public class ColorFader
{
    public Color currentColor = Color.white;
    [Range(0f,1f)] public float Progress = 0;

    [SerializeField, Range(0,10f)] private float    Speed       = 5f;
    [SerializeField] private    bool                Enabled     = false;
    [SerializeField] private    float               targetValue = 0;

    private bool  _fadeIn = false;

    private bool  _useTimer     = false;
    private float _timer        = 0f;
    private float _timeFadeOut  = 1f;

    public void FadeIn(float timeToFadeOut)
    {
        targetValue     = 1;
        _timer          = 0f;

        _timeFadeOut    = timeToFadeOut;
        _useTimer       = true;

        Enabled         = true;
        _fadeIn         = true;
    }

    public void FadeIn()
    {
        targetValue = 1;
        _timer      = 0f;
        _useTimer   = false;

        Enabled = true;
        _fadeIn = true;
    }
    public void FadeOut()
    {
        targetValue = 0;
        Enabled     = true;
        _fadeIn     = false;
    }

    public void OnUpdate()
    {
        if (_useTimer)
        {
            if (_timer >= _timeFadeOut) FadeOut();
            else _timer += Time.deltaTime;
        }

        if (Enabled)
        {
            Progress = Mathf.Lerp(Progress, targetValue, Speed * Time.deltaTime);

            if (_fadeIn)
            {
                if (Progress >= 1f) 
                    Enabled = false;
            }
            else if (Progress <= 0.05f) 
                Enabled = false;

            currentColor.a = Progress;
        }
    }
}