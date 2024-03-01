using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Code.Core.Config
{
public interface IConfigParser : IDisposable
{
    public void ParseConfig();
    public UniTask ParseConfigAsync(CancellationToken token);
}
}