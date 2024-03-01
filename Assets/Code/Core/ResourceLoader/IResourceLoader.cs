using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Code.Core.ResourceLoader
{
public interface IResourceLoader : IDisposable
{
    public UniTaskVoid PreloadInCash<TResource>(string resourceId);
    public TResource LoadResource<TResource>(string resourceId);
    public void LoadResource<TResource>(string resourceId, Action<TResource> onResourceLoaded, CancellationToken token);
    public UniTask<TResource> LoadResourceAsync<TResource>(string resourceId, CancellationToken token);
    public void ReleaseResource<TResource>(TResource resource);
    public void ReleaseAllResources();
}
}