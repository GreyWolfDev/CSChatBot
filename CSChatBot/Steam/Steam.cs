using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DB;
using DB.Models;
using ModuleFramework;
using Telegram.Bot;
using DB.Extensions;
using System.Net;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Steam
{
    /// <summary>
    /// A sample module for CSChatBot
    /// </summary>
    [Module(Author = "parabola949", Name = "Steam", Version = "1.0")]
    public class Steam
    {
        private static string _steamKey;
        public Steam(Instance db, Setting settings, TelegramBotClient bot)
        {
            settings.AddField(db, "SteamKey");
            var steamKey = settings.GetString(db, "SteamKey");
            if (String.IsNullOrWhiteSpace(steamKey))
            {
                //now ask for the API Key
                Console.Clear();
                Console.Write("What is your Steam API key? : ");
                steamKey = Console.ReadLine();
                settings.SetString(db, "SteamKey", steamKey);
            }
            if (String.IsNullOrEmpty(steamKey)) return;
            _steamKey = steamKey;
        }

        [ChatCommand(Triggers = new[] { "setsteamid" }, HideFromInline = true, HelpText = "Sets your steam id in the database")]
        public static CommandResponse SetSteamId(CommandEventArgs args)
        {
            if (String.IsNullOrEmpty(_steamKey)) return new CommandResponse("Ask an admin to set their Steam API key");

            var u = args.SourceUser;

            var input = args.Parameters;
            if (input.Contains(" "))
                input = input.Split(' ')[0];

            if (input.Contains("/id/"))
            {
                input = input.Substring(input.IndexOf("/id/") + 4);
                input = input.Replace("/", "");
            }

            if (long.TryParse(input, out var id))
            {
                u.SetSetting<string>("SteamId", args.DatabaseInstance, "", id.ToString());
                return new CommandResponse($"User id set to {id}");
            }
            //attempt to get the users id64
            using (var wc = new WebClient())
            {
                var build = new Builder(_steamKey);
                var userid = JsonConvert.DeserializeObject<ResolveVanityUrlResponse>(wc.DownloadString(build.CreateIDFinder(input))).response;
                if (userid.success != 1)
                {
                    return new CommandResponse($"Unable to find steam user '{input}'.  Please try using your steam profile url: `https://steamcommunity.com/id/<username/` or the <username> from that url.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                }

                u.SetSetting<string>("SteamId", args.DatabaseInstance, "", userid.steamid);
                return new CommandResponse($"User id found and set to {userid.steamid}");
            }
        }

        [ChatCommand(Triggers = new[] { "steam" }, HelpText = "Shows your steam account summary")]
        public static CommandResponse GetSteam(CommandEventArgs args)
        {
            if (String.IsNullOrEmpty(_steamKey)) return new CommandResponse("Ask an admin to set their Steam API key");



            var user = args.Message.GetTarget(args.Parameters, args.SourceUser, args.DatabaseInstance);
            var id = user.GetSetting<string>("SteamId", args.DatabaseInstance, "");
            if (user.UserId == args.SourceUser.UserId & !String.IsNullOrEmpty(args.Parameters))
            {
                //they are looking up a specific user, let's try to find the id
                //attempt to get the users id64
                using (var wc = new WebClient())
                {
                    var build = new Builder(_steamKey);
                    var userid = JsonConvert.DeserializeObject<ResolveVanityUrlResponse>(wc.DownloadString(build.CreateIDFinder(args.Parameters))).response;
                    if (userid.success == 1)
                    {
                        id = userid.steamid;
                    }
                }
            }

            if (String.IsNullOrEmpty(id))
            {
                return new CommandResponse($"Please set your steamid using /setsteamid <your profile url>");
            }



            using (var wc = new WebClient())
            {
                var build = new Builder(_steamKey);
                var url = build.CreateSummaryUrl(id);
                var steamuser = JsonConvert.DeserializeObject<PlayerSummaries>(wc.DownloadString(url)).response?.players?.player[0];
                if (steamuser == null)
                    return new CommandResponse("Could not load user info");
                //build our response
                var response = $"{steamuser.personaname}\n";
                var pub = steamuser.communityvisibilitystate == 3;
                var description = UnixTimeStampToDateTime(steamuser.timecreated).ToString();
                if (!pub)
                {
                    response += $"{steamuser.profileurl}\nThis profile is set to private, cannot get any more information.";
                }
                else
                {


                    var isPlaying = !String.IsNullOrEmpty(steamuser.gameextrainfo);
                    response += $"Profile created: {UnixTimeStampToDateTime(steamuser.timecreated)}\n";
                    if (isPlaying)
                    {
                        response += $"Currently Playing: {steamuser.gameextrainfo}\n";
                        description = $"Currently Playing: {steamuser.gameextrainfo}\n";
                        var game = JsonConvert.DeserializeObject<SchemaResponse>(wc.DownloadString(build.CreateSchemaUrl(steamuser.gameid))).game;
                        response += $"http://store.steampowered.com/app/" + steamuser.gameid + "\n";
                        try
                        {
                            url = build.CreateUserGameStatUrl(steamuser.steamid, steamuser.gameid);
                            var stats = JsonConvert.DeserializeObject<UserGameStatResponse>(wc.DownloadString(url)).playerstats;
                            if (game.availableGameStats?.stats != null)
                            {
                                var gstats = game.availableGameStats.stats;
                                if (gstats.Any())
                                {
                                    response += "\nA few stats:\n";
                                    foreach (var s in gstats.Where(x => stats.stats.Any(y => x.name == y.name)).Take(5))
                                    {
                                        if (stats.stats.Any(x => x.name == s.name))
                                        {
                                            response += $"{(String.IsNullOrEmpty(s.displayName) ? s.name : s.displayName)}: ";
                                            response += stats.stats.FirstOrDefault(x => x.name == s.name).value + "\n";
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }

                    response += $"\nUser Profile: {steamuser.profileurl}\n";

                    //get games list
                    var u = build.CreateGameListUrl(id);
                    var gameList = JsonConvert.DeserializeObject<GameList>(wc.DownloadString(u)).response;
                    response += $"\nUser has {gameList.game_count} games in library\n";
                    description += $"\n{gameList.game_count} games in library\n";
                    gameList = JsonConvert.DeserializeObject<GameList>(wc.DownloadString(build.CreateRecentGameListUrl(id))).response;

                    //STEAM DB is being a jerk and not allowing bots through...
                    //trying to do it myself, however the api keeps returning Internal Server Errors on most games, but not all....  USELESS!
                    //var nonFreeGameList = JsonConvert.DeserializeObject<GameList>(wc.DownloadString(u.Replace("&include_played_free_games=1", ""))).response;
                    //var total = 0;
                    //foreach (var g in nonFreeGameList.games)
                    //{
                    //    u = build.CreateGamePriceUrl(gameList.games[0].appid);
                    //    try
                    //    {
                    //        var prices = JsonConvert.DeserializeObject<GamePrices>(wc.DownloadString(u));
                    //        total += prices.result.assets.
                    //            }
                    //    catch
                    //    {

                    //    }
                    //}


                    //GetAccountPricing(id);
                    //response += $"Total value of game library: {prices.total}\n" +
                    //            $"Total hours played: {prices.hours}\n\n";


                    if (gameList.games != null)
                    {
                        response += "Most recent games\n----------------------------\n";
                        foreach (var g in gameList.games)
                        {
                            response += $"{g.name} - {(double)g.playtime_forever / (double)60:0.##} Hours Total\n";
                        }
                    }
                }

                return new CommandResponse(response) { ImageUrl = steamuser.avatar, ImageDescription = description, ImageTitle = steamuser.personaname };
            }




        }

        private static dynamic GetAccountPricing(string id)
        {

            string page = "";
            using (var client = new CookieAwareWebClient())
            {
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
                page = client.DownloadString($"https://steamdb.info/calculator/{id}/?cc=us");
            }
            var total = page.Substring(page.IndexOf("<b class=\"number-price\">") + "<b class=\"number-price\">".Length);
            total = total.Substring(0, total.IndexOf("</b>"));
            var hourRegex = new Regex("(<b>)(.*)\\n(<em>Hours on record</em>)");
            var hours = hourRegex.Match(page).Value;
            hours = hours.Substring(3);
            hours = hours.Substring(0, hours.IndexOf("</b>"));

            return new { total, hours };


        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        #region base client helper
        public class Builder
        {
            public string Key;

            const string BaseUrl = "https://api.steampowered.com/";

            public Builder(string key)
            {
                Key = key;
            }

            public string CreateGamePriceUrl(int appid)
            {
                return $"{BaseUrl}{Api.Economy.GetAssetPrices}key={Key}&appid={appid}";
            }
            public string CreateSummaryUrl(string userid)
            {
                return $"{BaseUrl}{Api.SteamUser.GetPlayerSummaries}key={Key}&steamids={userid}";
            }

            public string CreateIDFinder(string username)
            {
                return $"{BaseUrl}{Api.SteamUser.ResolveVanityUrl}key={Key}&vanityurl={username}";
            }

            public string CreateSchemaUrl(string appid)
            {
                return $"{BaseUrl}{Api.SteamUserStats.GetSchemaForGame}key={Key}&appid={appid}";
            }

            public string CreateGameListUrl(string userid)
            {
                return $"{BaseUrl}{Api.PlayerService.GetOwnedGames}key={Key}&steamid={userid}";
            }

            public string CreateRecentGameListUrl(string userid)
            {
                return $"{BaseUrl}{Api.PlayerService.GetRecentlyPlayedGames}key={Key}&steamid={userid}";
            }

            public string CreateUserGameStatUrl(string userid, string appid)
            {
                return $"{BaseUrl}{Api.SteamUserStats.GetUserStatsForGame}key={Key}&steamid={userid}&appid={appid}";
            }
        }

        public static class Api
        {
            public static class SteamUser
            {
                private const string Base = "ISteamUser/";
                public static string GetPlayerSummaries = Base + "GetPlayerSummaries/v0001/?";
                public static string ResolveVanityUrl = Base + "ResolveVanityURL/v0001/?";
            }

            public static class Economy
            {
                private const string Base = "ISteamEconomy/";
                public static string GetAssetPrices = Base + "GetAssetPrices/v0001/?";
            }

            public static class PlayerService
            {
                private const string Base = "IPlayerService/";
                public static string GetOwnedGames = Base + "GetOwnedGames/v0001/?include_appinfo=1&include_played_free_games=1&";
                public static string GetRecentlyPlayedGames = Base + "GetRecentlyPlayedGames/v0001/?count=5&";
            }

            public static class SteamUserStats
            {
                private const string Base = "ISteamUserStats/";
                public static string GetSchemaForGame = Base + "GetSchemaForGame/v2/?";
                public static string GetUserStatsForGame = Base + "GetUserStatsForGame/v0002/?";
            }

        }

        public class CookieAwareWebClient : WebClient
        {
            public CookieContainer CookieContainer { get; set; } = new CookieContainer();

            protected override WebRequest GetWebRequest(Uri uri)
            {
                WebRequest request = base.GetWebRequest(uri);
                if (request is HttpWebRequest)
                {
                    (request as HttpWebRequest).CookieContainer = CookieContainer;
                }
                return request;
            }
        }
        #endregion
    }
}
