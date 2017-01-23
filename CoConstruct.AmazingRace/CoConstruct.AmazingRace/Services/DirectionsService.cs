using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CoConstruct.AmazingRace.Models.GoogleMaps;
using Newtonsoft.Json;

namespace CoConstruct.AmazingRace.Services
{ 
    public interface IDirectionsService
    {
        Task<List<string>> GetDirections(string address1, string address2);
    }

    public class DirectionsService : IDirectionsService
    {
        private HttpClient _client;
        private string _apiKey = "AIzaSyB3eUuOb4se-CprWEn7kUG8h9BWAUom7xc";

        public DirectionsService()
        {
            _client = new HttpClient();
        }

        public async Task<List<string>> GetDirections(string address1, string address2)
        {
            var encodedAddress1 = System.Net.WebUtility.UrlEncode(address1);
            var encodedAddress2 = System.Net.WebUtility.UrlEncode(address2);
            var api = $"https://maps.googleapis.com/maps/api/directions/json?origin={encodedAddress1}&destination={encodedAddress2}&key={_apiKey}";

            var steps = new List<string>();
            var response = await _client.GetAsync(api);
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                var x = JsonConvert.DeserializeObject<RootObject>(content);
                if (x != null)
                {
                    foreach (var rt in x.routes)
                    {
                        foreach (var leg in rt.legs)
                        {
                            foreach (var step in leg.steps)
                            {
                                steps.Add(step.html_instructions);
                            }
                        }
                    }
                }
            }

            return steps;
            //https://maps.googleapis.com/maps/api/directions/json?origin=75+9th+Ave+New+York,+NY&destination=MetLife+Stadium+1+MetLife+Stadium+Dr+East+Rutherford,+NJ+07073&key=YOUR_API_KEY
        }
    }
}