using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Core.Utils.AsyncSubscribe
{
public static class ButtonExtensions
{
    public static IDisposable SubscribeClickAsync(this Button button, Func<CancellationToken, UniTask> onClickCallback, CancellationToken token = default)
    {
        var tcs = new UniTaskCompletionSource();

        async void OnClickHandler()
        {
            if (token.IsCancellationRequested)
            {
                button.onClick.RemoveListener(OnClickHandler);
                tcs.TrySetCanceled();
                return;
            }

            try
            {
                await onClickCallback(token);
                tcs.TrySetResult();
            }
            catch (Exception e)
            {
                if (e is not OperationCanceledException)
                {
                    Debug.LogException(e);
                }
                tcs.TrySetException(e);
            }
        }

        button.onClick.AddListener(OnClickHandler);

        return new Unsubscriber(() =>
        {
            button.onClick.RemoveListener(OnClickHandler);
            tcs.TrySetCanceled();
        });
    }
}
}