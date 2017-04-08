using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DB;
using DB.Models;
using DB.Extensions;
using ModuleFramework;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System.Collections;
using System.Reflection;

namespace Cleverbot
{
    /// <summary>
    /// A sample module for CSChatBot
    /// </summary>
    [ModuleFramework.Module(Author = "parabola949", Name = "Mitsuku", Version = "1.0")]
    public class Cleverbot
    {
        
        public Cleverbot(Instance db, Setting settings, TelegramBotClient bot)
        {
            var me = bot.GetMeAsync().Result;
            bot.OnUpdate += (sender, args) =>
            {
                new Task(() =>
                {
                    try
                    {
                        //check to see if it was to me, a reply to me, whatever
                        var message = args.Update.Message;
                        if (message?.Date == null) return;
                        if (message.Date < DateTime.UtcNow.AddSeconds(-10)) return;
                        if (message?.Text == null) return;
                        
                        var text = message.Text.ToLower();

                        if (text.StartsWith("!") || text.StartsWith("/")) return; //ignore commands
                        

                        if (text.Contains(me.FirstName.ToLower()) || text.Contains(me.Username.ToLower()) ||
                            message.ReplyToMessage?.From.Id == me.Id || message.Chat.Type == ChatType.Private)
                        {
                            DB.Models.User u = db.GetUserById(message.From.Id);
                            if (u.Grounded == true) return;
                            DB.Models.Group g = null;
                            if (message.Chat.Type != ChatType.Private)
                            {
                                //check if group has AI enabled
                                g = db.GetGroupById(message.Chat.Id);
                                var enabled = g.GetSetting<bool>("MitsukuEnabled", db, false);
                                if (!enabled) return;
                            }
                            if (message.Text.Contains("ZBERT"))
                            {
                                bot.SendTextMessageAsync(message.Chat.Id, $"User {message.From.FirstName} is now ignored.", replyToMessageId: message.MessageId);
                                u = db.GetUserById(message.From.Id);
                                u.Grounded = true;
                                u.GroundedBy = "Mitsuku";
                                u.Save(db);
                                return;
                            }
                            var cookieCode = "";
                            if (message.Chat.Type == ChatType.Private || text.Contains("my") || text.Contains("i am"))
                            {
                                //personal message
                                
                                cookieCode = u.GetSetting<string>("MitsukuCookie", db, "");
                            }
                            else
                            {
                                //group message
                                g = db.GetGroupById(message.Chat.Id);
                                cookieCode = g.GetSetting<string>("MitsukuCookie", db, "");
                            }
                            text = text.Replace(me.FirstName.ToLower(), "Mitsuku").Replace("@", "").Replace(me.Username.ToLower(), "Mitsuku");
                            //change this to use webrequest, so we can use cookies.

                            //var request = WebRequest.Create("https://kakko.pandorabots.com/pandora/talk?botid=f326d0be8e345a13&skin=chat");
                            var values = new NameValueCollection()
                                            {
                                                {"message", text}
                                            };
                            //var response = request.GetResponse()
                            var wc = new CookieAwareWebClient();
                            if (!String.IsNullOrEmpty(cookieCode))
                                wc.CookieContainer.Add(new Cookie
                                {
                                    Domain = "kakko.pandorabots.com",
                                    Name = "botcust2",
                                    Value = cookieCode
                                });
                            var response =
                                Encoding.UTF8.GetString(
                                    wc.UploadValues(
                                        "https://kakko.pandorabots.com/pandora/talk?botid=f326d0be8e345a13&skin=chat", values
                                        ));


                            //sample botcust2=94b4d935be0b9c19
                            var cookie = GetAllCookies(wc.CookieContainer).First();
                            if (cookie.Value != cookieCode)
                            {
                                if (u != null)
                                    u.SetSetting<string>("MitsukuCookie", db, "", cookie.Value);
                                if (g != null)
                                    g.SetSetting<string>("MitsukuCookie", db, "", cookie.Value);
                            }
                            //wc.CookieContainer
                            var matches = Regex.Matches(response, "(Mіtsuku:</B>(.*))(<B>You:</B>)", RegexOptions.CultureInvariant);
                            var match = matches[0].Value;
                            if (match.Contains("<B>You:</B>"))
                                match = match.Substring(0, match.IndexOf("<B>You:</B>"));
                            match =
                                match.Replace("Mіtsuku:", "")
                                    .Replace("Mitsuku:", "")
                                    .Replace("</B> ", "")
                                    .Replace(" .", ".")
                                    .Replace("<br>", "  ")
                                    .Replace("Mitsuku", "Seras")
                                    .Replace("Мitsuku", "Seras")
                                    .Replace("Mіtsuku", "Seras")
                                    .Replace(" < P ALIGN=\"CENTER\">", "")
                                    .Replace("</P>", " ")
                                    .Trim();
                            match = match.Replace("<B>", "```").Replace("You:", "").Replace("Μitsuku:", "").Trim();
                            match = match.Replace("Mousebreaker", "Para");

                            //[URL]http://www.google.co.uk/search?hl=en&amp;q=dot net&amp;btnI=I%27m+Feeling+Lucky&amp;meta=[/URL]
                            if (match.Contains("[URL]"))
                            {
                                //parse out the url
                                var url = Regex.Match(match, @"(\[URL\].*\[/URL\])").Value;
                                match = "[" + match.Replace(url, "") + "]";
                                url = url.Replace("[URL]", "").Replace("[/URL]", "").Replace(".co.uk", ".com");
                                match += $"({url})"; //markdown linking
                                Console.WriteLine(match);

                                bot.SendTextMessageAsync(args.Update.Message.Chat.Id, match, replyToMessageId: message.MessageId, parseMode: ParseMode.Markdown);
                            }
                            //<P ALIGN="CENTER"><img src="http://www.square-bear.co.uk/mitsuku/gallery/donaldtrump.jpg"></img></P>
                            else if (match.Contains("img src=\""))
                            {
                                var img = Regex.Match(match, "<img src=\"(.*)\"></img>").Value;
                                match = match.Replace(img, "").Replace("<P ALIGN=\"CENTER\">", "").Trim();

                                ;
                                img = img.Replace("<img src=\"", "").Replace("\"></img>", "");
                                //download the photo
                                var filename = args.Update.Message.MessageId + ".jpg";
                                new WebClient().DownloadFile(img, filename);
                                //create the file to send
                                var f2S = new FileToSend(filename, new FileStream(filename, FileMode.Open, FileAccess.Read));
                                Console.WriteLine(match);
                                bot.SendPhotoAsync(args.Update.Message.Chat.Id, f2S, match);
                                //bot.SendTextMessageAsync(args.Update.Message.Chat.Id, match);
                            }
                            else
                            {
                                Console.WriteLine(match);
                                bot.SendTextMessageAsync(args.Update.Message.Chat.Id, match, replyToMessageId: message.MessageId, parseMode: ParseMode.Markdown);
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }).Start();
            };

        }

        [ChatCommand(Triggers = new[] { "toggleai", "stfu" }, GroupAdminOnly = true, HideFromInline = true, InGroupOnly = true, HelpText = "Toggle AI speech for group")]
        public static CommandResponse Toggle(CommandEventArgs args)
        {
            
            var g = args.DatabaseInstance.GetGroupById(args.Message.Chat.Id);
            var enabled = g.GetSetting<bool>("MitsukuEnabled", args.DatabaseInstance, false);
            enabled = !enabled;
            g.SetSetting<bool>("MitsukuEnabled", args.DatabaseInstance, false, enabled);
            return new CommandResponse("AI Enabled for group: " + enabled);
        }

        private static IEnumerable<Cookie> GetAllCookies(CookieContainer c)
        {
            Hashtable k = (Hashtable)c.GetType().GetField("m_domainTable", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(c);
            foreach (DictionaryEntry element in k)
            {
                SortedList l = (SortedList)element.Value.GetType().GetField("m_list", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(element.Value);
                foreach (var e in l)
                {
                    var cl = (CookieCollection)((DictionaryEntry)e).Value;
                    foreach (Cookie fc in cl)
                    {
                        yield return fc;
                    }
                }
            }
        }
    }

    public class CookieAwareWebClient : WebClient
    {
        public CookieAwareWebClient()
        {
            CookieContainer = new CookieContainer();
        }
        public CookieContainer CookieContainer { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            return request;
        }
    }

    public class CleverResponse
    {
        public string clever { get; set; }
    }

}
