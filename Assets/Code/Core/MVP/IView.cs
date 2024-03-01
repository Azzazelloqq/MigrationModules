using System;

namespace Code.Core.MVP
{
public interface IView : IDisposable
{
    protected internal IPresenter Presenter { get; }
}
}