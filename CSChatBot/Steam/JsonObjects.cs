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

    #region Game Prices

    public class GamePrices
    {
        public Result result { get; set; }
    }

    public class Result
    {
        public bool success { get; set; }
        public Asset[] assets { get; set; }
    }

    public class Asset
    {
        public Prices prices { get; set; }
        public string name { get; set; }
        public string date { get; set; }
        public Class1[] _class { get; set; }
        public string classid { get; set; }
        public Original_Prices original_prices { get; set; }
    }

    public class Prices
    {
        public int USD { get; set; }
        public int GBP { get; set; }
        public int EUR { get; set; }
        public int RUB { get; set; }
        public int BRL { get; set; }
        public int Unknown { get; set; }
        public int JPY { get; set; }
        public int NOK { get; set; }
        public int IDR { get; set; }
        public int MYR { get; set; }
        public int PHP { get; set; }
        public int SGD { get; set; }
        public int THB { get; set; }
        public int VND { get; set; }
        public int KRW { get; set; }
        public int TRY { get; set; }
        public int UAH { get; set; }
        public int MXN { get; set; }
        public int CAD { get; set; }
        public int AUD { get; set; }
        public int NZD { get; set; }
        public int PLN { get; set; }
        public int CHF { get; set; }
        public int AED { get; set; }
        public int CLP { get; set; }
        public int CNY { get; set; }
        public int COP { get; set; }
        public int PEN { get; set; }
        public int SAR { get; set; }
        public int TWD { get; set; }
        public int HKD { get; set; }
        public int ZAR { get; set; }
        public int INR { get; set; }
        public int ARS { get; set; }
        public int CRC { get; set; }
        public int ILS { get; set; }
        public int KWD { get; set; }
        public int QAR { get; set; }
        public int UYU { get; set; }
        public int KZT { get; set; }
        public int BYN { get; set; }
    }

    public class Original_Prices
    {
        public int USD { get; set; }
        public int GBP { get; set; }
        public int EUR { get; set; }
        public int RUB { get; set; }
        public int BRL { get; set; }
        public int JPY { get; set; }
        public int NOK { get; set; }
        public int IDR { get; set; }
        public int MYR { get; set; }
        public int PHP { get; set; }
        public int SGD { get; set; }
        public int THB { get; set; }
        public int VND { get; set; }
        public int KRW { get; set; }
        public int TRY { get; set; }
        public int UAH { get; set; }
        public int MXN { get; set; }
        public int CAD { get; set; }
        public int AUD { get; set; }
        public int NZD { get; set; }
        public int PLN { get; set; }
        public int CHF { get; set; }
        public int AED { get; set; }
        public int CLP { get; set; }
        public int CNY { get; set; }
        public int COP { get; set; }
        public int PEN { get; set; }
        public int SAR { get; set; }
        public int TWD { get; set; }
        public int HKD { get; set; }
        public int ZAR { get; set; }
        public int INR { get; set; }
        public int ARS { get; set; }
        public int CRC { get; set; }
        public int ILS { get; set; }
        public int KWD { get; set; }
        public int QAR { get; set; }
        public int UYU { get; set; }
        public int KZT { get; set; }
    }

    public class Class1
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    #endregion
}
