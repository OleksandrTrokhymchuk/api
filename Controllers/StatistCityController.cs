using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace KURSOVA
{
    [ApiController]
    [Route("[controller]")]
    public class StatistCityController : ControllerBase
    {
        private readonly ILogger<StatistCityController> _logger;
        public StatistCityController(ILogger<StatistCityController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public List<CityMain> City(long UserId)
        {
            Database db = new Database();
            return db.SelectStatistCity(UserId).Result;
        }


    }
}
