using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DB;
using DB.Extensions;
using DB.Models;
using ModuleFramework;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Telegram.Bot.Types.InlineQueryResults;
using Misc.Models;
using Misc.Interfaces;
// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006 // Naming Styles
namespace Misc
{
    /// <summary>
    /// Misc stuff
    /// </summary>
    [Module(Author = "parabola949", Name = "Misc", Version = "1.0")]
    public class Misc
    {
        public Misc(Instance db, Setting settings, TelegramBotClient bot)
        {
            RandoFactGenerator.Init();
            settings.AddField(db, "ClarifaiToken");
            var ClarifaiAppId = settings.GetString(db, "ClarifaiToken");
            var r = new Random();
            if (String.IsNullOrWhiteSpace(ClarifaiAppId))
            {
                //now ask for the API Key
                Console.Clear();
                Console.Write("What is your Clarifai Token? : ");
                ClarifaiAppId = Console.ReadLine();
                settings.SetString(db, "ClarifaiToken", ClarifaiAppId);
            }
            if (String.IsNullOrEmpty(ClarifaiAppId)) return;

            //settings.AddField(db, "ClarifaiAppSecret");
            //var ClarifaiAppSecret = settings.GetString(db, "ClarifaiAppSecret");
            //if (String.IsNullOrWhiteSpace(ClarifaiAppSecret))
            //{
            //    //now ask for the API Key
            //    Console.Clear();
            //    Console.Write("What is your Clarifai App Secret? : ");
            //    ClarifaiAppSecret = Console.ReadLine();
            //    settings.SetString(db, "ClarifaiAppSecret", ClarifaiAppSecret);
            //}
            //if (String.IsNullOrEmpty(ClarifaiAppSecret)) return;

            bot.OnUpdate += (sender, args) =>
            {
                try
                {
                    if (!(args.Update.Message?.Date > DateTime.UtcNow.AddSeconds(-15)))
                    {
                        //Log.WriteLine("Ignoring message due to old age: " + update.Message.Date);
                        return;
                    }
                    if (args.Update?.Message?.Type == MessageType.Photo)
                    {
                        var bannsfw = false;
                        var warnnsfw = false;
                        var g = db.GetGroupById(args.Update.Message.Chat.Id);
                        if (args.Update.Message.Chat.Type != ChatType.Private)
                        {

                            var nsfw = g.GetSetting<bool>("NSFW", db, false);
                            bannsfw = g.GetSetting<bool>("BANNSFW", db, false);
                            warnnsfw = g.GetSetting<bool>("WARNNSFW", db, false);
                            if (!nsfw & !bannsfw & !warnnsfw) return;
                        }
                        new Task(() =>
                        {
                            var photo = args.Update.Message.Photo.OrderByDescending(x => x.Height).FirstOrDefault(x => x.FileId != null);
                            var pathing = bot.GetFileAsync(photo.FileId).Result;
                            var url =
                                $"https://api.telegram.org/file/bot{settings.TelegramBotAPIKey}/{pathing.FilePath}";
                            var nsfw = GetIsNude(url, ClarifaiAppId);
                            Console.WriteLine($"Image NSFW: {nsfw}%");
                            if (nsfw > 70)
                            {
                                if (!bannsfw & !warnnsfw)
                                {
                                    var responses = new[]
                                     {
                                    "rrrrf",
                                    "Hot!",
                                    "OMAI",
                                    "*mounts*",
                                    "oh fuck the hell yes....",
                                    "mmf.  *excuses herself for a few minutes*",
                                    "Yes, I'll take one, to go..  to my room...",
                                    "Keep em cumming...",
                                };
                                    bot.SendTextMessageAsync(args.Update.Message.Chat.Id,
                                        responses[r.Next(responses.Length)], replyToMessageId: args.Update.Message.MessageId);
                                }
                                else
                                {

                                    var chat = g.GetSetting<string>("NSFWLogChat", db, null);
                                    if (chat != null)
                                    {
                                        try
                                        {
                                            var result = bot.ForwardMessageAsync(chat, args.Update.Message.Chat.Id, args.Update.Message.MessageId).Result;
                                        }
                                        catch (Exception e)
                                        {
                                            while (e.InnerException != null)
                                                e = e.InnerException;
                                            bot.SendTextMessageAsync(args.Update.Message.Chat.Id, e.Message + ": " + chat);
                                        }
                                    }
                                    if (bannsfw)
                                    {
                                        //ban the user
                                        bot.KickChatMemberAsync(args.Update.Message.Chat.Id, args.Update.Message.From.Id);
                                    }
                                    else if (warnnsfw)
                                    {
                                        bot.SendTextMessageAsync(args.Update.Message.Chat.Id, "This is a SFW channel.  Please do not post such images here.");
                                    }
                                    bot.DeleteMessageAsync(args.Update.Message.Chat.Id, args.Update.Message.MessageId);
                                }

                                //download it
                                Directory.CreateDirectory("nsfw");
                                var name = "nsfw\\" + pathing.FilePath.Substring(pathing.FilePath.LastIndexOf("/") + 1);
                                new WebClient().DownloadFileAsync(new Uri(url), name);
                            }
                        }).Start();
                    }
                }
                catch
                {
                    // ignored
                }
            };
        }

