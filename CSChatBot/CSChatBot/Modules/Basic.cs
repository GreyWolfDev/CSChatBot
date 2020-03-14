using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DB;
using DB.Extensions;
using DB.Models;
using HtmlAgilityPack;
using ModuleFramework;
using Telegram.Bot;
using Menu = ModuleFramework.Menu;

namespace CSChatBot.Modules
{
    [ModuleFramework.Module(Author = "parabola949", Name = "Basic", Version = "1.0")]
    class Basic
    {
        
        private static readonly Regex Reg = new Regex("(<h3 class=\"r\">)(.*?)(</h3>)", RegexOptions.IgnoreCase);
        public Basic(Instance instance, Setting setting, TelegramBotClient bot)
        {

        }

        [ChatCommand(Triggers = new[] { "google", "g", "lmgtfy" }, HideFromInline = true, Parameters = new[] { "<your search>" })]
        public static CommandResponse Google(CommandEventArgs args)
        {
            if (String.IsNullOrEmpty(args.Parameters)) return new CommandResponse("");
            var data = new WebClient().DownloadString($"https://www.google.com/search?q={args.Parameters}");
            var matches = Reg.Matches(data);
            try
            {
                var searchResults = new Dictionary<string, string>();
                foreach (Match result in matches)
                {
                    if (result.Value.Contains("a class=\"sla\"")) continue;
                    if (result.Value.Contains("<a href=\"/search?q=")) continue;
                    var start = result.Value.Replace("<h3 class=\"r\"><a href=\"/url?q=", "");
                    var url = start.Substring(0, start.IndexOf("&amp;sa=U"));
                    var title = start.Substring(start.IndexOf(">") + 1);
                    title = title.Replace("<b>", "").Replace("</b>", "").Replace("</a></h3>", "");
                    title = System.Web.HttpUtility.HtmlDecode(title);
                    searchResults.Add(title, url);
                    if (searchResults.Count >= 5) break;
                }
                var menu = new Menu();
                foreach (var result in searchResults)
                    menu.Buttons.Add(new InlineButton(result.Key, url: result.Value));
                return new CommandResponse("Here are the results for " + args.Parameters, menu: menu);
            }
            catch (Exception e)
            {
                return new CommandResponse("Sorry, I wasn't able to pull the results.  Send this to Para: " + e.Message);
            }
        }

        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        [ChatCommand(Triggers = new[] { "roll" }, HelpText = "Rolls dice")]
        public static CommandResponse Roll(CommandEventArgs e)
        {
            if (!e.Parameters.Any())
            {
                return new CommandResponse("Wrong syntax");
            }
            //get what they want to roll
            var inputs = e.Parameters.Split(' ').Select(x => x.ToLower());
            var result = "";
            foreach (var input in inputs)
            {
                //how many times?
                var temp = new List<int>();

                var times = int.Parse(input.Substring(0, input.IndexOf("d")));
                var sides = int.Parse(Regex.Match(input, "(d\\d*)").Value.Substring(1));
                for (var i = 0; i < times; i++)
                    temp.Add(RollDice((byte)sides));
                if (input.Contains("k"))
                {
                    if (input.Contains("kl"))
                    {
                        //keep lowest how many?
                        var keep = int.Parse(input.Substring(input.IndexOf("kl") + 2));
                        result += $"Set: {temp.Aggregate("", (c, v) => $"{v} {c}")}\n";
                        temp = temp.OrderBy(x => x).Take(keep).ToList();
                        result += $"Keep {keep} lowest: {temp.Aggregate("", (c, v) => $"{v} {c}")}\n" +
                            $"Total: {temp.Sum(x => x)}\n\n";
                    }
                    else
                    {
                        var t = input.Replace("h", "");
                        var keep = int.Parse(t.Substring(t.IndexOf("k") + 1));
                        result += $"Set: {temp.Aggregate("", (c, v) => $"{v} {c}")}\n";
                        temp = temp.OrderByDescending(x => x).Take(keep).ToList();
                        result += $"Keep {keep} highest: {temp.Aggregate("", (c, v) => $"{v} {c}")}\n" +
                            $"Total: {temp.Sum(x => x)}\n\n";
                    }
                }
                else
                {
                    result += $"Set: {temp.Aggregate("", (c, v) => $"{v} {c}")}\n" +
                        $"Total: {temp.Sum(x => x)}\n\n";
                }
            }



            return new CommandResponse(result);
        }

        // This method simulates a roll of the dice. The input parameter is the
        // number of sides of the dice.

        public static byte RollDice(byte numberSides)
        {
            if (numberSides <= 0)
                throw new ArgumentOutOfRangeException("numberSides");
            byte[] randomNumber = new byte[1];
            do
            {
                rngCsp.GetBytes(randomNumber);
            }
            while (!IsFairRoll(randomNumber[0], numberSides));
            return (byte)((randomNumber[0] % numberSides) + 1);
        }

        private static bool IsFairRoll(byte roll, byte numSides)
        {
            int fullSetsOfValues = Byte.MaxValue / numSides;
            return roll < numSides * fullSetsOfValues;
        }
    }
}
