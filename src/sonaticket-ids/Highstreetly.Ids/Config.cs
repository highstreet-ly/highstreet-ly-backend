using System;
using System.Collections.Generic;
using Highstreetly.Infrastructure;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace Highstreetly.Ids
{
    public class Config
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var customProfile = new IdentityResource(
                name: "custom.profile",
                displayName: "Custom Profile",
                userClaims: new[] { JwtClaimTypes.Role });

            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                customProfile
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource(Scopes.PermissionsApi, "Permissions API")
                {
                    Scopes = new[]{ Scopes.PermissionsApi },
                    UserClaims = new List<string>{
                        System.Security.Claims.ClaimTypes.Role,
                        "access-all"
                    },
                },
                new ApiResource(Scopes.PaymentApi, "Payments API")
                {
                    Scopes = new[]{ Scopes.PaymentApi },
                    UserClaims = new List<string>{
                        System.Security.Claims.ClaimTypes.Role,
                        "access-all"
                    },
                },
                new ApiResource(Scopes.TicketManagementApi, "Ticket Management API")
                {
                    Scopes = new[]{ Scopes.TicketManagementApi },
                    UserClaims = new List<string>{
                        System.Security.Claims.ClaimTypes.Role,
                        "access-all"
                    },
                },
                new ApiResource(Scopes.TicketReservationsApi, "Ticket Reservations API")
                {
                    Scopes = new[]{ Scopes.TicketReservationsApi },
                    UserClaims = new List<string>{
                        System.Security.Claims.ClaimTypes.Role,
                        "access-all"
                    },
                },
                new ApiResource(Scopes.MainApi, "Sonaticket API")
                {
                    Scopes = new[]{ Scopes.MainApi },
                    UserClaims = new List<string>{
                        System.Security.Claims.ClaimTypes.Role,
                        "access-all"
                    },
                },
                new ApiResource(Scopes.DashApi, "Sonaticket Dashboard API", new []{ System.Security.Claims.ClaimTypes.Role})
                {
                    UserClaims = new List<string>{
                        System.Security.Claims.ClaimTypes.Role
                    },
                    Scopes = new[]{ Scopes.DashApi }
                },
                new ApiResource(Scopes.TicketsApi, "Sonatribe Tickets API", new []{ System.Security.Claims.ClaimTypes.Role})
                {
                    UserClaims = new List<string>{
                        System.Security.Claims.ClaimTypes.Role
                    },
                    Scopes = new[]{ Scopes.TicketsApi }
                },
                new ApiResource(Scopes.OrdersApi, "Sonatribe Orders API", new []{ System.Security.Claims.ClaimTypes.Role})
                {
                    UserClaims = new List<string>{
                        System.Security.Claims.ClaimTypes.Role
                    },
                    Scopes = new[]{ Scopes.OrdersApi }
                },
                new ApiResource(Scopes.OperatorApi, "Sonatribe Operator API", new []{ System.Security.Claims.ClaimTypes.Role})
                {
                    UserClaims = new List<string>{
                        System.Security.Claims.ClaimTypes.Role
                    },
                    Scopes = new[]{ Scopes.OperatorApi }
                }
            };

        }

        public static IEnumerable<ApiScope> GetApiScopes() =>
            new List<ApiScope>
            {
                new ApiScope(Scopes.DashApi, Scopes.DashApi)
                {
                    UserClaims = {JwtClaimTypes.Audience},
                },
                new ApiScope(Scopes.PaymentApi, Scopes.PaymentApi)
                {
                    UserClaims = {JwtClaimTypes.Audience}
                },
                new ApiScope(Scopes.MainApi, Scopes.MainApi)
                {           
                    UserClaims = { JwtClaimTypes.Audience }
                },
                new ApiScope(Scopes.OrdersApi, Scopes.OrdersApi)
                {           
                    UserClaims = { JwtClaimTypes.Audience }
                },
                new ApiScope(Scopes.PermissionsApi, Scopes.PermissionsApi)
                {           
                    UserClaims = { JwtClaimTypes.Audience }
                },
                new ApiScope(Scopes.TicketManagementApi, Scopes.TicketManagementApi)
                {           
                    UserClaims = { JwtClaimTypes.Audience }
                },
                new ApiScope(Scopes.TicketsApi, Scopes.TicketsApi)
                {           
                    UserClaims = { JwtClaimTypes.Audience }
                },
                new ApiScope(Scopes.TicketReservationsApi, Scopes.TicketReservationsApi)
                {           
                    UserClaims = { JwtClaimTypes.Audience }
                },
                new ApiScope(Scopes.OperatorApi, Scopes.OperatorApi)
                {           
                    UserClaims = { JwtClaimTypes.Audience }
                },
            };
        
        
        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> Clients(IConfiguration configuration)
        {
            // client credentials client
            return new List<Client>
            {
                // payment service client
                new Client
                {
                    ClientId = Scopes.PaymentProcessor,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },
                    
                    AllowedScopes = { Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },
                new Client
                {
                    ClientId = Scopes.PaymentApi,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },

                    AllowedScopes = { Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },

                new Client
                {
                    ClientId = Scopes.PermissionsApi,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },

                    AllowedScopes = { Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },
                new Client
                {
                    ClientId = Scopes.PermissionsProcessor,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },
                    AllowedScopes = { Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },

                // ticketmanagement service client
                new Client
                {
                    ClientId = Scopes.TicketManagementApi,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },
                    AllowedScopes = { Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },
                new Client
                {
                    ClientId = Scopes.TicketManagementProcessor,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },
                    AllowedScopes = { Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },
                // ticket reservation service client
                new Client
                {
                    ClientId = Scopes.TicketReservationsApi,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },
                    AllowedScopes = { Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },
                new Client
                {
                    ClientId = Scopes.TicketReservationsProcessor,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },
                    AllowedScopes = { Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },
                // main api
                new Client
                {
                    ClientId = "sonatribe.api",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },
                    AllowedScopes = { Scopes.DashApi, Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },
                // dashbaord api
                new Client
                {
                    ClientId = "sonatribe.dashboard.api",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },
                    AllowedScopes = { Scopes.MainApi, Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },
                new Client
                {
                    ClientId = "sonatribe.operator.api",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },
                    AllowedScopes = { Scopes.MainApi, Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },
                new Client
                {
                    ClientId = "sonatribe.orders.api",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },
                    AllowedScopes = { Scopes.MainApi, Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },
                new Client
                {
                    ClientId = "sonatribe.shop.api",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(HashExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]))
                    },
                    AllowedScopes = { Scopes.MainApi, Scopes.PermissionsApi, Scopes.TicketManagementApi, Scopes.TicketReservationsApi, Scopes.PaymentApi }
                },

                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowOfflineAccess  = true,
                    AccessTokenLifetime = 7 * 24 * 60 * 60,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RedirectUris =
                    {
                        $"https://{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("MAIN_UI_PORT")}/callback"
                    },
                    PostLogoutRedirectUris =
                    {
                        $"https://{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("MAIN_UI_PORT")}",
                    },
                    AllowedCorsOrigins =
                    {
                        $"https://{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("MAIN_UI_PORT")}"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        Scopes.MainApi,
                        Scopes.PermissionsApi,
                        Scopes.TicketManagementApi,
                        Scopes.TicketReservationsApi,
                        Scopes.PaymentApi,
                    },
                },

                new Client
                {
                    ClientId = "dashboard",
                    ClientName = "Dashboard Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowOfflineAccess  = true,
                    AccessTokenLifetime = 7 * 24 * 60 * 60,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequireConsent = false,
                    RedirectUris =
                    {
                        $"https://dashboard.{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("DASHBOARD_UI_PORT")}/callback"
                    },
                    PostLogoutRedirectUris =
                    {
                        $"https://dashboard.{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("DASHBOARD_UI_PORT")}"
                    },
                    AllowedCorsOrigins =
                    {
                        $"https://dashboard.{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("DASHBOARD_UI_PORT")}"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        System.Security.Claims.ClaimTypes.Role,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        Scopes.PermissionsApi,
                        Scopes.TicketManagementApi,
                        Scopes.TicketReservationsApi,
                        Scopes.PaymentApi,
                        Scopes.DashApi
                    },
                },
                
                 new Client
                {
                    ClientId = "operator",
                    ClientName = "Operator Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowOfflineAccess  = true,
                    AccessTokenLifetime = 7 * 24 * 60 * 60,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequireConsent = false,
                    RedirectUris =
                    {
                        $"https://ops.{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("OPS_UI_PORT")}/callback"
                    },
                    PostLogoutRedirectUris =
                    {
                        $"https://ops.{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("OPS_UI_PORT")}"
                    },
                    AllowedCorsOrigins =
                    {
                        $"https://ops.{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("OPS_UI_PORT")}"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        System.Security.Claims.ClaimTypes.Role,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        Scopes.PermissionsApi,
                        Scopes.TicketManagementApi,
                        Scopes.TicketReservationsApi,
                        Scopes.PaymentApi,
                        Scopes.DashApi,
                        Scopes.OperatorApi
                    },
                },
                
                new Client
                {
                    ClientId = "orders",
                    ClientName = "Orders Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowOfflineAccess  = true,
                    AccessTokenLifetime = 7 * 24 * 60 * 60,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequireConsent = false,
                    RedirectUris =
                    {
                        $"https://orders.{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("ORDERS_UI_PORT")}/callback"
                    },
                    PostLogoutRedirectUris =
                    {
                        $"https://orders.{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("ORDERS_UI_PORT")}"
                    },
                    AllowedCorsOrigins =
                    {
                        $"https://orders.{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("ORDERS__UI_PORT")}"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        System.Security.Claims.ClaimTypes.Role,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        Scopes.MainApi,
                        Scopes.PermissionsApi,
                        Scopes.TicketManagementApi,
                        Scopes.TicketReservationsApi,
                        Scopes.PaymentApi,
                        Scopes.OrdersApi
                    },
                },

                new Client
                {
                    ClientId = "tickets",
                    ClientName = "Tickets Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowOfflineAccess  = true,
                    AccessTokenLifetime = 7 * 24 * 60 * 60,
                    AllowAccessTokensViaBrowser = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequireConsent = false,
                    RedirectUris =
                    {
                        $"https://shop.{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("SHOP_UI_PORT")}/callback"
                    },
                    PostLogoutRedirectUris =
                    {
                        $"https://shop.{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("SHOP_UI_PORT")}"
                    },
                    AllowedCorsOrigins =
                    {
                        $"https://shop.{Environment.GetEnvironmentVariable("DOMAIN_PREPEND")}{Environment.GetEnvironmentVariable("A_RECORD")}.{Environment.GetEnvironmentVariable("DOMAIN_TLD")}{Environment.GetEnvironmentVariable("SHOP_UI_PORT")}"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "roles",
                        System.Security.Claims.ClaimTypes.Role,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        Scopes.MainApi,
                        Scopes.PermissionsApi,
                        Scopes.TicketManagementApi,
                        Scopes.TicketReservationsApi,
                        Scopes.PaymentApi,
                        Scopes.TicketsApi
                    },
                }
            };
        }
    }
}