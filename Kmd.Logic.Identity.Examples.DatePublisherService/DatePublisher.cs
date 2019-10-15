using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kmd.Logic.Identity.Examples.DatePublisherService
{
    public class DatePublisher
    {
        readonly Timer _timer;
        private readonly string _datesApiUrl;
        private readonly ClientCredentialsConfig _clientCredentialsConfig;
        private string _accessToken;
        private JwtSecurityToken _parsedAccessToken;

        public DatePublisher(double interval, string datesApiUrl, ClientCredentialsConfig clientCredentialsConfig)
        {
            _datesApiUrl = datesApiUrl;
            _clientCredentialsConfig = clientCredentialsConfig;
            _timer = new Timer(interval) { AutoReset = true };
            _timer.Elapsed += async (sender, args) => await OnTimerElapsed();
        }

        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }

        private async Task OnTimerElapsed()
        {
            await RefreshAccessToken();

            if (!string.IsNullOrWhiteSpace(_accessToken))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

                    var dateOfInterest = DatesOfInterest.GetRandomDateOfInterest();
                    var json = JsonConvert.SerializeObject(dateOfInterest);
                    var payload = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(_datesApiUrl, payload);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Successfully posted {json} to Dates API");
                    }
                    else
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error while calling Dates API: {content}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Access token is null: Dates API requires an access token");
            }
        }

        private async Task RefreshAccessToken()
        {
            // Only refresh the access token when necessary - using a small tolerance to avoid expiry edge cases
            if (_parsedAccessToken != null && _parsedAccessToken.ValidTo > DateTime.UtcNow.AddMinutes(-2))
            {
                return;
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_clientCredentialsConfig.TokenEndpointBaseAddress);

                    var clientCredentialsRequest = new ClientCredentialsRequest
                    {
                        grant_Type = "client_credentials",
                        client_Id = _clientCredentialsConfig.ClientId,
                        client_Secret = _clientCredentialsConfig.Secret,
                        scope = _clientCredentialsConfig.Scope
                    };

                    var clientCredsJson = JsonConvert.SerializeObject(clientCredentialsRequest);
                    var content = new StringContent(clientCredsJson, Encoding.UTF8, "application/json");

                    var requestResult = await httpClient.PostAsync(_clientCredentialsConfig.TokenEndpoint, content);
                    var contentResult = await requestResult.Content.ReadAsStringAsync();

                    if (!requestResult.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Error while obtaining access token: {contentResult}");
                        throw new Exception(contentResult);
                    }

                    var json = JObject.Parse(contentResult);
                    _accessToken = (string)json["accessToken"];
                    _parsedAccessToken = new JwtSecurityToken(_accessToken);
                }
            }
            catch (Exception e)
            {
                _accessToken = null;
                _parsedAccessToken = null;
                Console.WriteLine($"Error while obtaining access token: {e.Message}");
            }
        }

        private class ClientCredentialsRequest
        {
            public string grant_Type { get; set; }
            public string client_Id { get; set; }
            public string client_Secret { get; set; }
            public string scope { get; set; }
        }
    }
}