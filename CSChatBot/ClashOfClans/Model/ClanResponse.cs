using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashOfClans.Model
{

    public class ClanResponse
    {
        public string tag { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public Location location { get; set; }
        public Badgeurls badgeUrls { get; set; }
        public int clanLevel { get; set; }
        public int clanPoints { get; set; }
        public int clanVersusPoints { get; set; }
        public int requiredTrophies { get; set; }
        public string warFrequency { get; set; }
        public int warWinStreak { get; set; }
        public int warWins { get; set; }
        public int warTies { get; set; }
        public int warLosses { get; set; }
        public bool isWarLogPublic { get; set; }
        public int members { get; set; }
        public Memberlist[] memberList { get; set; }
        public string reason { get; set; }
    }

    public class Location
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isCountry { get; set; }
    }

    public class Memberlist
    {
        public string tag { get; set; }
        public string name { get; set; }
        public string role { get; set; }
        public int expLevel { get; set; }
        public League league { get; set; }
        public int trophies { get; set; }
        public int versusTrophies { get; set; }
        public int clanRank { get; set; }
        public int previousClanRank { get; set; }
        public int donations { get; set; }
        public int donationsReceived { get; set; }
    }

    public class League
    {
        public int id { get; set; }
        public string name { get; set; }
        public Iconurls iconUrls { get; set; }
    }

    public class Iconurls
    {
        public string small { get; set; }
        public string tiny { get; set; }
        public string medium { get; set; }
    }

}
