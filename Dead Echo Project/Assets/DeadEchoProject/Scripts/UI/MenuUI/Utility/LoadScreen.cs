using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Analytics;

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

    private void Start()
    {
        
    }

    public void UpdateState(float loadingProgress)
    {
        float progress      = Mathf.Clamp01(loadingProgress / 0.9f);
        loadingSlider.value = (progress * 100);
    }

    public void LoadScene(string sceneName) => SceneLoader.Instance.FinishSceneLoad(sceneName);
    public void LoadScene(int sceneIndex) =>   SceneLoader.Instance.FinishSceneLoad(sceneIndex);

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