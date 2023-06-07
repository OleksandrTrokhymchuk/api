using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KURSOVA
{

    [ApiController]
    [Route("[controller]")]
    public class StatistFavoriteTripController : ControllerBase
    {
        private readonly ILogger<StatistFavoriteTripController> _logger;
        public StatistFavoriteTripController(ILogger<StatistFavoriteTripController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public List<FavoriteTripMain> FavotireTrip(long UserId)
        {
            Database db = new Database();
            return db.SelectStatistFavoriteTrip(UserId).Result;
        }


    }
}
