using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace SW9_Project.Logging
{
    class Logger
    {
        private static string directory = "log/";
        private static string testLogFilePath = directory + "-test.txt";
        private static string commentLogFilePath = directory + "-comment.txt";
        StreamWriter sw = File.AppendText(Logger.testFilePath);

        int userID;

        public static Logger CurrentLogger;

        public Logger()
        {
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
        }

        public static void Initialize() {
            if(CurrentLogger == null) {
                CurrentLogger = new Logger();
            }
        }
        

        public static string testFilePath
        {
            get { return Logger.testLogFilePath; }
            set { if (value.Length > 0) Logger.testLogFilePath = value; }
        }

        public static string commentFilePath
        {
            get { return Logger.commentLogFilePath; }
            set { if (value.Length > 0) Logger.commentLogFilePath = value; }
        }

        private bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        /// <summary>
        /// Generate userId. Files exist in log directory, get highest id and add 1 to the current userID
        /// </summary>
        /// <returns>userId</returns>
        public int NewUser()
        {
            string pattern = @"^\d+";
            List<int> ids = new List<int>();
            Regex r = new Regex(pattern, RegexOptions.IgnoreCase);
            if(!IsDirectoryEmpty(directory))
            {
                foreach (string s in Directory.GetFiles(directory, "*.txt").Select(System.IO.Path.GetFileName))
                {
                    if (r.IsMatch(s))
                    {
                        Match m = r.Match(s);
                        ids.Add(Int32.Parse(m.Value));
                    }
                }

                userID = ids.Max() + 1;
            }
            else
            {
                userID = 1;
            }

            testFilePath = directory + userID + "-test.txt";
            commentFilePath = directory + userID + "-comment.txt";

            Log("New user registered: " + userID);

            return userID;
        }

        /// <summary>
        /// The user is finished and we are done writing to the file(s)
        /// </summary>
        public void EndUser()
        {
            string message = "Test session ended.";
            Log(message);
        }

        /// <summary>
        /// Log the creation of a new size test
        /// </summary>
        /// <param name="gridHeight"></param>
        /// <param name="gridWidth"></param>
        /// <param name="cellHeight"></param>
        /// <param name="cellWidth"></param>
        public void StartNewSizeTest(int gridHeight, int gridWidth, double cellHeight, double cellWidth)
        {
            string message = "Started new size test." + 
                             " Grid height: " + gridHeight +
                             " Grid width: " + gridWidth;
            Log(message);
        }

        /// <summary>
        /// Log the end of a size test
        /// </summary>
        public void EndCurrentSizeTest()
        {
            string message = "Size test ended.";
            Log(message);
        }

        /// <summary>
        /// Log that new gesture test has started
        /// </summary>
        /// <param name="gestureType"></param>
        /// <param name="gestureDirection"></param>
        public void StartNewgestureTest(GestureType gestureType, GestureDirection gestureDirection)
        {
            string message = "Started new gesture test." + 
                             " Type: " + gestureType.ToString() + 
                             " Direction: " + gestureDirection.ToString();
            Log(message);
        }

        /// <summary>
        /// Log that the current gesture test has ended.
        /// </summary>
        public void EndCurrentGestureTest()
        {
            string message = "Gesture test enden.";
            Log(message);
        }

        /// <summary>
        /// Log the creation of a new target on the canvas
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        public void AddNewTarget(String shape, int gridX, int gridY)
        {
            string message = "New " + shape + " target added. " +
                             " X = " + gridX + " Y = " + gridY;
            Log(message);
        }

        /// <summary>
        /// Log that target has been hit
        /// </summary>
        public void CurrentTargetHit()
        {
            string message = "Target hit!";
            Log(message);
        }

        /// <summary>
        /// Log a kinect gesture
        /// </summary>
        /// <param name="gesture"></param>
        public void AddNewKinectGesture(KinectGesture gesture, Cell cell)
        {
            string message = "KINECT GESTURE" + 
                             " Type: " + gesture.Type.ToString() +
                             " Direction: " + gesture.Direction.ToString() +
                             " Shape: " + gesture.Shape + 
                             " Pointer: X = " + gesture.Pointer.X + " Y = " + gesture.Pointer.Y +
                             " Cell: X = " + cell.X + " Y = " + cell.Y;

            Log(message);
        }

        /// <summary>
        /// Log a mobile gesture
        /// </summary>
        /// <param name="gesture"></param>
        public void AddNewMobileGesture(MobileGesture gesture)
        {
            string message = "MOBILE GESTURE" +
                             " Type: " + gesture.Type.ToString() +
                             " Direction: " + gesture.Direction.ToString() +
                             " Shape: " + gesture.Shape;

            Log(message);

            Console.WriteLine(message);
        }

        /// <summary>
        /// Write message to the log file
        /// </summary>
        /// <param name="msg"></param>
        private void Log(string msg)
        {
            try {
                if (msg.Length > 0)
                {
                    sw.WriteLine("[{0} {1}]: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), msg);
                    sw.Flush();
                }
            } catch(IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Write comment to log file
        /// </summary>
        /// <param name="comment"></param>
        public static void LogComment(string comment)
        {
            if (comment.Length > 0)
            {
                using (StreamWriter sw = File.AppendText(Logger.commentFilePath))
                {
                    sw.WriteLine("[{0} {1}]: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), comment);
                    sw.Flush();
                }
            }
        }

        //private static void flush()
        //{
        //    File.WriteAllText(Logger.testFilePath, string.Empty);
        //}

    }
}
