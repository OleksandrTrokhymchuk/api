using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KURSOVA
{
    public class SearchCityClient
    {
        private HttpClient _httpClient;
        public static string _addressGeo;

        public SearchCityClient()
        {
            _addressGeo = Constants.addressGeo;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_addressGeo);
        }


        public async Task<Response> SearchSomeCityAsync(string address)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://google-maps-geocoding.p.rapidapi.com/geocode/json?address={address}&language=en"),
                Headers =
                {
                    { "X-RapidAPI-Key", "230eaf5c21msh45a32c3e3ad8194p17351ajsn0e84bb4170b5" },
                    { "X-RapidAPI-Host", "google-maps-geocoding.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                var responseObject = JsonConvert.DeserializeObject<Response>(body);
                return responseObject;
            }

        }

    }
}