        [ChatCommand(Triggers = new[] { "randofact" }, HelpText = "Generates a random \"fact\"")]
        public static CommandResponse RandoFact(CommandEventArgs e)
        {
            return new CommandResponse(RandoFactGenerator.Get());
        }

        public static ICovidDataLoader Loader = new WorldoMeterDataLoader();


        [ChatCommand(Triggers = new[] { "covid" }, HelpText = "Get covid stats.  Add a country or state on the end to filter", Parameters = new[] { "[country] | [state] | null" })]
        public static CommandResponse Covid(CommandEventArgs e)
        {
            //var data = CovidData.LoadData();
            //CountryStats c = null;
            //if (!String.IsNullOrWhiteSpace(e.Parameters))
            //{
            //    c = data.FirstOrDefault(x => x.Country.ToLower().StartsWith(e.Parameters.ToLower()));
            //}
            //if (c == null)
            //    c = data.FirstOrDefault(x => x.Country.StartsWith("Global"));

            //return new CommandResponse($"{c.Country}:\n{c.TotalCases:n0} Total\n" +
            //    $"{c.NewCases:n0} New cases\n" +
            //    $"{c.TotalDeaths:n0} Total deaths\n" +
            //    $"{c.NewDeaths:n0} New deaths\n" +
            //    $"{c.TotalRecovered:n0} Total recovered\n" +
            //    $"{c.ActiveCases:n0} Active cases\n" +
            //    $"{c.SeriousCases:n0} Serious cases\n" +
            //    $"{c.TotalCasesPerMillion:n0} Total cases / 1M pop\n" +
            //    $"{((double)c.TotalDeaths * 100 / (double)(c.TotalDeaths + c.TotalRecovered)):n2}% Estimated mortality rate");

            RegionData c = null;
            if (!String.IsNullOrWhiteSpace(e.Parameters))
            {
                c = Loader.GetStats(e.Parameters);
            }
            if (c == null)
                c = Loader.GetStats("Global");

            var result = $"{c.Country}";
            if (!String.IsNullOrWhiteSpace(c.State))
                result += $" - {c.State}";

            result += $":\n" +
            $"*{c.TotalCases:n0}* Total\n" +
                $"*{c.NewCases:n0}* New cases\n" +
                $"*{c.TotalDeaths:n0}* Total deaths\n" +
                $"*{c.NewDeaths:n0}* New deaths\n" +
                $"*{c.TotalRecovered:n0}* Total recovered\n" +
                $"*{c.ActiveCases:n0}* Active cases\n";
            if (c.SeriousCases != -1)
                result += $"*{c.SeriousCases:n0}* Serious cases\n";

            if (c.TotalCasesPerMillion != 0)
                result += $"*{c.TotalCasesPerMillion:n0}* Total cases / 1M pop ({c.TotalCasesPerMillion / 1000000:n6}%)\n";

            result += $"\n\"New\" refers to any new cases since\nmidnight UTC ({(DateTime.UtcNow - DateTime.UtcNow.Date).Hours} hours ago)\n";
            result += $"[Source]({c.Source})";
            //if (c.Deaths > 10 && c.Recovered > 0)
            //    result += $"{((double)c.Deaths * 100 / (double)(c.Deaths + c.Recovered)):n2}% Estimated mortality rate";
            var location = c.Country;
            if (c.State != null) location += " - " + c.State;
            return new CommandResponse(result)
            {
                ImageUrl = "https://www.worldometers.info/img/worldometers-fb.jpg",
                ImageDescription = location,
                ImageTitle = "Covid 19 Stats",
                ParseMode = ParseMode.Markdown,
                PreviewHtml = false
            };
        }

       




