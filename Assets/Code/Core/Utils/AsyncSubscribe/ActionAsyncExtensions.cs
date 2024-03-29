using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Core.Utils.AsyncSubscribe
{
public static class ActionAsyncExtensions
{
    public static IDisposable SubscribeAsync(this Action action, Func<CancellationToken, UniTask> asyncCallback, CancellationToken token)
    {
        var wrapper = new AsyncOperationWrapper(asyncCallback, token);

        Action wrappedAction = () => wrapper.InvokeAsync();

        action += wrappedAction;

        return new Unsubscriber(() => action -= wrappedAction);
    }

    private class AsyncOperationWrapper
    {
        private readonly Func<CancellationToken, UniTask> _asyncCallback;
        private readonly CancellationToken _token;

        public AsyncOperationWrapper(Func<CancellationToken, UniTask> asyncCallback, CancellationToken token)
        {
            _asyncCallback = asyncCallback;
            _token = token;
        }

        public async void InvokeAsync()
        {
            if (!_token.IsCancellationRequested)
            {
                await _asyncCallback(_token);
            }
        }
    }
}
}