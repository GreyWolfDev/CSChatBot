using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSChatBot.Helpers;
using DB;
using DB.Extensions;
using DB.Models;
using ModuleFramework;

namespace CSChatBot.Modules
{
    [Module(Author = "parabola949", Name = "User Functions", Version = "1.0")]
    class UserFunctions
    {
        public UserFunctions(Instance instance, Setting setting)
        {

        }

        #region Chat Commands
        [ChatCommand(Triggers = new[] { "setloc" }, HideFromInline = true)]
        public static CommandResponse SetLocation(CommandEventArgs args)
        {
            args.SourceUser.Location = args.Parameters;
            args.SourceUser.Save(args.DatabaseInstance);
            return new CommandResponse("Location set.");
        }

        [ChatCommand(Triggers = new[] { "points" }, HelpText = "Gets your points")]
        public static CommandResponse GetPoints(CommandEventArgs args)
        {
            var target = UserHelper.GetTarget(args);
            return new CommandResponse(target == null
                ? $"{args.Parameters} not found!"
                : $"{target.Name} has {target.Points} points!");
        }

        [ChatCommand(Triggers = new[] { "first" }, HelpText = "Shows when you were first seen by the bot")]
        public static CommandResponse GetFirstSeen(CommandEventArgs args)
        {
            var target = UserHelper.GetTarget(args);
            return new CommandResponse(target == null
                ? $"{args.Parameters} not found!"
                : $"{target.Name} was first seen (by me): {target.FirstSeen}");
        }

        [ChatCommand(Triggers = new[] { "last" }, HelpText = "Gets when a user was last seen, and where")]
        public static CommandResponse GetLastSeen(CommandEventArgs args)
        {
            var target = UserHelper.GetTarget(args);
            string length = null;
            if (target != null)
                length = LengthAgo(target.LastHeard);
            return new CommandResponse(target == null
                ? $"{args.Parameters} not found!"
                : $"{target.Name} was last seen (by me) {target.LastState} {(length == null ? "just now" : length + " ago")} ({target.LastHeard})");
        }

        [ChatCommand(Triggers = new[] { "top" }, HelpText = "Shows a list of users with the highest points")]
        public static CommandResponse GetTopUsers(CommandEventArgs args)
        {
            var howMany = 5;
            if (!String.IsNullOrWhiteSpace(args.Parameters))
                Int32.TryParse(args.Parameters.Trim(), out howMany);
            return
                new CommandResponse(
                    args.DatabaseInstance.Users.OrderByDescending(x => x.Points).Take(howMany).Aggregate("Top " + howMany + " users:",
                        (current, user) => current + "\n" + (user.Name + " with " + user.Points + " points")));
        }

        [ChatCommand(Triggers = new[] { "fine" }, HideFromInline = true)]
        public static CommandResponse FineUser(CommandEventArgs args)
        {
            var target = UserHelper.GetTarget(args);
            if (target == null) return new CommandResponse(args.Parameters + " not found!");
            target.Debt += 15;
            target.Save(args.DatabaseInstance);
            return new CommandResponse($"{target.Name} has been fined, and now has a debt of {target.Debt} foxdollars.");
        }

        [ChatCommand(Triggers = new[] { "debt" }, HelpText = "Gets your current debt")]
        public static CommandResponse GetDebt(CommandEventArgs args)
        {
            var target = UserHelper.GetTarget(args);
            return new CommandResponse(target == null ? $"{args.Parameters} not found!" : $"{target.Name} has a debt of {target.Debt} foxdollars.");
        }
        #endregion
        #region Helper Methods


        private static string LengthAgo(DateTime when)
        {
            DateTime dtNow = DateTime.Now;
            TimeSpan ts = dtNow.Subtract(when);
            if (ts < TimeSpan.FromSeconds(30))
                return null;
            int years = ts.Days / 365; //no leap year accounting
            int months = (ts.Days % 365) / 30; //naive guess at month size
            int weeks = ((ts.Days % 365) % 30) / 7;
            int days = (((ts.Days % 365) % 30) % 7);

            StringBuilder sb = new StringBuilder();
            if (years > 0)
            {
                sb.Append(years + " years, ");
            }
            if (months > 0)
            {
                sb.Append(months + " months, ");
            }
            if (weeks > 0)
            {
                sb.Append(weeks + " weeks, ");
            }
            if (days > 0)
            {
                sb.Append(days + " days, ");
            }
            if (ts.Hours > 0)
            {
                sb.Append(ts.Hours + " hours, ");
            }
            if (ts.Minutes > 0)
            {
                sb.Append(ts.Minutes + " minutes");
            }
            var result = sb.ToString();
            if (result.EndsWith(" "))
                result = result.Substring(0, result.Length - 2);

            return result;
        }
        #endregion
    }
}
