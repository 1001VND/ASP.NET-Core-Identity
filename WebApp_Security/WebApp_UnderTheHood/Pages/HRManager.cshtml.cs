using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using WebApp_UnderTheHood.Authorization;
using WebApp_UnderTheHood.Dto;
using WebApp_UnderTheHood.Pages.Account;

namespace WebApp_UnderTheHood.Pages
{
    [Authorize(Policy = "HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty]
        public List<WeatherForecastDTO> weatherForecastItems { get; set; } = new List<WeatherForecastDTO>();
        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task OnGetAsync()
        {
            // get token from session
            JwtToken token = new JwtToken();

            var strTokenObj = HttpContext.Session.GetString("access_token");

            if (string.IsNullOrEmpty(strTokenObj))
            {
                token = await Authenticate();
            }
            else
            {
                token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj) ?? new JwtToken();
            }

            if (token == null || string.IsNullOrEmpty(token.AccessToken) || token.ExpiresAt <= DateTime.UtcNow)
            {
                token = await Authenticate();
            }

            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);

            weatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast") ?? new List<WeatherForecastDTO>();
        }

        #region -- Authentication and getting the token
        private async Task<JwtToken> Authenticate()
        {
            var httpClient = _httpClientFactory.CreateClient("OurWebAPI");

            var res = await httpClient.PostAsJsonAsync("auth", new Credential
            {
                UserName = "admin",
                Password = "admin"
            });

            res.EnsureSuccessStatusCode();

            string strJwt = await res.Content.ReadAsStringAsync();

            HttpContext.Session.SetString("access_token", strJwt);

            return JsonConvert.DeserializeObject<JwtToken>(strJwt) ?? new JwtToken();
        }
        #endregion
    }
}
