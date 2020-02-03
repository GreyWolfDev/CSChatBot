using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DB;
using DB.Extensions;
using DB.Models;
using ModuleFramework;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using OpenWeatherMap;

// ReSharper disable InconsistentNaming

namespace WeatherModule
{
    /// <summary>
    /// A sample module for CSChatBot - gets the weather
    /// </summary>
    [ModuleFramework.Module(Author = "parabola949", Name = "Weather", Version = "1.0")]
    public class Weather
    {
        public static string ApiKey { get; set; }
        internal static Random Rand => new Random();
        public Weather(Instance db, Setting settings, TelegramBotClient bot)
        {
            //In the module constructor, you can do some initial setup, like adding information to the database and getting input

            settings.AddField(db, "WeatherAPIKey");
            ApiKey = settings.GetString(db, "WeatherAPIKey");

            if (String.IsNullOrWhiteSpace(ApiKey))
            {
                //now ask for the API Key
                Console.Clear();
                Console.Write("What is your OpenWeatherMap API key? : ");
                ApiKey = Console.ReadLine();
                settings.SetString(db, "WeatherAPIKey", ApiKey);
            }
        }

        [ChatCommand(Triggers = new[] { "w", "weather" }, DontSearchInline = true, HelpText = "Gets the weather using your phone GPS", Parameters = new[] { "none - yourself", "<userid>", "<@username>", "as a reply", "<location>" })]
        public static CommandResponse GetWeather(CommandEventArgs args)
        {
            return GetResponse(args, false);
        }

        [ChatCommand(Triggers = new[] { "fw" }, DontSearchInline = true, HelpText = "Get the fucking weather, eh?", Parameters = new[] { "none - yourself", "<userid>", "<@username>", "as a reply", "<location>" })]
        public static CommandResponse GetFknWeather(CommandEventArgs args)
        {
            return GetResponse(args, true);
        }

        private static CommandResponse GetResponse(CommandEventArgs args, bool fknweather)
        {
            var loc = args.Parameters;
            if (String.IsNullOrEmpty(loc))
            {
                return (fknweather ? GetFknWeather(args.SourceUser.Location) : GetWeather(args.SourceUser.Location)) ?? new CommandResponse("Please use !setloc <location> to set your default location, or enter a location as a parameter");
            }
            var target = args.Message.GetTarget(args.Parameters, args.SourceUser, args.DatabaseInstance);
            var location = target == args.SourceUser ? loc : target.Location;
            var weather = fknweather ? GetFknWeather(location) : GetWeather(location);

            return weather ?? new CommandResponse($"Could not find weather for {loc}");
        }



        public static CommandResponse GetWeather(string location)
        {
            var baseUrl = "https://api.openweathermap.org/data/2.5/";
            var url = baseUrl + $"weather?appid={ApiKey}&";
            var param = "";
            //check if location is coords
            if (float.TryParse(location.Split(',')[0], out float lat))
            {
                param = $"lat={location.Split(',')[0]}&lon={location.Split(',')[1]}";
            }
            else
                param = $"zip={location},us";
            var c = Parse(url + param);
            //var client = new OpenWeatherMapClient(ApiKey);
            //var c = client.CurrentWeather.GetByZipCode(location).Result;
            if (!String.IsNullOrEmpty(c.name))
            {
                //Forecast:\n{ParseForecast($"{baseUrl}forecast?appid={ApiKey}&{param}")}
                return new CommandResponse($" Conditions for *{c.name}*:\n{c.weather[0].description} {(int)c.main.temp}°F\n", parseMode: ParseMode.Markdown)
                {
                    //ImageUrl = c.Weather.Icon,
                    //ImageDescription = $"{c.weather1} {c.temperature_string}",
                    //ImageTitle = c.place
                };
            }
            return null;
        }

