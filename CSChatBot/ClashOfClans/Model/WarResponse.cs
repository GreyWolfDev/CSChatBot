using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashOfClans.Model
{

    public class WarResponse
    {
        public string state { get; set; }
        public int teamSize { get; set; }
        public string preparationStartTime { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public Clan clan { get; set; }
        public Opponent opponent { get; set; }
    }

    public class Clan
    {
        public string tag { get; set; }
        public string name { get; set; }
        public Badgeurls badgeUrls { get; set; }
        public int clanLevel { get; set; }
        public int attacks { get; set; }
        public int stars { get; set; }
        public float destructionPercentage { get; set; }
        public Member[] members { get; set; }
    }

    public class Opponent
    {
        public string tag { get; set; }
        public string name { get; set; }
        public Badgeurls badgeUrls { get; set; }
        public int clanLevel { get; set; }
        public int attacks { get; set; }
        public int stars { get; set; }
        public float destructionPercentage { get; set; }
        public Member[] members { get; set; }
    }

    public class Badgeurls
    {
        public string small { get; set; }
        public string large { get; set; }
        public string medium { get; set; }
    }

    public class Member
    {
        public string tag { get; set; }
        public string name { get; set; }
        public int townhallLevel { get; set; }
        public int mapPosition { get; set; }
        public int opponentAttacks { get; set; }
        public Bestopponentattack bestOpponentAttack { get; set; }
        public Attack[] attacks { get; set; }
    }

    public class Bestopponentattack
    {
        public string attackerTag { get; set; }
        public string defenderTag { get; set; }
        public int stars { get; set; }
        public float destructionPercentage { get; set; }
        public int order { get; set; }
    }

    public class Attack
    {
        public string attackerTag { get; set; }
        public string defenderTag { get; set; }
        public int stars { get; set; }
        public float destructionPercentage { get; set; }
        public int order { get; set; }
    }
}
