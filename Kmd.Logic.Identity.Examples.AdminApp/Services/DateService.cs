using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kmd.Logic.Identity.Examples.AdminApp.Domain;
using Newtonsoft.Json;

namespace Kmd.Logic.Identity.Examples.AdminApp.Services
{
    public class DateService : IDisposable
    {
        private readonly HttpClient _client;

        public DateService()
        {
            _client = new HttpClient {BaseAddress = new Uri(App.DatesApiUrl)};
        }

        public async Task<List<CalendarEvent>> GetDates(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/dates");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var dates =JsonConvert.DeserializeObject<List<CalendarEvent>>(await response.Content.ReadAsStringAsync());
                return dates;
            }

            return null;
        }

        public async Task<bool> DeleteAllDates(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/dates");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _client.SendAsync(request);

            return response.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}