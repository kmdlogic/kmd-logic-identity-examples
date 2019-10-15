using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Kmd.Logic.Identity.Examples.AggregatedCalendar.Auth
{
    public static class AzureAdB2CAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAzureAdB2C(this AuthenticationBuilder builder)
            => builder.AddAzureAdB2C(_ => { });

        public static AuthenticationBuilder AddAzureAdB2C(this AuthenticationBuilder builder, Action<AzureAdB2COptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsSetup>();
            builder.AddOpenIdConnect();
            return builder;
        }

        public class OpenIdConnectOptionsSetup : IConfigureNamedOptions<OpenIdConnectOptions>
        {
            public OpenIdConnectOptionsSetup(IOptions<AzureAdB2COptions> b2COptions)
            {
                _azureAdB2COptions = b2COptions.Value;
            }

            private readonly AzureAdB2COptions _azureAdB2COptions;

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = _azureAdB2COptions.ClientId;
                options.Authority = _azureAdB2COptions.Authority;
                options.UseTokenLifetime = true;
                options.TokenValidationParameters = new TokenValidationParameters() { NameClaimType = "name" };

                options.Events = new OpenIdConnectEvents()
                {
                    OnRedirectToIdentityProvider = OnRedirectToIdentityProvider,
                    OnRemoteFailure = OnRemoteFailure,
                    OnAuthorizationCodeReceived = OnAuthorizationCodeReceived
                };
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }

            public Task OnRedirectToIdentityProvider(RedirectContext context)
            {
                context.ProtocolMessage.Scope += $" offline_access {_azureAdB2COptions.ApiScopes}";
                context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                return Task.CompletedTask;
            }

            public Task OnRemoteFailure(RemoteFailureContext context)
            {
                context.HandleResponse();

                if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("access_denied"))
                {
                    context.Response.Redirect("/");
                }
                else
                {
                    context.Response.Redirect("/Home/Error?message=" + Uri.EscapeDataString(context.Failure.Message));
                }

                return Task.CompletedTask;
            }

            public async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
            {
                // Use MSAL to swap the code for an access token
                // Extract the code from the response notification
                var code = context.ProtocolMessage.Code;

                var signedInUserId = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

                IConfidentialClientApplication cca = ConfidentialClientApplicationBuilder
                    .Create(_azureAdB2COptions.ClientId)
                    .WithB2CAuthority(_azureAdB2COptions.Authority)
                    .WithRedirectUri(_azureAdB2COptions.RedirectUri)
                    .WithClientSecret(_azureAdB2COptions.ClientSecret)
                    .Build();

                new MSALStaticCache(signedInUserId, context.HttpContext).EnablePersistence(cca.UserTokenCache);

                try
                {
                    AuthenticationResult result = await cca.AcquireTokenByAuthorizationCode(_azureAdB2COptions.ApiScopes.Split(' '), code)
                        .ExecuteAsync();

                    context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                }
                catch (Exception ex)
                {
                    context.Response.Redirect("/Home/Error?message=" + Uri.EscapeDataString(ex.Message));
                }
            }
        }
    }
}