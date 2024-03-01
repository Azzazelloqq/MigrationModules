using System;
using Cysharp.Threading.Tasks;

namespace Code.Core.SceneSwitcher.Factory
{
public interface ISceneFactory : IDisposable
{
    public UniTask<TScene> CreateSceneAsync<TScene>() where TScene : IScene;
}
}