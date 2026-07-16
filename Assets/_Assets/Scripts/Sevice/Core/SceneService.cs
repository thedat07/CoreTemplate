using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * SceneService — Quản lý load/unload scene.
 * - Load sync / async với callback
 * - Fire EventBus cho toàn bộ hệ thống
 * - Hỗ trợ progress tracking cho UI loading bar
 */

public class SceneService : ISceneService
{
    private MonoBehaviour coroutineRunner;

    private MonoBehaviour CoroutineRunner
    {
        get
        {
            if (coroutineRunner == null)
            {
                var go = new GameObject("[SceneServiceRunner]");
                UnityEngine.Object.DontDestroyOnLoad(go);
                coroutineRunner = go.AddComponent<SceneRunnerBehaviour>();
            }
            return coroutineRunner;
        }
    }

    public string CurrentSceneName => SceneManager.GetActiveScene().name;
    public int CurrentSceneIndex => SceneManager.GetActiveScene().buildIndex;

    // ----- Sync Load -----

    public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, Action onLoaded = null)
    {
        FireLoading(sceneName, mode);

        SceneManager.LoadScene(sceneName, mode);

        FireLoaded(sceneName, mode);
        onLoaded?.Invoke();
    }

    public void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, Action onLoaded = null)
    {
        string name = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
        if (string.IsNullOrEmpty(name)) name = $"Index_{sceneIndex}";

        FireLoading(name, mode);
        SceneManager.LoadScene(sceneIndex, mode);
        FireLoaded(name, mode);
        onLoaded?.Invoke();
    }

    // ----- Async Load -----

    public void LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, Action onLoaded = null, Action<float> onProgress = null)
    {
        CoroutineRunner.StartCoroutine(LoadAsyncCoroutine(sceneName, mode, onLoaded, onProgress));
    }

    public void LoadSceneAsync(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, Action onLoaded = null, Action<float> onProgress = null)
    {
        string name = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
        if (string.IsNullOrEmpty(name)) name = $"Index_{sceneIndex}";

        CoroutineRunner.StartCoroutine(LoadAsyncCoroutine(name, mode, onLoaded, onProgress, sceneIndex));
    }

    // ----- Utility -----

    public void ReloadCurrentScene(LoadSceneMode mode = LoadSceneMode.Single)
    {
        LoadScene(CurrentSceneName, mode);
    }

    public void UnloadScene(string sceneName)
    {
        var scene = SceneManager.GetSceneByName(sceneName);
        if (!scene.isLoaded) return;

        SceneManager.UnloadSceneAsync(sceneName);

        EventBus<SceneUnloadedEvent>.Raise(new SceneUnloadedEvent
        {
            SceneName = sceneName
        });
    }

    // ----- Coroutine -----

    private IEnumerator LoadAsyncCoroutine(string sceneName, LoadSceneMode mode, Action onLoaded, Action<float> onProgress, int? buildIndex = null)
    {
        FireLoading(sceneName, mode);

        AsyncOperation operation = buildIndex.HasValue
            ? SceneManager.LoadSceneAsync(buildIndex.Value, mode)
            : SceneManager.LoadSceneAsync(sceneName, mode);

        if (operation == null) yield break;

        operation.allowSceneActivation = true;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            onProgress?.Invoke(progress);

            EventBus<SceneProgressEvent>.Raise(new SceneProgressEvent
            {
                SceneName = sceneName,
                Progress = progress
            });

            yield return null;
        }

        FireLoaded(sceneName, mode);
        onLoaded?.Invoke();
    }

    // ----- Event Helpers -----

    private void FireLoading(string sceneName, LoadSceneMode mode)
    {
        EventBus<SceneLoadingEvent>.Raise(new SceneLoadingEvent
        {
            SceneName = sceneName,
            Mode = mode
        });
    }

    private void FireLoaded(string sceneName, LoadSceneMode mode)
    {
        EventBus<SceneLoadedEvent>.Raise(new SceneLoadedEvent
        {
            SceneName = sceneName,
            Mode = mode
        });
    }
}

/// <summary>Minimal MonoBehaviour to run SceneService coroutines.</summary>
internal class SceneRunnerBehaviour : MonoBehaviour { }