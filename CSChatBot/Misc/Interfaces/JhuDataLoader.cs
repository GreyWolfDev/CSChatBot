using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Misc.Interfaces
{
    public class JhuDataLoader : ICovidDataLoader
    {
        private string _source = "https://services9.arcgis.com/N9p5hsImWXAccRNI/arcgis/rest/services/Z7biAeD8PAkqgmWhxG2A/FeatureServer/1/query?f=json&where=Confirmed%20%3E%200&returnGeometry=false&spatialRel=esriSpatialRelIntersects&outFields=Country_Region%2CProvince_State%2CConfirmed%2CRecovered%2CDeaths%2CActive&orderByFields=Confirmed%20desc%2CCountry_Region%20asc%2CProvince_State%20asc&outSR=102100&resultOffset=0&resultRecordCount=600&cacheHint=true";
        private static List<RegionData> _cache;
        private static DateTime _lastLoad = DateTime.MinValue;
        public RegionData GetStats(string input)
        {
            var data = LoadData();
            //attempt first to find by province / state
            var region = false;
            var choice = data.FirstOrDefault(x => x.State?.ToLower().StartsWith(input.ToLower()) ?? false);
            if (choice == null)
            {
                choice = data.FirstOrDefault(x => x.Country.ToLower().StartsWith(input.ToLower()));
                if (choice != null)
                    region = true;
            }
            if (choice == null)
            {
                //global stats are given then
                choice = new RegionData
                {
                    Country = "Global",
                    ActiveCases = data.Sum(x => x.ActiveCases),
                    TotalCases = data.Sum(x => x.TotalCases),
                    TotalDeaths = data.Sum(x => x.TotalDeaths),
                    TotalRecovered = data.Sum(x => x.TotalRecovered)
                };
            }

            if (region)
            {
                var temp = choice;
                choice = new RegionData
                {
                    Country = temp.Country,
                    ActiveCases = data.Where(x => x.Country == temp.Country).Sum(x => x.ActiveCases),
                    TotalCases = data.Where(x => x.Country == temp.Country).Sum(x => x.TotalCases),
                    TotalDeaths = data.Where(x => x.Country == temp.Country).Sum(x => x.TotalDeaths),
                    TotalRecovered = data.Where(x => x.Country == temp.Country).Sum(x => x.TotalRecovered)
                };
            }

            return choice;
        }

        public List<RegionData> LoadData()
        {
            if ((DateTime.Now - _lastLoad).TotalMinutes < 10)
            {
                return _cache;
            }
            using (var client = new HttpClient())
            {
                var json = client.GetStringAsync(_source).Result;
                var data = JsonConvert.DeserializeObject<CovidStats>(json);
                _cache = data.features.Select(x => x.attributes).ToList();
            }
            _lastLoad = DateTime.Now;
            return _cache;
        }
    }
}