        public static CommandResponse GetFknWeather(string location)
        {

            dynamic phrases = JsonConvert.DeserializeObject(new StreamReader(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\fw.json").ReadToEnd());
            var url = $"https://api.openweathermap.org/data/2.5/weather?appid={ApiKey}&";
            //check if location is coords
            if (location.Contains(","))
            {
                if (location.Split(',')[1].Contains("."))
                {
                    url += $"lat={location.Split(',')[0]}&lon={location.Split(',')[1]}";
                }
                else
                    url += $"zip={location},us";
            }
            else
                url += $"zip={location},us";
            var c = Parse(url);
            if (!String.IsNullOrEmpty(c.name))
            {

                var result = $"Conditions for *{c.name}*:\n{c.weather[0].description} {(int)c.main.temp}°F\n";
                //now the tricky bits.
                var tempF = c.main.temp;
                var tempC = 5.0 / 9.0 * (tempF - 32);
                var choices = new List<dynamic>();
                foreach (var phrase in phrases.phrases)
                {
                    //first match on conditions...
                    if (phrase.condition != null && c.weather[0].description.ToLower().Contains(phrase.condition.ToString()))
                    {
                        if (phrase.min != null)
                        {
                            var min = Int32.Parse(phrase.min.ToString());
                            if (phrase.max != null)
                            {
                                //range
                                var max = Int32.Parse(phrase.max.ToString());
                                if (tempC >= min && tempC <= max)
                                    choices.Add(phrase);
                            }
                            else
                            {
                                //only a minimum
                                if (tempC >= min)
                                    choices.Add(phrase);
                            }
                        }
                        else if (phrase.max != null)
                        {
                            //only a maximum
                            var max = Int32.Parse(phrase.max.ToString());
                            if (tempC <= max)
                                choices.Add(phrase);
                        }
                        else
                        {
                            choices.Add(phrase);
                        }

                    }

                    //next match on temp
                    if (phrase.min != null)
                    {
                        var min = Int32.Parse(phrase.min.ToString());
                        if (phrase.max != null)
                        {
                            //range
                            var max = Int32.Parse(phrase.max.ToString());
                            if (tempC >= min && tempC <= max)
                                if (!choices.Contains(phrase))
                                    choices.Add(phrase);
                        }
                        else
                        {
                            //only a minimum
                            if (tempC >= min)
                                if (!choices.Contains(phrase))
                                    choices.Add(phrase);
                        }
                    }
                    else if (phrase.max != null)
                    {
                        //only a maximum
                        var max = Int32.Parse(phrase.max.ToString());
                        if (tempC <= max)
                            if (!choices.Contains(phrase))
                                choices.Add(phrase);
                    }

                }

                //we have our choices...
                var choice = choices[Rand.Next(choices.Count)];
                result += choice.title.ToString().Replace("|", " ") + "\n" + choice.subline.ToString();
                return new CommandResponse(result, parseMode: ParseMode.Markdown)
                {
                    //ImageUrl = c.icon_url,
                    //ImageDescription = choice.title.ToString().Replace("|", " ") + "\n" + choice.subline.ToString(),
                    //ImageTitle = c.place
                };
            }
            return null;

        }

        public static string ParseForecast(string input_xml)
        {
            try
            {
                var cli = new WebClient();
                cli.Encoding = System.Text.Encoding.UTF8;
                byte[] data = Encoding.Default.GetBytes(cli.DownloadString(input_xml));
                string weather = Encoding.UTF8.GetString(data);


                var doc = XDocument.Load(cli.OpenRead(input_xml));
                var forecast = doc.Descendants("txt_forecast").Descendants("forecastday");
                var reply = "";
                for (int i = 0; i < 4; i++)
                {
                    var day = forecast.ElementAt(i);
                    if (day.Descendants().FirstOrDefault(x => x.Name == "period").Value == "0")
                    {
                        //night or day?
                        if (day.Descendants().FirstOrDefault(x => x.Name == "title").Value.Contains("Night"))
                            reply += "*Tonight*: ";
                        else
                            reply += "*Today*: ";
                    }
                    else
                    {
                        reply += $"*{day.Descendants().FirstOrDefault(x => x.Name == "title").Value}*: ";
                    }

                    reply += day.Descendants().FirstOrDefault(x => x.Name == "fcttext").Value + "\n";
                }
                return reply;
            }
            catch
            {
                return "Unable to load forecast";
            }
        }

        public static Condition Parse(string inputXml)
        {
            //Variables
            var c = new Condition();

            var cli = new WebClient { Encoding = Encoding.UTF8 };
            byte[] data = Encoding.Default.GetBytes(cli.DownloadString(inputXml));
            string weather = Encoding.UTF8.GetString(data);
            c = JsonConvert.DeserializeObject<Condition>(weather);
            c.main.temp = (float)(c.main.temp - 273.15) * 9 / 5 + 32;
            return c;
        }

    }

    public class Condition
    {
        public Coord coord { get; set; }
        public RWeather[] weather { get; set; }
        public string _base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }

    public class Coord
    {
        public float lon { get; set; }
        public float lat { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
    }

    public class Wind
    {
        public float speed { get; set; }
        public int deg { get; set; }
        public float gust { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public float message { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class RWeather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    //public class Conditions
    //{
    //    public string place = "";
    //    public string obs_time = "";
    //    public string weather1 = "";
    //    public string temperature_string = "";
    //    public string relative_humidity = "";
    //    public string wind_string = "";
    //    public string pressure_mb = "";
    //    public string dewpoint_string = "";
    //    public string visibility_km = "";
    //    public string latitude = "";
    //    public string longitude = "";
    //    public string icon_url;
    }

    [Serializable]
    public class FknWeatherResponse
    {
        public string location { get; set; }
        public string readableLocation { get; set; }
        public Temperature temperature { get; set; }
        public string remark { get; set; }
        public string flavor { get; set; }
        public Forecast forecast { get; set; }

        public class Temperature
        {
            public int c { get; set; }
            public int f { get; set; }
        }

        public class Sun
        {
            public Temperature high { get; set; }
            public Temperature low { get; set; }
            public string forecast { get; set; }
        }

        public class Mon
        {
            public Temperature high { get; set; }
            public Temperature low { get; set; }
            public string forecast { get; set; }
        }

        public class Tues
        {
            public Temperature high { get; set; }
            public Temperature low { get; set; }
            public string forecast { get; set; }
        }

        public class Wed
        {
            public Temperature high { get; set; }
            public Temperature low { get; set; }
            public string forecast { get; set; }
        }

        public class Thur
        {
            public Temperature high { get; set; }
            public Temperature low { get; set; }
            public string forecast { get; set; }
        }

        public class Fri
        {
            public Temperature high { get; set; }
            public Temperature low { get; set; }
            public string forecast { get; set; }
        }


        public class Sat
        {
            public Temperature high { get; set; }
            public Temperature low { get; set; }
            public string forecast { get; set; }
        }

        public class Day
        {
            public Temperature high { get; set; }
            public Temperature low { get; set; }
            public string forecast { get; set; }
        }

        public class Forecast
        {
            public Day Sun { get; set; }
            public Day Mon { get; set; }
            public Day Tues { get; set; }
            public Day Wed { get; set; }
            public Day Thur { get; set; }
            public Day Fri { get; set; }
            public Day Sat { get; set; }

        }
    }


