using System.Threading.Tasks;
using Code.Core.Logger;
using Facebook.Unity;

namespace Code.Core.Facebook
{
public class FacebookSDKService : IFacebookSDKService
{
    private readonly IInGameLogger _logger;

    public FacebookSDKService(IInGameLogger logger)
    {
        _logger = logger;
    }
    
    public async Task Initialize()
    {
        FB.Init();
        
        while (!FB.IsInitialized)
        {
            await Task.Yield();
        }
        
        FB.ActivateApp();
        
        _logger.Log("[Facebook] Facebook initialized");
    }

    public void Dispose()
    {
    }
}
}