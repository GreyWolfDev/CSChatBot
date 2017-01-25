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
using Telegram.Bot.Types.Enums;

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
        public Weather(Instance db, Setting settings)
        {
            //In the module constructor, you can do some initial setup, like adding information to the database and getting input

            settings.AddField(db, "WeatherAPIKey");
            ApiKey = settings.GetString(db, "WeatherAPIKey");

            if (String.IsNullOrWhiteSpace(ApiKey))
            {
                //now ask for the API Key
                Console.Clear();
                Console.Write("What is your weather underground API key? : ");
                ApiKey = Console.ReadLine();
                settings.SetString(db, "WeatherAPIKey", ApiKey);
            }
        }

        [ChatCommand(Triggers = new[] { "w", "weather" })]
        public static CommandResponse GetWeather(CommandEventArgs args)
        {
            return GetResponse(args, false);
        }

        [ChatCommand(Triggers = new[] { "fw" })]
        public static CommandResponse GetFknWeather(CommandEventArgs args)
        {
            return GetResponse(args, true);
        }

        private static CommandResponse GetResponse(CommandEventArgs args, bool fknweather)
        {
            var loc = args.Parameters;
            if (String.IsNullOrEmpty(loc))
            {
                return new CommandResponse(String.IsNullOrEmpty(args.SourceUser.Location) ? "Please use !setloc <location> to set your default location, or enter a location as a parameter" : GetWeather(args.SourceUser.Location), parseMode: ParseMode.Markdown);
            }
            var target = args.Message.GetTarget(args.Parameters, args.SourceUser, args.DatabaseInstance);
            var location = target == args.SourceUser ? loc : target.Location;
            var weather = fknweather?GetFknWeather(location): GetWeather(location);

            return new CommandResponse(!String.IsNullOrEmpty(weather) ? weather : $"Could not find weather for {loc}", parseMode: ParseMode.Markdown);
        }

        

        public static string GetWeather(string location)
        {
            var c = Parse($"http://api.wunderground.com/api/{ApiKey}/conditions/pws:1/q/{location}.xml");
            if (!String.IsNullOrEmpty(c.place))
            {

                return ($" Conditions for {c.place}:\n{c.weather1} {c.temperature_string}\nForecast:\n{ParseForecast($"http://api.wunderground.com/api/{ApiKey}/forecast/pws:1/q/{location}.xml")}");
            }
            return null;
        }

        public static string GetFknWeather(string location)
        {

            dynamic phrases = JsonConvert.DeserializeObject(new StreamReader(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Resources\\fw.json").ReadToEnd());
            var c = Parse($"http://api.wunderground.com/api/{ApiKey}/conditions/pws:1/q/{location}.xml");
            if (!String.IsNullOrEmpty(c.place))
            {

                var result = $"Conditions for {c.place}:\n{c.weather1} {c.temperature_string}\n";
                //now the tricky bits.
                var tempF = float.Parse(c.temperature_string.Substring(0, c.temperature_string.IndexOf("F", StringComparison.Ordinal)).Trim());
                var tempC = 5.0 / 9.0 * (tempF - 32);
                var choices = new List<dynamic>();
                foreach (var phrase in phrases.phrases)
                {
                    //first match on conditions...
                    if (phrase.condition != null && c.weather1.ToLower().Contains(phrase.condition.ToString()))
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
                return result;
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

        public static Conditions Parse(string inputXml)
        {
            //Variables
            var c = new Conditions();

            var cli = new WebClient { Encoding = Encoding.UTF8 };
            byte[] data = Encoding.Default.GetBytes(cli.DownloadString(inputXml));
            string weather = Encoding.UTF8.GetString(data);

            using (XmlReader reader = XmlReader.Create(new StringReader(weather)))
            {
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.Equals("full"))
                            {
                                reader.Read();
                                c.place = (reader.Value);
                            }
                            else if (reader.Name.Equals("observation_time"))
                            {
                                reader.Read();
                                c.obs_time = reader.Value;
                            }
                            else if (reader.Name.Equals("weather"))
                            {
                                reader.Read();
                                c.weather1 = reader.Value;
                            }
                            else if (reader.Name.Equals("temperature_string"))
                            {
                                reader.Read();
                                c.temperature_string = reader.Value;
                            }
                            else if (reader.Name.Equals("relative_humidity"))
                            {
                                reader.Read();
                                c.relative_humidity = reader.Value;
                            }
                            else if (reader.Name.Equals("wind_string"))
                            {
                                reader.Read();
                                c.wind_string = reader.Value;
                            }
                            else if (reader.Name.Equals("pressure_mb"))
                            {
                                reader.Read();
                                c.pressure_mb = reader.Value;
                            }
                            else if (reader.Name.Equals("dewpoint_string"))
                            {
                                reader.Read();
                                c.dewpoint_string = reader.Value;
                            }
                            else if (reader.Name.Equals("visibility_km"))
                            {
                                reader.Read();
                                c.visibility_km = reader.Value;
                            }
                            else if (reader.Name.Equals("latitude"))
                            {
                                reader.Read();
                                c.latitude = reader.Value;
                            }
                            else if (reader.Name.Equals("longitude"))
                            {
                                reader.Read();
                                c.longitude = reader.Value;
                            }

                            break;
                    }
                }
            }
            return c;
        }

    }

    public class Conditions
    {
        public string place = "";
        public string obs_time = "";
        public string weather1 = "";
        public string temperature_string = "";
        public string relative_humidity = "";
        public string wind_string = "";
        public string pressure_mb = "";
        public string dewpoint_string = "";
        public string visibility_km = "";
        public string latitude = "";
        public string longitude = "";
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
}
