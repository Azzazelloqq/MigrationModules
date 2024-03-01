using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Core.SceneSwitcher
{
public interface IScene : IDisposable
{
    public string SceneId { get; }
    protected internal ISceneContext SceneContext { get; }
    protected internal ISceneSwitcher SceneSwitcher { get; }

    public UniTask InitializeAsync(CancellationToken token);
}
}