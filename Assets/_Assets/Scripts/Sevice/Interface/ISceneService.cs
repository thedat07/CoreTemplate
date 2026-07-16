using System;
using UnityEngine.SceneManagement;

public interface ISceneService
{
    string CurrentSceneName { get; }
    int CurrentSceneIndex { get; }

    void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, Action onLoaded = null);
    void LoadScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, Action onLoaded = null);
    void LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, Action onLoaded = null, Action<float> onProgress = null);
    void LoadSceneAsync(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single, Action onLoaded = null, Action<float> onProgress = null);
    void ReloadCurrentScene(LoadSceneMode mode = LoadSceneMode.Single);
    void UnloadScene(string sceneName);
}