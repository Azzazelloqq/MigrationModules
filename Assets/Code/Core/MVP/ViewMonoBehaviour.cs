using UnityEngine;

namespace Code.Core.MVP
{
public abstract class ViewMonoBehaviour<TPresenter> : MonoBehaviour, IView where TPresenter : IPresenter
{
    IPresenter IView.Presenter => presenter;

    protected bool isDestroyed;
    protected bool isDisposed;
    protected TPresenter presenter;

    #region AbstractMethods

    public virtual void Initialize(TPresenter basePresenter)
    {
        presenter = basePresenter;
    }
    
    #endregion

    #region VirtualMethods

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }
        
        OnDispose();
        
        if (!isDestroyed)
        {
            Destroy(gameObject);
        }

        isDisposed = true;
    }

    protected virtual void OnDispose()
    {
    }

    protected virtual void OnDestroy()
    {
        isDestroyed = true;
    }

    #endregion
}
}