        private static Stream GetStreamFromUrl(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(url);

            return new MemoryStream(imageData);
        }

        [ChatCommand(Triggers = new[] { "togglensfw" }, GroupAdminOnly = true, HelpText = "Toggle NSFW image detection for the group")]
        public static CommandResponse ToggleNSFW(CommandEventArgs args)
        {
            var g = args.DatabaseInstance.GetGroupById(args.Message.Chat.Id);
            var nsfw = !g.GetSetting<bool>("NSFW", args.DatabaseInstance, false);
            g.SetSetting<bool>("NSFW", args.DatabaseInstance, false, nsfw);
            return new CommandResponse("NSFW: " + nsfw);
        }

        [ChatCommand(Triggers = new[] { "warnnsfw" }, GroupAdminOnly = true, HelpText = "Warn NSFW posts in the channel")]
        public static CommandResponse WarnNSFW(CommandEventArgs args)
        {
            var g = args.DatabaseInstance.GetGroupById(args.Message.Chat.Id);
            var nsfw = !g.GetSetting<bool>("WARNNSFW", args.DatabaseInstance, false);
            g.SetSetting<bool>("WARNNSFW", args.DatabaseInstance, false, nsfw);
            var chat = g.GetSetting<string>("NSFWLogChat", args.DatabaseInstance, null);
            if (chat == null)
            {
                args.Bot.SendTextMessageAsync(args.Message.Chat.Id, "You also can set a chat to log images detected using /setnsfwlog <chatid>");
            }
            return new CommandResponse("Warn NSFW: " + nsfw);
        }

        [ChatCommand(Triggers = new[] { "bannsfw" }, GroupAdminOnly = true, HelpText = "Ban NSFW posts from the channel")]
        public static CommandResponse BanNSFW(CommandEventArgs args)
        {
            var g = args.DatabaseInstance.GetGroupById(args.Message.Chat.Id);
            var nsfw = !g.GetSetting<bool>("BANNSFW", args.DatabaseInstance, false);
            g.SetSetting<bool>("BANNSFW", args.DatabaseInstance, false, nsfw);
            var chat = g.GetSetting<string>("NSFWLogChat", args.DatabaseInstance, null);
            if (chat == null)
            {
                args.Bot.SendTextMessageAsync(args.Message.Chat.Id, "You also can set a chat to log images detected using /setnsfwlog <chatid>");
            }
            return new CommandResponse("Ban NSFW: " + nsfw);
        }

        [ChatCommand(Triggers = new[] { "setnsfwlog" }, GroupAdminOnly = true, HelpText = "Ban NSFW posts from the channel")]
        public static CommandResponse SetNSFWLog(CommandEventArgs args)
        {
            var split = args.Message.Text.Split(' ');
            if (split.Length == 1)
            {
                return new CommandResponse("You need to supply a group id: /setnsfwlog <chatid>");
            }
            if (!long.TryParse(split[1], out long chatId))
            {
                return new CommandResponse("You need to supply a group id: /setnsfwlog <chatid>");
            }
            var g = args.DatabaseInstance.GetGroupById(args.Message.Chat.Id);

            g.SetSetting<string>("NSFWLogChat", args.DatabaseInstance, null, chatId.ToString());
            return new CommandResponse("Logging NSFW images to: " + chatId);
        }

        /// <summary>
        /// Gets the chance that an image is NSFW / Nude
        /// </summary>
        /// <param name="url">The url of the image</param>
        /// <returns>Percentage chance that the image is NSFW</returns>
        private int GetIsNude(string url, string token)
        {
            //var token = GetClarifaiToken(appid, secret);
            try
            {
                using (var wc = new WebClient())
                {
                    string response = "";
                    try
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                        wc.Headers.Add("Authorization", $"Key {token}");
                        response = wc.UploadString($"https://api.clarifai.com/v2/models/e9576d86d2004ed1a38ba0cf39ecb4b1/outputs", "POST", JsonConvert.SerializeObject(new ClarifaiInputs(url)));
                        var result = JsonConvert.DeserializeObject<ClarifaiOutput>(response);
                        return (int)(result.outputs[0].data.concepts.First(x => x.name == "nsfw").value * 100);
                    }
                    catch
                    {
                        //token = GetClarifaiToken(appid, secret, true);
                        //wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                        //wc.Headers.Add("Authorization", $"Bearer {secret}");
                        //response = wc.UploadString($"https://api.clarifai.com/v2/models/e9576d86d2004ed1a38ba0cf39ecb4b1/outputs", "POST", JsonConvert.SerializeObject(new ClarifaiInputs(url)));
                        return 0;
                    }

                }
            }
            catch
            {
                // ignored
                return 0;
            }
        }

