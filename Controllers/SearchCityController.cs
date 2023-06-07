using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KURSOVA
{
    [ApiController]
    [Route("[controller]")]

    public class SearchCityController : ControllerBase
    {
        private readonly ILogger<SearchCityController> _logger;

        public SearchCityController(ILogger<SearchCityController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "SearchCity")]
        public Response City(string address, long UserId)
        {
            Database db = new Database();
            SearchCityClient client = new SearchCityClient();
            db.InsertSearchCityAsync(client.SearchSomeCityAsync(address).Result, address, UserId);
            return client.SearchSomeCityAsync(address).Result;
        }
    }
}
