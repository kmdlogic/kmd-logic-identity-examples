using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kmd.Logic.Identity.Examples.AdminApp.Domain;
using Newtonsoft.Json;

namespace Kmd.Logic.Identity.Examples.AdminApp.Services
{
    public class TodoService : IDisposable
    {
        private readonly HttpClient _client;

        public TodoService()
        {
            _client = new HttpClient { BaseAddress = new Uri(App.TodosApiUrl) };
        }

        public async Task<List<Todo>> GetTodos(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/todos");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var todos = JsonConvert.DeserializeObject<List<Todo>>(await response.Content.ReadAsStringAsync());
                return todos;
            }

            return null;
        }

        public async Task<bool> DeleteTodosForUser(string accessToken, string userId)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/todos/{userId}");
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