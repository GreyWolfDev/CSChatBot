using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DB;
using DB.Models;
using ModuleFramework;
using Newtonsoft.Json;
using Telegram.Bot;

namespace XKCD
{
    /// <summary>
    /// A sample module for CSChatBot
    /// </summary>
    [ModuleFramework.Module(Author = "parabola949", Name = "XKCD", Version = "1.0")]
    public class XKCD
    {
        internal static Random R = new Random();
        private TelegramBotClient _bot;
        public XKCD(Instance db, Setting settings, TelegramBotClient bot)
        {
            _bot = bot;
        }

        [ChatCommand(Triggers = new[] { "xkcd" }, DontSearchInline = true, HelpText = "This is a sample command")]
        public static CommandResponse GetXkcd(CommandEventArgs args)
        {
            XkcdPost chosen;
            var current = JsonConvert.DeserializeObject<XkcdPost>(
                    new WebClient().DownloadString("https://xkcd.com/info.0.json"));
            if (String.IsNullOrEmpty(args.Parameters))
            {
                //get the current XKCD
                
                chosen = JsonConvert.DeserializeObject<XkcdPost>(
                    new WebClient().DownloadString($"https://xkcd.com/{R.Next(current.num)}/info.0.json"));


            }
            else
            {
                var num = 0;
                if (int.TryParse(args.Parameters, out num))
                {
                    chosen =
                        JsonConvert.DeserializeObject<XkcdPost>(
                            new WebClient().DownloadString($"https://xkcd.com/{Math.Min(current.num, num)}/info.0.json"));
                }
                else
                {
                    if (String.Equals(args.Parameters, "new", StringComparison.InvariantCultureIgnoreCase))
                    {
                        chosen = current;
                    }
                    else
                    {
                        //search
                        var url = $"https://www.google.com/search?q={args.Parameters} inurl:https://xkcd.com";
                        var page = new WebClient { UseDefaultCredentials = true }.DownloadString(url);

                        page = page.Substring(page.IndexOf("<div id=\"search\">"));
                        page = page.Substring(page.IndexOf("q=") + 2);
                        page = page.Substring(0, page.IndexOf("/&amp")).Replace("https://xkcd.com/", "");
                        chosen =
                        JsonConvert.DeserializeObject<XkcdPost>(
                            new WebClient().DownloadString($"https://xkcd.com/{page}/info.0.json"));
                    }
                }
            }


            return new CommandResponse($"{chosen.title}\n{chosen.alt}\n{chosen.img}");
        }
    }


    public class XkcdPost
    {
        public string month { get; set; }
        public int num { get; set; }
        public string link { get; set; }
        public string year { get; set; }
        public string news { get; set; }
        public string safe_title { get; set; }
        public string transcript { get; set; }
        public string alt { get; set; }
        public string img { get; set; }
        public string title { get; set; }
        public string day { get; set; }
    }

}
