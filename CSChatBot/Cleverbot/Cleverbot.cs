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
using ModuleFramework;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Cleverbot
{
    /// <summary>
    /// A sample module for CSChatBot
    /// </summary>
    [Module(Author = "parabola949", Name = "Mitsuku", Version = "1.0")]
    public class Cleverbot
    {
        public static bool Enabled = true;
        public Cleverbot(Instance db, Setting settings, TelegramBotClient bot)
        {
            var me = bot.GetMeAsync().Result;
            bot.OnUpdate += (sender, args) =>
            {
                new Task(() =>
                {
                    if (!Enabled) return;
                    try
                    {
                        //check to see if it was to me, a reply to me, whatever
                        var message = args.Update.Message;
                        if (message.Date < DateTime.UtcNow.AddSeconds(-10)) return;
                        if (message?.Text == null) return;
                        var text = message.Text.ToLower();

                        if (text.StartsWith("!") || text.StartsWith("/")) return; //ignore commands
                        if (message.Chat.Type != ChatType.Private && message.Chat.Id != -1001119386963) return;

                        if (text.Contains(me.FirstName.ToLower()) || text.Contains(me.Username.ToLower()) ||
                            message.ReplyToMessage?.From.Id == me.Id || message.Chat.Type == ChatType.Private)
                        {
                            text = text.Replace(me.FirstName.ToLower(), "Mitsuku").Replace("@", "").Replace(me.Username.ToLower(), "Mitsuku");
                            var response =
                                Encoding.UTF8.GetString(
                                    new WebClient().UploadValues(
                                        "https://kakko.pandorabots.com/pandora/talk?botid=f326d0be8e345a13&skin=chat",
                                        new NameValueCollection()
                                        {
                                        {"message", text}
                                        }));
                            var matches = Regex.Matches(response, "(Мitsuku:</B>(.*))");
                            var match = matches[0].Value;

                            match =
                                match.Replace("Мitsuku:", "")
                                    .Replace("</B> ", "")
                                    .Replace(" .", ".")
                                    .Replace("<br>", "  ")
                                    .Replace("Mitsuku", "Seras")
                                    .Replace("Мitsuku", "Seras")
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

        [ChatCommand(Triggers = new[] { "toggleai", "stfu" }, DevOnly = true, DontSearchInline = true, HideFromInline = true, HelpText = "Toggle AI speech")]
        public static CommandResponse Toggle(CommandEventArgs args)
        {
            Enabled = !Enabled;
            return new CommandResponse("AI Enabled: " + Enabled);
        }
    }


    public class CleverResponse
    {
        public string clever { get; set; }
    }

}
