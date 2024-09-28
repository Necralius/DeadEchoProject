using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private List<SceneAsset> _scenesLoaded = new List<SceneAsset>();

    [SerializeField] private List<string> _scenesToLoad = new List<string>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _scenesToLoad.ForEach(e => { PreLoadScene(e); });
    }

    public void FinishSceneLoad(string sceneName)
       => _scenesLoaded.Find(e => (e.Data.name == sceneName)).LoadData.allowSceneActivation = true;

    public void FinishSceneLoad(int sceneIndex)
        => _scenesLoaded.Find(e => (e.Data.buildIndex == sceneIndex)).LoadData.allowSceneActivation = true;

    public void PreLoadScene(string sceneName) => StartCoroutine(C_PreLoadScene(sceneName));
    private IEnumerator C_PreLoadScene(string sceneName)
    {
        AsyncOperation opr       = SceneManager.LoadSceneAsync(sceneName);
        opr.allowSceneActivation = false;

        SceneAsset sceneAsset    = null;

        _scenesLoaded.Add(
            sceneAsset = new SceneAsset()
            {
                Data = SceneManager.GetSceneByName(sceneName),
                LoadData = opr
            });
        sceneAsset.OnSceneStartLoad?.Invoke();

        while (!opr.isDone)
        {
            if (opr.progress >= 0.9f)
            {
                sceneAsset.OnSceneFinishLoad?.Invoke();
                Debug.Log($"Scene {sceneName} has been loaded!");
                yield break;
            }
            yield return null;
        }
    }
}

[Serializable]
public class SceneAsset
{
    public AsyncOperation LoadData;
    public Scene Data;
    public UnityEvent OnSceneFinishLoad;
    public UnityEvent OnSceneStartLoad;
}