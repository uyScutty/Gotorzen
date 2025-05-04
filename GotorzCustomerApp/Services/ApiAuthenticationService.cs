using Microsoft.AspNetCore.Components.Authorization;

using Microsoft.AspNetCore.Components.Authorization;

using System.Net.Http.Json;



public class ApiAuthenticationService
{
    private readonly HttpClient _http;
    private readonly ApiAuthenticationStateProvider _authProvider;

    public ApiAuthenticationService(HttpClient http, AuthenticationStateProvider provider)
    {
        _http = http;
        _authProvider = (ApiAuthenticationStateProvider)provider;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var loginRequest = new { Email = email, Password = password };
        var response = await _http.PostAsJsonAsync("api/auth/login", loginRequest);

        if (!response.IsSuccessStatusCode)
            return false;

        var token = await response.Content.ReadAsStringAsync();
        _authProvider.SetToken(token);
        _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return true;
    }

    public void Logout()
    {
        _authProvider.Logout();
        _http.DefaultRequestHeaders.Authorization = null;
    }
}
