using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KURSOVA
{
    [ApiController]
    [Route("[controller]")]

    public class SearchTripController : ControllerBase
    {
        private readonly ILogger<SearchTripController> _logger;

        public SearchTripController(ILogger<SearchTripController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "SearchTrip")]
        public BlaBlaCarResponse Trip(string coor1, string coor2, string data, long UserId)
        {
            Database db = new Database();
            SearchTripClient client = new SearchTripClient();
            db.InsertSearchTripAsync(client.SearchSomeTripAsync(coor1, coor2, data).Result, coor1, coor2, data, UserId);
            return client.SearchSomeTripAsync(coor1, coor2, data).Result;
        }

      
    }


}
