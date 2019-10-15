using System.Net;
using System.Windows;
using Kmd.Logic.Identity.Examples.AdminApp.Tokens;
using Microsoft.Identity.Client;

namespace Kmd.Logic.Identity.Examples.AdminApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string Tenant = "logicidentityprod.onmicrosoft.com";
        private const string AzureAdB2CHostname = "logicidentityprod.b2clogin.com";
        public static string PolicySignUpSignIn = "B2C_1A_signup_signin";
        private const string ClientId = "68bf6751-9bdd-44ed-b060-d1b039fb1c75";
        public static string[] ApiScopes =
        {
            "https://logicidentityprod.onmicrosoft.com/ieapis/dates.read",
            "https://logicidentityprod.onmicrosoft.com/ieapis/dates.write",
            "https://logicidentityprod.onmicrosoft.com/ieapis/todos.read",
            "https://logicidentityprod.onmicrosoft.com/ieapis/todos.write",
            "https://logicidentityprod.onmicrosoft.com/ieapis/todos.admin"
        };

        private static readonly string AuthorityBase = $"https://{AzureAdB2CHostname}/tfp/{Tenant}/";
        public static string AuthoritySignInSignUp = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string DatesApiUrl = "https://localhost:44327/";
        public static string TodosApiUrl = "https://localhost:44328/";

        public static IPublicClientApplication PublicClientApp { get; }

        static App()
        {
            // Specify to use TLS 1.2 as default connection, otherwise API calls will fail
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            PublicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithB2CAuthority(AuthoritySignInSignUp)
                .Build();

            TokenCacheHelper.Bind(PublicClientApp.UserTokenCache);
        }
    }
}