        private string _clarifaiToken = "";

        private string GetClarifaiToken(string id, string secret, bool renew = false)
        {
            if (String.IsNullOrEmpty(_clarifaiToken) || renew)
            {
                using (var client = new HttpClient())
                {
                    var url = "https://api.clarifai.com/v2/token";
                    var content = new StringContent(JsonConvert.SerializeObject("\"grant_type\":\"client_credentials\""), Encoding.UTF8, "application/json");
                    String encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(id + ":" + secret));
                    client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {encoded}");
                    var response = client.PostAsync(url, content).Result;
                    response.EnsureSuccessStatusCode();
                    var res = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<ClarifaiAPIKey>(res);
                    if (result.status.code == 10000)
                    {
                        _clarifaiToken = result.access_token;
                    }
                }
            }
            return _clarifaiToken;
        }

        [ChatCommand(Triggers = new[] { "think" }, HelpText = "Checks to see what the internet thinks", Parameters = new[] { "<topic>" })]
        public static CommandResponse InternetThink(CommandEventArgs args)
        {
            var result = GetResult(args.Parameters);
            //return message
            return new CommandResponse(result.conclusion, parseMode: ParseMode.Html);
        }

        static ThinkResult GetResult(string query)
        {
            using (var wc = new WebClient())
            {
                wc.Headers.Add("X-Requested-With", "XMLHttpRequest");
                return JsonConvert.DeserializeObject<ThinkResult>(wc.DownloadString($"http://www.whatdoestheinternetthink.net/core/client.php?query={query}&searchtype=1"));
            }
        }
    }

    public class ClarifaiAPIKey
    {
        public Status status { get; set; }
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string scope { get; set; }
    }


    public class ClarifaiOutput
    {
        public Status status { get; set; }
        public Output[] outputs { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string description { get; set; }
    }

    public class Output
    {
        public string id { get; set; }
        public Status1 status { get; set; }
        public DateTime created_at { get; set; }
        public Model model { get; set; }
        public Input input { get; set; }
        public Data1 data { get; set; }
    }

    public class Status1
    {
        public int code { get; set; }
        public string description { get; set; }
    }

    public class Model
    {
        public string name { get; set; }
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public object app_id { get; set; }
        public Output_Info output_info { get; set; }
        public Model_Version model_version { get; set; }
    }

    public class Output_Info
    {
        public string message { get; set; }
        public string type { get; set; }
    }

    public class Model_Version
    {
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public Status2 status { get; set; }
    }

    public class Status2
    {
        public int code { get; set; }
        public string description { get; set; }
    }

    public class Input
    {
        public string id { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public Image image { get; set; }
    }

    public class Image
    {
        public string url { get; set; }
    }

    public class Data1
    {
        public Concept[] concepts { get; set; }
    }

    public class Concept
    {
        public string id { get; set; }
        public string name { get; set; }
        public object app_id { get; set; }
        public float value { get; set; }
    }


    public class ClarifaiInputs
    {
        public InputIn[] inputs { get; set; }

        public ClarifaiInputs(string url)
        {
            inputs = new[]
            {
                new InputIn()
                {
                    data = new DataIn()
                    {
                        image = new ImageIn()
                        {
                            url = url
                        }
                    }
                }
            };
        }
    }

    public class InputIn
    {
        public DataIn data { get; set; }
    }

    public class DataIn
    {
        public ImageIn image { get; set; }
    }

    public class ImageIn
    {
        public string url { get; set; }
    }


    public class ThinkResult
    {
        public int success { get; set; }
        public int status { get; set; }
        public int searchtype { get; set; }
        public string searchtype_named { get; set; }
        public Result[] results { get; set; }
        public string source { get; set; }
        public string conclusion { get; set; }
        public string result_url { get; set; }
    }

    public class Result
    {
        public string id { get; set; }
        public string search { get; set; }
        public string winner { get; set; }
        public string positive { get; set; }
        public string negative { get; set; }
        public string indifferent { get; set; }
        public string date { get; set; }
        public string amount { get; set; }
        public string results { get; set; }
        public string source { get; set; }
    }
}
