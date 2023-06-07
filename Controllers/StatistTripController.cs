using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KURSOVA
{
    [ApiController]
    [Route("[controller]")]
    public class StatistTripController : ControllerBase
    {
        private readonly ILogger<StatistTripController> _logger;
        public StatistTripController(ILogger<StatistTripController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public List<TripMain> Trip(long UserId)
        {
            Database db = new Database();
            return db.SelectStatistTrip(UserId).Result;
        }


    }
}
