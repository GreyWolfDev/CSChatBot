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
            settings.AddField(db, "ClarifaiAppId");
            var ClarifaiAppId = settings.GetString(db, "ClarifaiAppId");
            var r = new Random();
            if (String.IsNullOrWhiteSpace(ClarifaiAppId))
            {
                //now ask for the API Key
                Console.Clear();
                Console.Write("What is your Clarifai App Id? : ");
                ClarifaiAppId = Console.ReadLine();
                settings.SetString(db, "ClarifaiAppId", ClarifaiAppId);
            }
            if (String.IsNullOrEmpty(ClarifaiAppId)) return;

            settings.AddField(db, "ClarifaiAppSecret");
            var ClarifaiAppSecret = settings.GetString(db, "ClarifaiAppSecret");
            if (String.IsNullOrWhiteSpace(ClarifaiAppSecret))
            {
                //now ask for the API Key
                Console.Clear();
                Console.Write("What is your Clarifai App Secret? : ");
                ClarifaiAppSecret = Console.ReadLine();
                settings.SetString(db, "ClarifaiAppSecret", ClarifaiAppSecret);
            }
            if (String.IsNullOrEmpty(ClarifaiAppSecret)) return;

            bot.OnUpdate += (sender, args) =>
            {
                try
                {
                    if (args.Update?.Message?.Type == MessageType.Photo)
                    {
                        if (args.Update.Message.Chat.Type != ChatType.Private)
                        {
                            var g = db.GetGroupById(args.Update.Message.Chat.Id);
                            var nsfw = g.GetSetting<bool>("NSFW", db, false);
                            if (!nsfw) return;
                        }
                        new Task(() =>
                        {
                            var photo = args.Update.Message.Photo.OrderByDescending(x => x.Height).FirstOrDefault(x => x.FileId != null);
                            var pathing = bot.GetFileAsync(photo.FileId).Result;
                            var url =
                                $"https://api.telegram.org/file/bot{settings.TelegramBotAPIKey}/{pathing.FilePath}";
                            var nsfw = GetIsNude(url, ClarifaiAppId, ClarifaiAppSecret);
                            if (nsfw > 70)
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

        [ChatCommand(Triggers =new[] { "randofact"}, HelpText ="Generates a random \"fact\"")]
        public static CommandResponse RandoFact(CommandEventArgs e)
        {
            return new CommandResponse(RandoFactGenerator.Get());
        }

        [ChatCommand(Triggers = new[] { "togglensfw" }, GroupAdminOnly = true, HelpText = "Toggle NSFW image detection for the group")]
        public static CommandResponse ToggleNSFW(CommandEventArgs args)
        {
            var g = args.DatabaseInstance.GetGroupById(args.Message.Chat.Id);
            var nsfw = !g.GetSetting<bool>("NSFW", args.DatabaseInstance, false);
            g.SetSetting<bool>("NSFW", args.DatabaseInstance, false, nsfw);
            return new CommandResponse("NSFW: " + nsfw);
        }

        /// <summary>
        /// Gets the chance that an image is NSFW / Nude
        /// </summary>
        /// <param name="url">The url of the image</param>
        /// <returns>Percentage chance that the image is NSFW</returns>
        private int GetIsNude(string url, string appid, string secret)
        {
            var token = GetClarifaiToken(appid, secret);
            try
            {
                using (var wc = new WebClient())
                {
                    string response = "";
                    try
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                        wc.Headers.Add("Authorization", $"Bearer {token}");
                        response = wc.UploadString($"https://api.clarifai.com/v2/models/e9576d86d2004ed1a38ba0cf39ecb4b1/outputs", "POST", JsonConvert.SerializeObject(new ClarifaiInputs(url)));
                    }
                    catch
                    {
                        token = GetClarifaiToken(appid, secret, true);
                        wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                        wc.Headers.Add("Authorization", $"Bearer {token}");
                        response = wc.UploadString($"https://api.clarifai.com/v2/models/e9576d86d2004ed1a38ba0cf39ecb4b1/outputs", "POST", JsonConvert.SerializeObject(new ClarifaiInputs(url)));
                    }
                    var result = JsonConvert.DeserializeObject<ClarifaiOutput>(response);
                    return (int)(result.outputs[0].data.concepts.First(x => x.name == "nsfw").value * 100);
                }
            }
            catch (Exception e)
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
