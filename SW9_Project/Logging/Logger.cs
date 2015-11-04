using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SW9_Project.Logging
{
    class Logger
    {  

        private static string logFilePath = "log-default.txt";

        public Logger(): this(logFilePath, false) { }
        public Logger(bool flush): this(logFilePath, flush) { }
        public Logger(string filePath): this(filePath, false) { }
        public Logger(string filePath, bool flush)
        {
            logFilePath = filePath;
            if(flush)
                Logger.flush();
        }

        public static string filePath
        {
            get { return Logger.logFilePath; }
            set { if (value.Length > 0) Logger.logFilePath = value; }
        }

        public void LogKinectGesture(string gestureName, Point pointer, DateTime time)
        {
            string message = "KINECT GESTURE" +
                             "\t name: " + gestureName +
                             "\t x,y: " + pointer +
                             "\t time: " + time;

            log(message);

            Console.WriteLine(message);
        }

        public void LogMobileGesture(string gestureName, DateTime time)
        {
            string message = "MOBILE GESTURE" +
                             "\t name: " + gestureName +
                             "\t time: " + time;

            log(message);

            Console.WriteLine(message);
        }

        public static void log(string msg)
        {
            if (msg.Length > 0)
            {
                using (StreamWriter sw = File.AppendText(Logger.filePath))
                {
                    sw.WriteLine("Log time [{0} {1}]: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), msg);
                    sw.Flush();
                }
            }
        }

        public static void flush()
        {
            File.WriteAllText(Logger.filePath, string.Empty);
        }

    }
}
