using ApiResource = Duende.IdentityServer.Models.ApiResource;
using ApiScope = Duende.IdentityServer.Models.ApiScope;
using Client = Duende.IdentityServer.Models.Client;
using IdentityResource = Duende.IdentityServer.Models.IdentityResource;
using Secret = Duende.IdentityServer.Models.Secret;

namespace DevSync.PocPro.Identity.Data;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
    [
        new IdentityResources.OpenId(),
        new IdentityResources.Profile()
    ];

    public static IEnumerable<ApiScope> ApiScopes =>
    [ 
        new ApiScope("account.api"),
        new ApiScope("shop.api")
    ];

    public static IEnumerable<ApiResource> ApiResources =>
    [
        new ApiResource("account.api")
        {
            Scopes = { "account.api" },
            UserClaims = { "sub" },
            ApiSecrets = {new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256())}
        },
        
        new ApiResource("shop.api")
        {
            Scopes = { "shop.api", "account.api" },
            UserClaims = { "sub" },
            ApiSecrets = {new Secret("611536EF-F270-4058-80CA-1C89C192F69A".Sha256())}
        }
    ];

    public static IEnumerable<Client> Clients =>
    [
            new()
        {
                ClientId = "admin.client",
                ClientName = "Admin Client",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0") },
                ClientUri = "http://localhost:5173",

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "http://localhost:5173/callback" },
                FrontChannelLogoutUri = "http://localhost:5173",
                PostLogoutRedirectUris = { "http://localhost:5173" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "account.api", "shop.api" }
        }
    ];
}