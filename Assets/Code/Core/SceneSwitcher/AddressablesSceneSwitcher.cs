using System;
using Code.Core.Logger;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Code.Core.SceneSwitcher
{
public class AddressablesSceneSwitcher : ISceneSwitcher
{
    public event Action<string> SceneStartedToSwitch;
    public event Action<string> SceneSwitched;

    private readonly IInGameLogger _inGameLogger;

    public AddressablesSceneSwitcher(IInGameLogger inGameLogger)
    {
        _inGameLogger = inGameLogger;
    }

    public void Dispose()
    {
        SceneSwitched = null;
        SceneStartedToSwitch = null;
    }

    public TContext SwitchToScene<TContext>(string sceneId, LoadSceneMode sceneMode = LoadSceneMode.Single)
        where TContext : ISceneContext
    {
        var loadOperation = Addressables.LoadSceneAsync(sceneId, sceneMode);

        loadOperation.WaitForCompletion();

        var rootGameObjects = loadOperation.Result.Scene.GetRootGameObjects();
        var sceneContext = GetSceneContext<TContext>(rootGameObjects, sceneId);

        return sceneContext;
    }

    public async UniTask<TContext> SwitchToSceneAsync<TContext>(string sceneId,
        LoadSceneMode sceneMode = LoadSceneMode.Single) where TContext : ISceneContext
    {
        var sceneInstance = await Addressables.LoadSceneAsync(sceneId, sceneMode).Task.AsUniTask();

        var rootGameObjects = sceneInstance.Scene.GetRootGameObjects();
        var sceneContext = GetSceneContext<TContext>(rootGameObjects, sceneId);

        return sceneContext;
    }

    public void SwitchToScene(string sceneId, LoadSceneMode sceneMode)
    {
        var loadOperation = Addressables.LoadSceneAsync(sceneId, sceneMode);
    }

    public void OnSceneStartedToSwitch(string sceneId)
    {
        SceneStartedToSwitch?.Invoke(sceneId);
    }

    public void OnSceneSwitched(string sceneId)
    {
        SceneSwitched?.Invoke(sceneId);
    }

    private T GetSceneContext<T>(GameObject[] rootObjects, string sceneId)
    {
        foreach (var rootGameObject in rootObjects)
        {
            if (rootGameObject.TryGetComponent(out T sceneContext))
            {
                return sceneContext;
            }
        }

        _inGameLogger.LogError($"Scene {sceneId} does not have a sceneContext {typeof(T)}");
        return default;
    }
}
}