using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
        StreamWriter sw; //FIX

        int userID;

        public static Logger CurrentLogger;

        public Logger()
        {
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            sw = File.AppendText(Logger.testFilePath);
        }
        private static int sgHeight, sgWidth, lgHeight, lgWidth;
        private static double canvasHeight, canvasWidth;

        public static void Intialize(int sHeight, int sWidth, int lHeight, int lWidth, double cnvasHeight, double cnvasWidth) {
            if (CurrentLogger == null) {
                CurrentLogger = new Logger();
            }
            sgHeight = sHeight;
            sgWidth = sWidth;
            lgHeight = lHeight;
            lgWidth = lWidth;
            canvasHeight = cnvasHeight;
            canvasWidth = cnvasWidth;
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
                int x = ids.Count() == 0 ? 0 : ids.Max();
                userID = x + 1;
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
        private void ChangeSize(int gridHeight, int gridWidth, double cellHeight, double cellWidth)
        {
            string message = "Changed grid size." +
                             " Grid height: " + gridHeight +
                             " Grid width: " + gridWidth +
                             " Cell height: " + cellHeight +
                             " Cell width: " + cellWidth;
            Log(message);
        }

        public void ChangeSize(GridSize size) {
            if(size == GridSize.Large) {
                ChangeSize(lgHeight, lgWidth, canvasHeight / lgHeight, canvasWidth / lgWidth);
            } else {
                ChangeSize(sgHeight, sgWidth, canvasHeight / sgHeight, canvasWidth / sgWidth);
            }
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
            string message = "Gesture test ended.";
            Log(message);
        }

        /// <summary>
        /// Log that target has been hit
        /// </summary>
        public void CurrentTargetHit(bool hit, Cell target, Point p, bool correctShape)
        {
            string result = "";
            if(hit) {
                result = "Target hit!";
            }
            if (!hit) {
                result = correctShape ? "Target missed!" : "Incorrect shape chosen!";
            }
            string message = result + " Target postion: (" + target.X + "," + target.Y + "). Pointer position: (" + p.ToString() + ").";
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

            LogComment(message);
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

            LogComment(message);

            Console.WriteLine(message);
        }

        /// <summary>
        /// Write message to the log file
        /// </summary>
        /// <param name="msg"></param>
        private void Log(string msg)
        {
            const int MAX_RETRY = 10;
            const int DELAY_MS = 1000;
            bool result = false;
            int retry = 0;
            bool keepRetry = true;

            while (keepRetry && !result && retry < MAX_RETRY)
            {
                try
                {
                    if (msg.Length > 0)
                    {
                        sw.WriteLine("[{0} {1}]: {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString(), msg);
                        sw.Flush();
                        result = true;
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.ToString());
                    Thread.Sleep(DELAY_MS);
                    retry++;
                }
                catch (Exception e)
                {
                    keepRetry = false;
                }
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
