using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SW9_Project.Logging
{
    class Logger
    {  

        public void LogGesture(string gestureName, Point pointer, DateTime time)
        {
            string message = "GESTURE" +
                             "\t name: " + gestureName +
                             "\t x,y: " + pointer +
                             "\t time: " + time;

            Console.WriteLine(message);
        }
    }
}
