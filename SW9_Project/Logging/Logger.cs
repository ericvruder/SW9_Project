﻿using System;
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

        int userID;
        private static string testLogFilePath = "log/test.txt";

        public Logger(): this(testLogFilePath, false) { }
        public Logger(bool flush): this(testLogFilePath, flush) { }
        public Logger(string filePath): this(filePath, false) { }
        public Logger(string filePath, bool flush)
        {
            testLogFilePath = filePath;
            if(flush)
                Logger.flush();
        }

        public static string filePath
        {
            get { return Logger.testLogFilePath; }
            set { if (value.Length > 0) Logger.testLogFilePath = value; }
        }

        public void LogMessage(string msg)
        {
            log(msg);

            Console.WriteLine(msg);
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
