using System;
using Cysharp.Threading.Tasks;

namespace Code.Core.Root
{
public interface IRoot : IDisposable
{
    public UniTask Initialize();
}
}