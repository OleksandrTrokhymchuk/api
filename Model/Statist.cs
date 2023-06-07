using System.Collections.Generic;

namespace KURSOVA
{
    public class Statist
    {
        List<TripMain> tripMains { get; set; }
        List<CityMain> cityMains { get; set; }
    }

    public class TripMain
    {
        public string Request { get; set; }
        public string InfoAboutTrip { get; set; }
        public int Price { get; set; }
        public string StatusOfRequest { get; set; }
        public long UserId { get; set; }
    }




    public class CityMain
    {

        public string CityName { get; set; }
        public string Coordinates { get; set; }
        public long UserId { get; set; }


    }


    public class FavoriteTripMain
    {
        //public string InfoAboutTrip { get; set; }
        //public int Price { get; set; }

        public string Trip { get; set; }
        public long UserId { get; set; }

    }

}