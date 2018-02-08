using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ClashOfClans.Model;
using DB;
using DB.Extensions;
using DB.Models;
using ModuleFramework;
using Newtonsoft.Json;
using Telegram.Bot;

namespace ClashOfClans
{
    /// <summary>
    /// A sample module for CSChatBot
    /// </summary>
    [ModuleFramework.Module(Author = "parabola949", Name = "Clash of Clans", Version = "1.0")]
    public class CoC
    {
        private static string _token;
        public CoC(Instance db, Setting settings, TelegramBotClient bot)
        {
            settings.AddField(db, "CoCKey");
            var token = settings.GetString(db, "CoCKey");
            if (String.IsNullOrWhiteSpace(token))
            {
                //now ask for the API Key
                Console.Clear();
                Console.SetIn(new StreamReader(Console.OpenStandardInput(),
                    Console.InputEncoding,
                    false,
                    bufferSize: 1024));
                Console.Write("What is your Clash of Clans API key? : ");
                token = Console.ReadLine();
                settings.SetString(db, "CoCKey", token);
            }
            if (String.IsNullOrEmpty(token)) return;
            _token = token;
        }

        [ChatCommand(Triggers = new[] { "curwar", "checkwar" }, HelpText = "Gets Current War stats from CoC")]
        public static CommandResponse CurWar(CommandEventArgs args)
        {

            if (String.IsNullOrEmpty(_token)) return new CommandResponse("Ask the bot owner to set their CoC API key");
            var u = args.SourceUser;
            var clantag = u.GetSetting<string>("CoCClan", args.DatabaseInstance, null);
            
            if (String.IsNullOrEmpty(clantag))
            {
                return new CommandResponse("Please set your clan tag with /setclan");
            }

            using (var wc = new HttpClient())
            {
                wc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                var url = String.Format(UrlConstants.GetCurrentWarInformationTemplate, clantag).Replace("#", "%23");

                WarResponse war = null;
                try
                {
                    var json = wc.GetStringAsync(url).Result;
                    war = JsonConvert.DeserializeObject<WarResponse>(json);
                }
                catch (Exception e)
                {
                    return new CommandResponse($"Unable to find war information");
                }

                //let's take a quick look
                var state = war.state;
                state = char.ToUpper(state[0]) + state.Substring(1);
                var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);
                state = r.Replace(state, " ");
                if (state == "War Ended")
                {
                    if (war.clan.stars > war.opponent.stars)
                        state += " (Win)";
                    if (war.clan.stars == war.opponent.stars)
                    {
                        //get average desctruction
                        var us = war.clan.members.Sum(x => x.attacks.Sum(a => a.destructionPercentage)) / war.clan.attacks;
                        var them = war.opponent.members.Sum(x => x.attacks.Sum(a => a.destructionPercentage)) / war.opponent.attacks;
                        if (us > them)
                            state += " (Win)";
                        if (us == them)
                            state += " (Draw)";
                        if (us < them)
                            state += " (Lose)";
                    }
                    if (war.clan.stars < war.opponent.stars)
                        state += " (Lose)";
                }
                var response = $"Stage: {state}" +
                               $"\n{war.teamSize} v {war.teamSize}" +
                               $"\n----------------------------------" +
                               $"\n{war.clan.name}: {war.clan.stars} ⭐️" +
                               $"\n{war.clan.attacks} attacks made out of {war.teamSize * 2}" +
                               $"\n----------------------------------" +
                               $"\n{war.opponent.name}: {war.opponent.stars} ⭐️" +
                               $"\n{war.opponent.attacks} attacks made out of {war.teamSize * 2}" +
                               $"\n----------------------------------" +
                               $"\nMembers:" +
                               $"\n----------------------------------";

                foreach (var member in war.clan.members.OrderBy(x => x.mapPosition))
                {
                    response += $"\n{member.name}" +
                             $"\n{member.attacks?.Length ?? 0} ⚔️ - {member.attacks?.Sum(x => x.stars) ?? 0} ⭐️ ({member.attacks?.Average(x => x.destructionPercentage) ?? 0:N0}% damage)" +
                             $"\n--------------------";
                }


                return new CommandResponse(response)
                {
                    ImageUrl = war.clan.badgeUrls.medium,
                    ImageDescription = $"{war.clan.name} v {war.opponent.name}",
                    ImageTitle = state
                };
            }

        }

        [ChatCommand(Triggers = new[] { "setclan" }, HideFromInline = true,
            HelpText = "Sets your clan tag for Clash of Clans")]
        public static CommandResponse SetClan(CommandEventArgs args)
        {
            if (String.IsNullOrEmpty(_token)) return new CommandResponse("Ask the bot owner to set their CoC API key");

            var u = args.SourceUser;

            var input = args.Parameters.Trim();
            if (!input.StartsWith("#"))
                input = "#" + input;


            //Validate clan tag
            using (var wc = new HttpClient())
            {
                wc.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                var url = String.Format(UrlConstants.GetClanInformationUrlTemplate, input).Replace("#", "%23");

                ClanResponse clan = null;
                try
                {
                    var json = wc.GetStringAsync(url).Result;
                    clan = JsonConvert.DeserializeObject<ClanResponse>(json);
                }
                catch (Exception e)
                {
                    return new CommandResponse($"Unable to find clan '{input}'");
                }

                if (clan.reason != null)
                {
                    return new CommandResponse($"Unable to find clan '{input}'.  Reason: {clan.reason}");
                }

                u.SetSetting<string>("CoCClan", args.DatabaseInstance, "", input);
                var leaders = clan.memberList.Where(x => x.role == "leader").Aggregate("", (a, b) => a + "\n" + b.name);
                return new CommandResponse($"Clan found and set:\n{clan.name}\n{clan.members} Members, led by{leaders}");
            }
        }
    }
}
