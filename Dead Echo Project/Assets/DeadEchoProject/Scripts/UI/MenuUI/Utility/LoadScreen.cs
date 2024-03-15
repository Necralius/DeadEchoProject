using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreen : MonoBehaviour
{
    #region - Singleton Pattern -
    public static LoadScreen Instance;
    private void Awake()
    {
        if (Instance != null) Destroy(Instance.gameObject);
        Instance = this;      
    }
    #endregion

    [SerializeField] private Slider loadingSlider;
    [SerializeField] private float loadingProgress;
    [SerializeField] private GameObject loadingScreen;

    public void UpdateState(float loadingProgress)
    {
        float progress      = Mathf.Clamp01(loadingProgress / 0.9f);
        loadingSlider.value = (progress * 100);
    }

    public void LoadScene(string sceneName) => StartCoroutine(LoadSceneProcess(sceneName));
    public void LoadScene(int sceneIndex)   => StartCoroutine(LoadSceneProcess(sceneIndex));

    private IEnumerator LoadSceneProcess(string sceneName)
    {
        loadingScreen.gameObject.SetActive(true);

        AsyncOperation opr = SceneManager.LoadSceneAsync(sceneName);

        while (!opr.isDone)
        {
            loadingProgress = opr.progress;
            UpdateState(loadingProgress);
            yield return null;
        }

        loadingScreen.gameObject.SetActive(false);
    }

    private IEnumerator LoadSceneProcess(int sceneIndex)
    {
        loadingScreen.gameObject.SetActive(true);

        AsyncOperation opr = SceneManager.LoadSceneAsync(sceneIndex);

        while (!opr.isDone)
        {
            loadingProgress = opr.progress;
            UpdateState(loadingProgress);
            yield return null;
        }

        loadingScreen.gameObject.SetActive(false);
    }
}