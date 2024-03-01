using System;

namespace Code.Core.MVP
{
public interface IPresenter : IDisposable
{
    protected internal IModel Model { get; }
    protected internal IView View { get; }
}
}