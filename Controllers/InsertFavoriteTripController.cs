using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KURSOVA
{
    [ApiController]
    [Route("[controller]")]
    public class InsertFavoriteTripController : ControllerBase
    {
        private readonly ILogger<InsertFavoriteTripController> _logger;

        public InsertFavoriteTripController(ILogger<InsertFavoriteTripController> logger)
        {
            _logger = logger;
        }

       

        [HttpPost(Name = "InsertFavoriteTrip")]
        public string FavoriteTrip(string FavoriteTrip, long UserId)
        {
            Database db = new Database();
            db.InsertFavoriteTripAsync(FavoriteTrip, UserId);
            return FavoriteTrip;
        }

    }
}
