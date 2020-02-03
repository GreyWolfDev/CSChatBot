using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DB;
using DB.Models;
using ModuleFramework;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace DeepDream
{
    /// <summary>
    /// A sample module for CSChatBot
    /// </summary>
    //[ModuleFramework.Module(Author = "parabola949", Name = "DeepDream", Version = "1.0")]
    public class DeepDream
    {
//        public DeepDream(Instance db, Setting settings, TelegramBotClient bot)
//        {
//            //do your initialization here.  this is done before the bot begins receiving updates.
//            //DO NOT run  bot.StartReceiving();
//            //This is run in the main bot already.

//            //You can subscribe to updates.  This is a simple echo
//            bot.OnUpdate += (sender, args) =>
//            {
//                var p = args.Update?.Message?.Photo;
//                if (p == null) return;
//                if (args.Update.Message.Chat.Type != ChatType.Private) return;

//                //ok, so we got an image in private.  Neat!  Let's deep dream that shit.
//                var uploadUrl = "http://psychic-vr-lab.com/deepdream/upload.php";
//                /*
//                 Cookie: CONCRETE5=ldnan1dveje0n93gmcd0o5si87
//                 User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36
//                 Origin: http://psychic-vr-lab.com
//                 Referer: http://psychic-vr-lab.com/deepdream/
//                 Host: psychic-vr-lab.com
//                 Content-Type: multipart/form-data; boundary=----WebKitFormBoundarylX2LGNTA85Dmk9I9
//                 Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*//*; q = 0.8
//                 Content-Length:354585

//                Request Payload:
//                ------WebKitFormBoundarylX2LGNTA85Dmk9I9
//Content-Disposition: form-data; name="imagefile"; filename="Snarling-Wolf.jpg"
//Content-Type: image/jpeg


//------WebKitFormBoundarylX2LGNTA85Dmk9I9
//Content-Disposition: form-data; name="MAX_FILE_SIZE"

//5000000
//------WebKitFormBoundarylX2LGNTA85Dmk9I9--
        
//*/
//                //get our image bytes
//                var s = new MemoryStream();
//                var big = p.OrderByDescending(x => x.Height).First();
//                var file = bot.GetFileAsync(big.FileId).Result;
//                var fileName = file.FilePath.Replace("photos/", "");
//                using (var client = new HttpClient())
//                {
//                    using (var content =
//                        new MultipartFormDataContent("boundary=----WebKitFormBoundarylX2LGNTA85Dmk9I9"))
//                    {
//                        content.Add(new StreamContent(s), "imagefile");
//                        content.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
//                        content.Headers.Add("Content-Disposition", "form-data; name=imagefile; filename=" + HttpUtility.UrlEncode(fileName));
//                        using (var up = client.PostAsync(uploadUrl, content).Result)
//                        {
//                            var url = up.Content.ReadAsStringAsync();
//                            bot.SendTextMessageAsync(args.Update.Message.Chat.Id, url.Result);
//                        }
//                    }
//                }



//            };

//            //Or other things, like callback queries (inline buttons)
//            //bot.OnCallbackQuery += (sender, args) =>
//            //{
//            //    bot.SendTextMessageAsync(args.CallbackQuery.From.Id, args.CallbackQuery.Data);
//            //};
//        }

        [ChatCommand(Triggers = new[] { "test" }, DontSearchInline = true, HelpText = "This is a sample command")]
        public static CommandResponse Test(CommandEventArgs args)
        {
            //do something
            var length = args.Message.Text.Length;
            //return message
            return new CommandResponse($"Message: {args.Message.Text}\nLength: {length}");
        }
    }
}
