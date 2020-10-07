﻿using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;

/// <summary>
/// Bootstrapper for initializing the OpenID Connect service for the learning layers provider
/// </summary>
public class LearningLayersBootstrapper : BaseServiceBootstrapper
{
    protected override void RegisterServices()
    {
        OpenIDConnectService oidc = new OpenIDConnectService();
        oidc.OidcProvider = new LearningLayersOIDCProvider();
        // this example shows how the service can be used on an app for multiple platforms
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
