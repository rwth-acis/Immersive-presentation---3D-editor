using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;

public class OpenIDConnectBootstrapper : BaseServiceBootstrapper
{
    protected override void RegisterServices()
    {
        OpenIDConnectService oidc = new OpenIDConnectService();
        oidc.OidcProvider = new LearningLayersOidcProvider();
#if UNITY_WSA
        oidc.RedirectURI = "i5:/";
#else
        oidc.RedirectURI = "https://www.google.com";
#endif
        ServiceManager.RegisterService(oidc);
    }

    protected override void UnRegisterServices()
    {
        ServiceManager.RemoveService<OpenIDConnectService>();
    }
}
