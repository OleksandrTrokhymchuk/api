using System;
using System.Collections.Generic;
using Npgsql;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace KURSOVA
{
    public class Database
    {
        private string infoAboutTrip;
        private string durationOfTrip;
        private string statusOfRequest;
        NpgsqlConnection con = new NpgsqlConnection(Constants.Connect);
        public async Task InsertSearchTripAsync(BlaBlaCarResponse blaBlaCarResponse, string coor1, string coor2, string data, long UserId)
        {
            await con.OpenAsync();

            var numberOfTrips = blaBlaCarResponse.search_info.count;
            DateTime dateTime = DateTime.ParseExact(data, "MM-dd", null);
            string dataForUser = dateTime.ToString("d MMMM", new System.Globalization.CultureInfo("uk-UA"));

            if (numberOfTrips == 0)
            {
                var sql = "TRUNCATE public.\"Trip\"; INSERT INTO public.\"Trip\"(\"Request\", \"InfoAboutTrip\", \"Price\", \"StatusOfRequest\", \"UserId\") " +
                          "VALUES (@Request, @InfoAboutTrip, @Price, @StatusOfRequest, @UserId)";
                NpgsqlCommand comm = new NpgsqlCommand(sql, con);
                string statusOfRequest = $"На жаль, не знайдено жодної поїздки на {dataForUser}";
                string request = $"From {coor1} to {coor2} at 2023-{data}";
                string infoAboutTrip = "0 поїздок";
                int price = 0;

                comm.Parameters.AddWithValue("@Request", request);
                comm.Parameters.AddWithValue("@InfoAboutTrip", infoAboutTrip);
                comm.Parameters.AddWithValue("@Price", price);
                comm.Parameters.AddWithValue("@StatusOfRequest", statusOfRequest);
                comm.Parameters.AddWithValue("@UserId", UserId);

                await comm.ExecuteNonQueryAsync();
            }
            else
            {
                string statusOfRequest = $"Поїздки на {dataForUser} знайдено!\n\n";

                string sql;

                for (int i = 0; i < numberOfTrips; i++)
                {

                    if (i == 0)
                    {
                         sql = "TRUNCATE public.\"Trip\"; INSERT INTO public.\"Trip\"(\"Request\", \"InfoAboutTrip\", \"Price\", \"StatusOfRequest\", \"UserId\") " +
                          "VALUES (@Request, @InfoAboutTrip, @Price, @StatusOfRequest, @UserId)";
                    }
                    else
                    {
                         sql = "INSERT INTO public.\"Trip\"(\"Request\", \"InfoAboutTrip\", \"Price\", \"StatusOfRequest\", \"UserId\") " +
                          "VALUES (@Request, @InfoAboutTrip, @Price, @StatusOfRequest, @UserId)";
                    }
                        NpgsqlCommand comm = new NpgsqlCommand(sql, con);
                    int totalSeconds = blaBlaCarResponse.trips[i].duration_in_seconds;
                    TimeSpan duration = TimeSpan.FromSeconds(totalSeconds);
                    int hours = duration.Hours;
                    int minutes = duration.Minutes;

                    string durationOfTrip;
                    if (hours == 1)
                    {
                        durationOfTrip = $"{hours} годину {minutes} хвилин";
                    }
                    else if (hours >= 2 && hours <= 4)
                    {
                        durationOfTrip = $"{hours} години {minutes} хвилин";
                    }
                    else
                    {
                        durationOfTrip = $"{hours} годин {minutes} хвилин";
                    }

                    string request = $"From {coor1} to {coor2} at 2023-{data}";

                    string infoAboutTrip;
                    if (blaBlaCarResponse.trips[i].vehicle != null)
                    {
                        infoAboutTrip = $"Поїздка №{i + 1}: {blaBlaCarResponse.trips[i].link}\n" +
                            $"Автомобіль: {blaBlaCarResponse.trips[i].vehicle.make} {blaBlaCarResponse.trips[i].vehicle.model}\n" +
                            $"Виїзд з {blaBlaCarResponse.trips[i].waypoints[0].place.address} о {blaBlaCarResponse.trips[i].waypoints[0].date_time.ToString("HH:mm")}\n" +
                            $"Відстань: {blaBlaCarResponse.trips[i].distance_in_meters / 1000} км\n" +
                            $"Тривалість: {durationOfTrip}\n";
                    }
                    else
                    {
                        infoAboutTrip = $"Поїздка №{i + 1}: {blaBlaCarResponse.trips[i].link}\n" +
                            $"Автомобіль: Інформація відсутня\n" +
                            $"Виїзд з {blaBlaCarResponse.trips[i].waypoints[0].place.address} о {blaBlaCarResponse.trips[i].waypoints[0].date_time.ToString("HH:mm")}\n" +
                            $"Відстань: {blaBlaCarResponse.trips[i].distance_in_meters / 1000} км\n" +
                            $"Тривалість: {durationOfTrip}\n";
                    }

                    double priceString = double.Parse(blaBlaCarResponse.trips[i].price.amount, CultureInfo.InvariantCulture);
                    int price = (int)Math.Floor(priceString);

                    //comm.Parameters.Clear();
                    comm.Parameters.AddWithValue("@Request", request);
                    comm.Parameters.AddWithValue("@InfoAboutTrip", infoAboutTrip);
                    comm.Parameters.AddWithValue("@Price", price);
                    comm.Parameters.AddWithValue("@StatusOfRequest", statusOfRequest);
                    comm.Parameters.AddWithValue("@UserId", UserId);

                    await comm.ExecuteNonQueryAsync();
                }
            }

            con.Close();
        }


        public async Task<List<TripMain>> SelectStatistTrip(long UserId)
        {
            List<TripMain> tripMains = new List<TripMain>();
            await con.OpenAsync();

            var sql = "SELECT \"Request\", \"InfoAboutTrip\", \"Price\", \"StatusOfRequest\", \"UserId\" " +
                      "FROM public.\"Trip\" " +
                      "WHERE \"UserId\" = @UserId";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("@UserId", NpgsqlTypes.NpgsqlDbType.Bigint, UserId);

            NpgsqlDataReader npgsqlDataReader = await comm.ExecuteReaderAsync();
            while (await npgsqlDataReader.ReadAsync())
            {
                tripMains.Add(new TripMain
                {
                    Request = npgsqlDataReader.GetString(0),
                    InfoAboutTrip = npgsqlDataReader.GetString(1),
                    Price = npgsqlDataReader.GetInt32(2),
                    StatusOfRequest = npgsqlDataReader.GetString(3)
                });
            }

            await con.CloseAsync();
            return tripMains;
        }


        /////////////////////////////

        public async Task InsertSearchCityAsync(Response responseGeo, string address, long UserId)
        {
            var sql = "TRUNCATE public.\"City\"; INSERT INTO public.\"City\"(\"CityName\", \"Coordinates\", \"UserId\") " +
                      "VALUES (@CityName, @Coordinates, @UserId)";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);

            string coordinates = $"{responseGeo.Results[0].Geometry.Location.Lat.ToString().Replace(',', '.')}, {responseGeo.Results[0].Geometry.Location.Lng.ToString().Replace(',', '.')}";

            comm.Parameters.AddWithValue("@CityName", NpgsqlTypes.NpgsqlDbType.Text, address);
            comm.Parameters.AddWithValue("@Coordinates", NpgsqlTypes.NpgsqlDbType.Text, coordinates);
            comm.Parameters.AddWithValue("@UserId", NpgsqlTypes.NpgsqlDbType.Bigint, UserId);

            await con.OpenAsync();
            await comm.ExecuteNonQueryAsync();
            await con.CloseAsync();
        }

        public async Task<List<CityMain>> SelectStatistCity(long UserId)
        {
            List<CityMain> cityMains = new List<CityMain>();
            await con.OpenAsync();

            var sql = "SELECT \"CityName\", \"Coordinates\", \"UserId\" FROM public.\"City\" WHERE \"UserId\" = @UserId";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.Add("@UserId", NpgsqlTypes.NpgsqlDbType.Bigint).Value = UserId;

            NpgsqlDataReader npgsqlDataReader = await comm.ExecuteReaderAsync();
            while (await npgsqlDataReader.ReadAsync())
            {
                cityMains.Add(new CityMain
                {
                    CityName = npgsqlDataReader.GetString(0),
                    Coordinates = npgsqlDataReader.GetString(1)
                });
            }

            await con.CloseAsync();
            return cityMains;
        }




        //////////////////////////////////////////////




        public async Task InsertFavoriteTripAsync(string test, long UserId)
        {
            var sql = "INSERT INTO public.\"FavoriteTrip\"(\"Trip\", \"UserId\") " +
                      "VALUES (@Trip, @UserId)";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);

            comm.Parameters.AddWithValue("@Trip", test);
            comm.Parameters.AddWithValue("@UserId", UserId);

            await con.OpenAsync();
            await comm.ExecuteNonQueryAsync();
            await con.CloseAsync();
        }


        ////////////////////////////////



        public async Task DeleteListOfFavoriteTripsAsync(long UserId)
        {
            await con.OpenAsync();

            var sql = "DELETE FROM public.\"FavoriteTrip\" WHERE \"UserId\" = @UserId";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.Add("@UserId", NpgsqlTypes.NpgsqlDbType.Bigint).Value = UserId;

            await comm.ExecuteNonQueryAsync();

            await con.CloseAsync();
        }



        ////////////////////////////////

        public async Task<List<FavoriteTripMain>> SelectStatistFavoriteTrip(long UserId)
        {
            List<FavoriteTripMain> favoriteTripMains = new List<FavoriteTripMain>();
            await con.OpenAsync();

            var sql = "SELECT \"Trip\", \"UserId\" FROM public.\"FavoriteTrip\" WHERE \"UserId\" = @UserId";
            NpgsqlCommand comm = new NpgsqlCommand(sql, con);
            comm.Parameters.AddWithValue("@UserId", UserId);

            NpgsqlDataReader npgsqlDataReader = await comm.ExecuteReaderAsync();
            while (await npgsqlDataReader.ReadAsync())
            {
                favoriteTripMains.Add(new FavoriteTripMain
                {
                    Trip = npgsqlDataReader.GetString(0)
                });
            }

            await con.CloseAsync();
            return favoriteTripMains;
        }


    }
}
