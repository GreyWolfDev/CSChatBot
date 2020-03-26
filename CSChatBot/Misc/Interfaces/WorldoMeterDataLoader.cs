using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Misc.Interfaces
{
    public class WorldoMeterDataLoader : ICovidDataLoader
    {
        private string _source = "http://localhost/api/Covid/GetAll";
        private string _usSource = "https://www.worldometers.info/coronavirus/country/us/";
        private static List<RegionData> _cache;
        private static DateTime _lastLoad = DateTime.MinValue;
        public RegionData GetStats(string input)
        {
            var data = LoadData();
            var choice = data.FirstOrDefault(x => x.State?.ToLower() == input.ToLower());
            if (choice == null)
                choice = data.FirstOrDefault(x => x.Country.ToLower() == input.ToLower());
            if (choice == null)
                choice = data.FirstOrDefault(x => x.State?.ToLower().StartsWith(input.ToLower()) ?? false);
            if (choice == null)
                choice = data.FirstOrDefault(x => x.Country?.ToLower().StartsWith(input.ToLower()) ?? false);
            if (choice == null)
                choice = data.FirstOrDefault(x => x.Country == "Global");
            return choice;
        }

        public List<RegionData> LoadData()
        {
            
            return JsonConvert.DeserializeObject<List<RegionData>>(new HttpClient().GetStringAsync(_source).Result);
            //if ((DateTime.Now - _lastLoad).TotalMinutes < 10)
            //{
            //    return _cache;
            //}

            //let's do some parsing
            //var doc = new HtmlDocument();
            //var client = new HttpClient();
            //var stream = client.GetStreamAsync(_source).Result;
            //doc.Load(stream);
            //var temp = new List<RegionData>();

            //var table = doc.DocumentNode.SelectSingleNode("//table[@id='main_table_countries']");
            //var rows = table.SelectNodes("//tbody//tr");
            //var rows = doc.DocumentNode.SelectNodes("//table[@id='main_table_countries_today']/tbody/tr");
            //foreach (var row in rows)
            //{
            //    var c = new RegionData();
            //    var cells = row.SelectNodes("./td");
            //    var href = cells[0].SelectSingleNode("./a")?.Attributes["href"]?.Value;
            //    c.Country_Region = cells[0].InnerText.Trim();
            //    if (c.Country_Region.Contains("Total:"))
            //        c.Country_Region = "Global";
            //    c.Confirmed = GetAmount(cells[1]);
            //    c.NewCases = GetAmount(cells[2]);
            //    c.Deaths = GetAmount(cells[3]);
            //    c.NewDeaths = GetAmount(cells[4]);
            //    c.Recovered = GetAmount(cells[5]);
            //    c.Active = GetAmount(cells[6]);
            //    c.SeriousCases = GetAmount(cells[7]);
            //    c.TotalCasesPerMillion = GetAmountFloat(cells[8]);
            //    if (href != null)
            //        c.Source = $"https://www.worldometers.info/coronavirus/{href}";
            //    else
            //        c.Source = "https://www.worldometers.info/coronavirus";
            //    temp.Add(c);
            //}

            ////load US data
            //stream = client.GetStreamAsync(_usSource).Result;
            //doc.Load(stream);
            //rows = doc.DocumentNode.SelectNodes("//table[@id='usa_table_countries_today']/tbody/tr");
            //foreach (var row in rows)
            //{
            //    var c = new RegionData();
            //    var cells = row.SelectNodes("./td");
            //    c.Country_Region = "USA";
            //    c.Province_State = cells[0].InnerText.Trim();
            //    if (c.Province_State.Contains("Total:"))
            //        continue;
            //    c.Confirmed = GetAmount(cells[1]);
            //    c.NewCases = GetAmount(cells[2]);
            //    c.Deaths = GetAmount(cells[3]);
            //    c.NewDeaths = GetAmount(cells[4]);
            //    c.Active = GetAmount(cells[5]);
            //    c.Recovered = c.Confirmed - c.Active - c.Deaths;
            //    c.SeriousCases = -1;
            //    c.Source = "https://www.worldometers.info/coronavirus/country/us/";
            //    //c.TotalCasesPerMillion = GetAmountFloat(cells[8]);
            //    temp.Add(c);
            //}



            //_cache = temp;
            return _cache;
        }

        private int GetAmount(HtmlNode node)
        {
            var text = node.InnerText.Replace(",", "").Replace("+", "");
            if (String.IsNullOrWhiteSpace(text))
                text = "0";
            return int.Parse(text);
        }

        private float GetAmountFloat(HtmlNode node)
        {
            var text = node.InnerText.Replace(",", "").Replace("+", "");
            if (String.IsNullOrWhiteSpace(text))
                text = "0";
            return float.Parse(text);
        }
    }
}
