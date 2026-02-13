using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookingApi.Utils
{
    public static class FlightUtils
    {
        private static readonly string GeoNamesUsername = "w444555888"; // 你的帳號

        public static async Task<int?> CalculateFlightDurationAsync(string departureCity, string arrivalCity)
        {
            var dep = await GetCoordsFromGeoNamesAsync(departureCity);
            var arr = await GetCoordsFromGeoNamesAsync(arrivalCity);

            if (dep == null || arr == null) return null;

            double depLat = dep.Value.lat;
            double depLon = dep.Value.lon;
            double arrLat = arr.Value.lat;
            double arrLon = arr.Value.lon;

            double R = 6371; // 地球半徑 (公里)
            double dLat = Deg2Rad(arrLat - depLat);
            double dLon = Deg2Rad(arrLon - depLon);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(Deg2Rad(depLat)) *
                        Math.Cos(Deg2Rad(arrLat)) *
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;

            double averageSpeed = 900; // km/h
            double durationHours = distance / averageSpeed;

            return (int)Math.Round(durationHours * 60);
        }

        private static async Task<(double lat, double lon)?> GetCoordsFromGeoNamesAsync(string city)
        {
            using var client = new HttpClient();
            string url = $"http://api.geonames.org/searchJSON?q={Uri.EscapeDataString(city)}&maxRows=1&username={GeoNamesUsername}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.GetProperty("geonames").GetArrayLength() == 0) return null;

            var geo = doc.RootElement.GetProperty("geonames")[0];
            if (!double.TryParse(geo.GetProperty("lat").GetString(), out double lat)) return null;
            if (!double.TryParse(geo.GetProperty("lng").GetString(), out double lon)) return null;
            return (lat, lon);
        }

        private static double Deg2Rad(double deg) => deg * Math.PI / 180;
    }
}
