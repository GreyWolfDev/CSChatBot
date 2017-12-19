using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashOfClans
{
    public static class UrlConstants
    {
        public const string GetClanInformationUrlTemplate = @"https://api.clashofclans.com/v1/clans/{0}";
        public const string ListClanMembersUrlTemplate = @"https://api.clashofclans.com/v1/clans/{0}/members";
        public const string SearchClansUrlTemplate = @"https://api.clashofclans.com/v1/clans{0}";
        public const string GetCurrentWarInformationTemplate = @"https://api.clashofclans.com/v1/clans/{0}/currentwar";
        public const string GetWarLogInformationTemplate = @"https://api.clashofclans.com/v1/clans/{0}/warlog";
        public const string ListLocationsUrlTemplate = @"https://api.clashofclans.com/v1/locations";
        public const string GetLocationInformationUrlTemplate = @"https://api.clashofclans.com/v1/locations/{0}";
        public const string GetClanRankForLocationUrlTemplate = @"https://api.clashofclans.com/v1/locations/{0}/rankings/clans";
        public const string GetPlayerRankForLocationUrlTemplate = @"https://api.clashofclans.com/v1/locations/{0}/rankings/players";
        public const string ListLeagueUrlTemplate = @"https://api.clashofclans.com/v1/leagues";
    }
}
