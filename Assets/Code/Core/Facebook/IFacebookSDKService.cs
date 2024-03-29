using System;
using System.Threading.Tasks;

namespace Code.Core.Facebook
{
public interface IFacebookSDKService : IDisposable
{
    public Task Initialize();
}
}