using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Steam
{
    #region Player Summary

    public class PlayerSummaries
    {
        public SummaryResponse response { get; set; }
    }

    public class SummaryResponse
    {
        public Players players { get; set; }
    }

    public class Players
    {
        public Player[] player { get; set; }
    }

    public class Player
    {
        public string steamid { get; set; }
        public int communityvisibilitystate { get; set; }
        public int profilestate { get; set; }
        public string personaname { get; set; }
        public int lastlogoff { get; set; }
        public string profileurl { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public int personastate { get; set; }
        public string realname { get; set; }
        public string primaryclanid { get; set; }
        public int timecreated { get; set; }
        public int personastateflags { get; set; }
        public string gameextrainfo { get; set; }
        public string gameid { get; set; }
        public string loccountrycode { get; set; }
        public string locstatecode { get; set; }
        public int loccityid { get; set; }
    }

    #endregion

    #region Game List

    public class GameList
    {
        public GameResponse response { get; set; }
    }

    public class GameResponse
    {
        public int game_count { get; set; }
        public Game[] games { get; set; }
    }

    public class Game
    {
        public int appid { get; set; }
        public string name { get; set; }
        public string img_icon_url { get; set; }
        public string img_logo_url { get; set; }
        public bool has_community_visible_stats { get; set; }
        public int playtime_forever { get; set; }
        public int playtime_2weeks { get; set; }
    }

    #endregion

    #region Resolve Vanity Url
    public class ResolveVanityUrlResponse
    {
        public Response response { get; set; }
    }

    public class Response
    {
        public string steamid { get; set; }
        public int success { get; set; }
    }
    #endregion

    #region Game Schema

    public class SchemaResponse
    {
        public GameSchema game { get; set; }
    }

    public class GameSchema
    {
        public string gameName { get; set; }
        public string gameVersion { get; set; }
        public Availablegamestats availableGameStats { get; set; }
    }

    public class Availablegamestats
    {
        public Stat[] stats { get; set; }
        public Achievement[] achievements { get; set; }
    }

    public class Stat
    {
        public string name { get; set; }
        public int defaultvalue { get; set; }
        public string displayName { get; set; }
    }

    public class Achievement
    {
        public string name { get; set; }
        public int defaultvalue { get; set; }
        public string displayName { get; set; }
        public int hidden { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string icongray { get; set; }
    }

    #endregion

    #region User stats for game

    public class UserGameStatResponse
    {
        public Playerstats playerstats { get; set; }
    }

    public class Playerstats
    {
        public string steamID { get; set; }
        public string gameName { get; set; }
        public GameStat[] stats { get; set; }
        public UserAchievement[] achievements { get; set; }
    }

    public class GameStat
    {
        public string name { get; set; }
        public int value { get; set; }
    }

    public class UserAchievement
    {
        public string name { get; set; }
        public int achieved { get; set; }
    }

    #endregion
}
