using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Kmd.Logic.Identity.Examples.AggregatedCalendar.Auth;
using Kmd.Logic.Identity.Examples.AggregatedCalendar.Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;

namespace Kmd.Logic.Identity.Examples.AggregatedCalendar.Pages
{
    public class MyCalendarModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly AzureAdB2COptions _azureAdB2COptions;

        public MyCalendarModel(IHttpClientFactory clientFactory, IOptions<AzureAdB2COptions> b2COptions)
        {
            _clientFactory = clientFactory;
            _azureAdB2COptions = b2COptions.Value;
        }

        public IList<CalendarEvent> CalendarEvents { get; set; } = new List<CalendarEvent>();
        public string ErrorMessage { get; private set; }

        public async Task OnGetAsync()
        {
            ErrorMessage = null;
            CalendarEvents.Clear();

            try
            {
                var accessToken = await GetAccessToken();
                await GetAndAddDatesToCalendarEvents(accessToken);
                await GetAndAddTodosToCalendarEvents(accessToken);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        private async Task GetAndAddDatesToCalendarEvents(string accessToken)
        {
            var client = _clientFactory.CreateClient(Startup.DateApiClient);

            var request = new HttpRequestMessage(HttpMethod.Get, "api/dates");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var dates = await response.Content.ReadAsAsync<IEnumerable<CalendarEvent>>();
                foreach (var calendarEvent in dates)
                {
                    CalendarEvents.Add(calendarEvent);
                }
            }
        }

        private async Task GetAndAddTodosToCalendarEvents(string accessToken)
        {
            string signedInUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var client = _clientFactory.CreateClient(Startup.TodosApiClient);

            var request = new HttpRequestMessage(HttpMethod.Get, $"api/todos/{signedInUserId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var dates = await response.Content.ReadAsAsync<IEnumerable<CalendarEvent>>();
                foreach (var calendarEvent in dates)
                {
                    CalendarEvents.Add(calendarEvent);
                }
            }
        }

        private async Task<string> GetAccessToken()
        {
            try
            {
                // Retrieve the token with the specified scopes
                var scope = _azureAdB2COptions.ApiScopes.Split(' ');
                string signedInUserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                IConfidentialClientApplication cca =
                    ConfidentialClientApplicationBuilder.Create(_azureAdB2COptions.ClientId)
                        .WithRedirectUri(_azureAdB2COptions.RedirectUri)
                        .WithClientSecret(_azureAdB2COptions.ClientSecret)
                        .WithB2CAuthority(_azureAdB2COptions.Authority)
                        .Build();
                new MSALStaticCache(signedInUserId, HttpContext).EnablePersistence(cca.UserTokenCache);

                var accounts = await cca.GetAccountsAsync();
                AuthenticationResult result = await cca.AcquireTokenSilent(scope, accounts.FirstOrDefault())
                    .ExecuteAsync();

                return result.AccessToken;
            }
            catch (MsalUiRequiredException ex)
            {
                throw new Exception($"Session has expired. Please sign in again. {ex.Message}");
            }
            catch (Exception e)
            {
                throw new Exception($"Error obtaining access token. {e.Message}");
            }
        }
    }
}