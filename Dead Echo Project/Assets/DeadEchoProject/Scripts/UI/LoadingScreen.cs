using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    #region - Singleton Pattern -
    public static LoadingScreen Instance;
    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;
    }
    #endregion

    [SerializeField] private MenuManager    _menuManager = null;
    [SerializeField] private CameraManager  _cameraManager = null;
    [SerializeField] private GameObject     _loadingLogo = null;
    [SerializeField] private Slider         _loadingSlider = null;

    [SerializeField] private UnityEvent _onFinish = null;

    private void Start()
    {
        _menuManager    = FindFirstObjectByType<MenuManager>();
        _cameraManager  = FindFirstObjectByType<CameraManager>();
    }


    public UnityEvent OnFinish
    {
        get
        {
            _onFinish.RemoveAllListeners(); 
            return _onFinish;
        }
    }

    public void OpenMenuLoadingScreen(UnityAction action, float loadingMaxTime)
    {
        _menuManager.OpenMenu("LoadingScreen", delegate { _loadingLogo.SetActive(true); });
        _cameraManager.SetCameraPriority("LoadingCam");
        _onFinish.AddListener(action);

        StartCoroutine(FinishLoading(loadingMaxTime));
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
        FadeSystemManager.Instance.CallFadeAction(delegate { _onFinish.Invoke(); });
    }

}