using HtmlAgilityPack;
using Misc.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Misc
{
    public static class CovidData
    {
        private static string _source = "https://www.worldometers.info/coronavirus/";
        private static string _newSource = "https://services9.arcgis.com/N9p5hsImWXAccRNI/arcgis/rest/services/Z7biAeD8PAkqgmWhxG2A/FeatureServer/1/query?f=json&where=Confirmed%20%3E%200&returnGeometry=false&spatialRel=esriSpatialRelIntersects&outFields=Country_Region%2CProvince_State%2CConfirmed%2CRecovered%2CDeaths%2CActive&orderByFields=Confirmed%20desc%2CCountry_Region%20asc%2CProvince_State%20asc&outSR=102100&resultOffset=0&resultRecordCount=600&cacheHint=true";
        private static List<RegionData> _cache;
        private static DateTime _lastLoad = DateTime.MinValue;
        public static List<RegionData> LoadData()
        {
            if ((DateTime.Now - _lastLoad).TotalMinutes < 10)
            {
                return _cache;
            }
            //_cache.Clear();
            //let's do some parsing
            //var doc = new HtmlDocument();
            //var client = new HttpClient();
            //var stream = client.GetStreamAsync(_source).Result;
            //doc.Load(stream);
            //var temp = new List<CountryStats>();
            ////var table = doc.DocumentNode.SelectSingleNode("//table[@id='main_table_countries']");
            ////var rows = table.SelectNodes("//tbody//tr");
            //var rows = doc.DocumentNode.SelectNodes("//table[@id='main_table_countries']/tbody/tr");
            //foreach (var row in rows)
            //{
            //    var c = new CountryStats();
            //    var cells = row.SelectNodes("./td");
            //    c.Country = cells[0].InnerText.Trim();
            //    if (c.Country.Contains("Total:"))
            //        c.Country = "Global Stats";
            //    c.TotalCases = GetAmount(cells[1]);
            //    c.NewCases = GetAmount(cells[2]);
            //    c.TotalDeaths = GetAmount(cells[3]);
            //    c.NewDeaths = GetAmount(cells[4]);
            //    c.TotalRecovered = GetAmount(cells[5]);
            //    c.ActiveCases = GetAmount(cells[6]);
            //    c.SeriousCases = GetAmount(cells[7]);
            //    c.TotalCasesPerMillion = GetAmountFloat(cells[8]);
            //    temp.Add(c);
            //}


            
            using (var client = new HttpClient())
            {
                var json = client.GetStringAsync(_newSource).Result;
                var data = JsonConvert.DeserializeObject<CovidStats>(json);
                _cache = data.features.Select(x => x.attributes).ToList();
            }
            _lastLoad = DateTime.Now;
            return _cache;
        }

        

        public static RegionData GetStats(string country)
        {
            var data = LoadData();
            //attempt first to find by province / state
            var region = false;
            var choice = data.FirstOrDefault(x => x.State?.ToLower().StartsWith(country.ToLower()) ?? false);
            if (choice == null)
            {
                choice = data.FirstOrDefault(x => x.Country.ToLower().StartsWith(country.ToLower()));
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

            //var countries = LoadData();
            //var choice = countries.FirstOrDefault(x => x.Country.ToLower() == country.ToLower());
            //if (choice == null)
            //{
            //    choice = countries.FirstOrDefault(x => x.Country == "Global Stats");
            //}

            //return choice;
        }
    }
}
