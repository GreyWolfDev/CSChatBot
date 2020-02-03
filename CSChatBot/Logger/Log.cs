using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Logger
{

    public class Log
    {
        public readonly string Path;
        private readonly Queue<LogQueueItem> _logQueue = new Queue<LogQueueItem>();

        public Log(string directory)
        {
            Path = directory;
            Directory.CreateDirectory(Path);
            new Task(WatchQueue).Start();
        }

        public void Write(object msg, LogLevel level = LogLevel.Info, ConsoleColor? overrideColor = null, string fileName = "log.log")
        {
            try
            {
                var finalMessage = $"{DateTime.Now} ";
                ConsoleColor color = ConsoleColor.Gray;
                if (overrideColor != null)
                    color = overrideColor.Value;
                else
                    switch (level)
                    {
                        case LogLevel.Info:
                            color = ConsoleColor.Gray;
                            finalMessage += "[INFO] - ";
                            break;
                        case LogLevel.Warn:
                            color = ConsoleColor.Yellow;
                            finalMessage += "[WARN] - ";
                            break;
                        case LogLevel.Error:
                            color = ConsoleColor.Red;
                            finalMessage += "[ERROR] - ";
                            break;
                        case LogLevel.Highlight:
                            color = ConsoleColor.White;
                            finalMessage += "[HIGHLIGHT] - ";
                            break;
                    }
                finalMessage += msg.ToString();
                _logQueue.Enqueue(new LogQueueItem(System.IO.Path.Combine(Path, fileName), finalMessage));

                Console.ForegroundColor = color;
                Console.Write(finalMessage);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void WriteLine(object msg, LogLevel level = LogLevel.Info, ConsoleColor? overrideColor = null, string fileName = "log.log")
        {
            Write(msg + Environment.NewLine, level, overrideColor, fileName);
        }

        private void WatchQueue()
        {
            while (true)
            {

                while (_logQueue.Count > 0)
                {
                    var item = _logQueue.Dequeue();
                    if (item == null) continue;
                    var succeed = false;
                    while (succeed == false)
                    {

                        try
                        {
                            using (var sw = new StreamWriter(item.File, true))
                            {
                                sw.Write(item.Message);
                            }
                            Thread.Sleep(100);
                            succeed = true;
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                Thread.Sleep(2000);
            }
        }
    }

    public class LogQueueItem
    {
        public string File { get; set; }
        public string Message { get; set; }

        public LogQueueItem(string file, string message)
        {
            File = file;
            Message = message;
        }
    }

    public enum LogLevel
    {
        Info, Warn, Error, Highlight
    }

}
