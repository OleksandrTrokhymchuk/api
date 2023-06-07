using KURSOVA;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace курсова_2_частина.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeleteListOfFavoriteTripsController : ControllerBase
    {
        private readonly ILogger<DeleteListOfFavoriteTripsController> _logger;

        public DeleteListOfFavoriteTripsController(ILogger<DeleteListOfFavoriteTripsController> logger)
        {
            _logger = logger;
        }



        [HttpDelete(Name = "DeleteListOfFavoriteTripsAsync")]
        public void DeleteFavoriteTrip(long UserId)
        {
            Database db = new Database();
            db.DeleteListOfFavoriteTripsAsync(UserId);
        }

    }
}
