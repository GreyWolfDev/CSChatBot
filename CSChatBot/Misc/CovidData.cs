using HtmlAgilityPack;
using Misc.Models;
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
        private static List<CountryStats> _cache = new List<CountryStats>();
        private static DateTime _lastLoad = DateTime.MinValue;
        public static List<CountryStats> LoadData()
        {
            if ((DateTime.Now - _lastLoad).TotalMinutes < 10)
            {
                return _cache;
            }
            //_cache.Clear();
            //let's do some parsing
            var doc = new HtmlDocument();
            var client = new HttpClient();
            var stream = client.GetStreamAsync(_source).Result;
            doc.Load(stream);
            var temp = new List<CountryStats>();
            //var table = doc.DocumentNode.SelectSingleNode("//table[@id='main_table_countries']");
            //var rows = table.SelectNodes("//tbody//tr");
            var rows = doc.DocumentNode.SelectNodes("//table[@id='main_table_countries']/tbody/tr");
            foreach (var row in rows)
            {
                var c = new CountryStats();
                var cells = row.SelectNodes("./td");
                c.Country = cells[0].InnerText.Trim();
                if (c.Country.Contains("Total:"))
                    c.Country = "Global Stats";
                c.TotalCases = GetAmount(cells[1]);
                c.NewCases = GetAmount(cells[2]);
                c.TotalDeaths = GetAmount(cells[3]);
                c.NewDeaths = GetAmount(cells[4]);
                c.TotalRecovered = GetAmount(cells[5]);
                c.ActiveCases = GetAmount(cells[6]);
                c.SeriousCases = GetAmount(cells[7]);
                c.TotalCasesPerMillion = GetAmountFloat(cells[8]);
                temp.Add(c);
            }

            _lastLoad = DateTime.Now;
            _cache = temp;
            return _cache;
        }

        public static int GetAmount(HtmlNode node)
        {
            var text = node.InnerText.Replace(",", "").Replace("+", "");
            if (String.IsNullOrWhiteSpace(text))
                text = "0";
            return int.Parse(text);
        }

        public static float GetAmountFloat(HtmlNode node)
        {
            var text = node.InnerText.Replace(",", "").Replace("+", "");
            if (String.IsNullOrWhiteSpace(text))
                text = "0";
            return float.Parse(text);
        }

        public static CountryStats GetStats(string country)
        {
            var countries = LoadData();
            var choice = countries.FirstOrDefault(x => x.Country.ToLower() == country.ToLower());
            if (choice == null)
            {
                choice = countries.FirstOrDefault(x => x.Country == "Global Stats");
            }

            return choice;
        }
    }
}
