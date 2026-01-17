//using System.Net.Http;
//using Newtonsoft.Json;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace main.Data
//{
//    public class GeoResult
//    {
//        public string lat { get; set; }
//        public string lon { get; set; }
//    }

//    public static class GeoHelper
//    {
//        public static async Task<(double lat, double lng)> GeocodeAsync(string location)
//        {
//            using var client = new HttpClient();
//            string url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(location)}";

//            client.DefaultRequestHeaders.UserAgent.ParseAdd("WPF-App");

//            var json = await client.GetStringAsync(url);
//            var results = JsonConvert.DeserializeObject<List<GeoResult>>(json);

//            if (results == null || results.Count == 0)
//                return (0, 0);

//            double lat = double.Parse(results[0].lat, System.Globalization.CultureInfo.InvariantCulture);
//            double lng = double.Parse(results[0].lon, System.Globalization.CultureInfo.InvariantCulture);

//            return (lat, lng);
//        }
//    }
//}