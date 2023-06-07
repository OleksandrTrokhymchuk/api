using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace KURSOVA
{
    public class SearchTripClient
    {
        private HttpClient _httpClient;
        public static string _address;
        public static string _apikey;

        public SearchTripClient()
        {
            _address = Constants.addressBlaBlaCar;
            _apikey = Constants.apikey;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_address);
        }

        //public async Task<BlaBlaCarResponse> SearchSomeTripAsync(string coor1, string coor2, string data)
        //{
        //    string year = DateTime.Now.Year.ToString();
        //    var responce = await _httpClient.GetAsync($"/api/v3/trips?from_coordinate={coor1}&to_coordinate={coor2}&locale=uk-UA&currency=UAH&start_date_local={year}-{data}T08%3A55%3A00&key={_apikey}"); /*T08%3A55%3A00*/
        //    responce.EnsureSuccessStatusCode();
        //    var content = responce.Content.ReadAsStringAsync().Result;
        //    var result = JsonConvert.DeserializeObject<BlaBlaCarResponse>(content);
        //    return result;
        //}


        ////////////////////////////////////////////





        public async Task<BlaBlaCarResponse> SearchSomeTripAsync(string coor1, string coor2, string data, string nextCursor = null)
        {
            string year = DateTime.Now.Year.ToString();
            var url = $"/api/v3/trips?from_coordinate={coor1}&to_coordinate={coor2}&locale=uk-UA&currency=UAH&start_date_local={year}-{data}T08%3A55%3A00&key={_apikey}";

            if (nextCursor != null)
            {
                url += $"&from_cursor={nextCursor}";
            }

            var responce = await _httpClient.GetAsync(url);
            responce.EnsureSuccessStatusCode();
            var content = await responce.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<BlaBlaCarResponse>(content);

            if (result.next_cursor != null)
            {
                var nextPageResult = await SearchSomeTripAsync(coor1, coor2, data, result.next_cursor);
                result.trips.AddRange(nextPageResult.trips);
            }

            return result;
        }








    }
}
