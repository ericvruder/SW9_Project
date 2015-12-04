using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using SW9_Project;
using System.IO;

namespace DataParser {
    class Test {

        private static string directory = "..\\..\\..\\InfoPages/";

        public string ID { get; set; }

        public Test(List<Test> tests) {
        }
        public Dictionary<GestureType, List<Attempt>> Attempts { get; set; }
        public Dictionary<GestureType, TimeSpan> TestStart { get; set; }
        public Dictionary<GestureType, TimeSpan> PracticeTime { get; set; }

        private Test() {
            Attempts = new Dictionary<GestureType, List<Attempt>>();
            TestStart = new Dictionary<GestureType, TimeSpan>();
            PracticeTime = new Dictionary<GestureType, TimeSpan>();
        }
        public Test(string path) : this() {

            ID = path.Split('/').Last().Split('.').First();
            
            using (StreamReader sr = new StreamReader(path)) {
                string line = "";
                GridSize size = GridSize.Large;
                GestureType type = GestureType.Pinch;
                while ((line = sr.ReadLine()) != null) {
                    if (line.Contains("Started new gesture test.")) {

                        string tobesearched = "Type: ";
                        string toBefound = line.Substring(line.IndexOf(tobesearched) + tobesearched.Length).Split(' ')[0];
                        switch (toBefound) {
                            case "Throw": type = GestureType.Throw; break;
                            case "Tilt": type = GestureType.Tilt; break;
                            case "Swipe": type = GestureType.Swipe; break;
                            case "Pinch": type = GestureType.Pinch; break;
                        }
                        if (!Attempts.ContainsKey(type)) {
                            Attempts.Add(type, new List<Attempt>());
                        }

                        string[] para = line.Trim().Split('[', ']')[1].Split(':');
                        PracticeTime.Add(type, new TimeSpan(Int32.Parse(para[0]), Int32.Parse(para[1]), Int32.Parse(para[2])));

                    } else if(line.Contains("Grid height: 10")) {
                        size = GridSize.Small;
                    }
                    else if(line.Contains("Grid height: 5")) {
                        size = GridSize.Large;
                    }
                    else if (line.Contains("Target")) {
                        Attempt attempt = new Attempt(line, size);
                        Attempts[type].Add(attempt);
                    }
                }
            }
            foreach(var g in Attempts)
            {
                TestStart.Add(g.Key, g.Value[0].Time);
                g.Value.RemoveAt(0);
                PracticeTime[g.Key] = TestStart[g.Key] - PracticeTime[g.Key];
            }
        }

        public void GenerateHTML() {
            using(StreamReader sr = new StreamReader(directory + "template.html"))
            using(StreamWriter sw = new StreamWriter(directory + ID + ".html")) {
                string line = "";
                while((line = sr.ReadLine()) != null) {
                    if(line.Contains("%ID%"))
                    {
                        line = "var ID = " + ID + ";";
                    }
                    else if (line.Contains("%Tilt%") || line.Contains("%Swipe%") || line.Contains("%Throw%") || line.Contains("%Pinch%")) {
                        GestureType type = GetTypePlaceHolder(line);
                        line = GetJSPercentageArray(GetHitsPerTry(Attempts[type]), type);
                        line += GetJSTimeArray(GetTimePerTarget(Attempts[type], TestStart[type]), type);
                    } 
                    sw.WriteLine(line);
                }
            }
        }

        public void DrawAllHitBoxes() {
            foreach(var gesture in Attempts) {
                DrawHitBox(gesture.Value, gesture.Key.ToString());
            }
        }

        public static void CreateAverageHitboxes(List<Test> tests)
        {
            Test averageTest = new Test();
            averageTest.ID = "Average";
            foreach (var test in tests)
            {
                foreach (var gesture in test.Attempts)
                {
                    if (averageTest.Attempts.ContainsKey(gesture.Key))
                    {
                        averageTest.Attempts[gesture.Key].AddRange(gesture.Value);
                    }
                    else
                    {
                        averageTest.Attempts.Add(gesture.Key, new List<Attempt>(gesture.Value));
                    }
                }
            }
            averageTest.DrawAllHitBoxes();
        }

        private static Dictionary<GestureType, float[]> GetAverageHitPercentagePerTurn(List<Test> tests) {

            List<GestureType> gestures = new List<GestureType> { GestureType.Pinch, GestureType.Swipe, GestureType.Throw, GestureType.Tilt };

            Dictionary<GestureType, float[]> averageHitPercentagePerGesture = new Dictionary<GestureType, float[]>();

            foreach (var gesture in gestures) {

                float[] avgPercentage = new float[tests[0].Attempts[0].Count];
                List<float[]> percentages = new List<float[]>();

                foreach (var test in tests) {
                    percentages.Add(GetHitsPerTry(test.Attempts[gesture]));
                }

                for (int i = 0; i < avgPercentage.Length; i++) {
                    foreach (var percentage in percentages) {
                        avgPercentage[i] += percentage[i];
                    }
                    avgPercentage[i] /= (float)percentages.Count;
                }
                averageHitPercentagePerGesture.Add(gesture, avgPercentage);
            }

            return averageHitPercentagePerGesture;

        }
        private static Dictionary<GestureType, float[]> GetAverageTimePerTarget(List<Test> tests) {
            List<GestureType> gestures = new List<GestureType> { GestureType.Pinch, GestureType.Swipe, GestureType.Throw, GestureType.Tilt };

            Dictionary<GestureType, float[]> averageTimePerGesture = new Dictionary<GestureType, float[]>();

            foreach (var gesture in gestures) {

                float[] averageTime = new float[tests[0].Attempts[0].Count];
                List<float[]> times = new List<float[]>();

                foreach(var test in tests) {
                    times.Add(GetTimePerTarget(test.Attempts[gesture], test.TestStart[gesture]));
                }

                for (int i = 0; i < averageTime.Length; i++) {
                    foreach (var time in times) {
                        averageTime[i] += time[i];
                    }
                    averageTime[i] /= (float)times.Count;
                }
                averageTimePerGesture.Add(gesture, averageTime);
            }

            return averageTimePerGesture;
        }

        public static void GenerateAverageHTML(List<Test> tests) {

            var averageHitPercentagePerGesture = GetAverageHitPercentagePerTurn(tests);
            var averageTimePerTargetPerGesture = GetAverageTimePerTarget(tests);


            using (StreamReader sr = new StreamReader(directory + "template.html"))
            using (StreamWriter sw = new StreamWriter(directory + "Average" + ".html")) {
                string line = "";
                while ((line = sr.ReadLine()) != null) {

                    if (line.Contains("%ID%")) {
                        line = "var ID = \"Average\";";
                    } else if ( line.Contains("%Tilt%") || line.Contains("%Swipe%") || line.Contains("%Throw%") || line.Contains("%Pinch%")) {

                        GestureType type = GetTypePlaceHolder(line);
                        line = GetJSPercentageArray(averageHitPercentagePerGesture[type], type);
                        line += GetJSTimeArray(averageTimePerTargetPerGesture[type], type);

                    } 
                    sw.WriteLine(line);
                }
            }

        }

        private static Dictionary<GestureType, TimeSpan> GetAveragePracticeTimePerGesture(List<Test> tests) {
            Dictionary<GestureType, TimeSpan> averagePracticeTimes = new Dictionary<GestureType, TimeSpan>();

            foreach(var test in tests) {
                foreach(var time in test.PracticeTime) {
                    if (!averagePracticeTimes.ContainsKey(time.Key)) {
                        averagePracticeTimes.Add(time.Key, time.Value);
                    }
                    else {
                        averagePracticeTimes[time.Key] += time.Value;
                    }
                }
            }

            foreach(var time in averagePracticeTimes) {
                //time.Value = TimeSpan.FromSeconds(time.Value.TotalSeconds / tests.Count);
            }

            return averagePracticeTimes;
        }

        private static GestureType GetTypePlaceHolder(string line) {
             if (line.Contains("%Tilt%")) {
                return GestureType.Tilt;
            } else if (line.Contains("%Swipe%")) {
                return GestureType.Swipe;
            } else if (line.Contains("%Throw%")) {
                return GestureType.Throw;
            } else if (line.Contains("%Pinch%")) {
                return GestureType.Pinch;
            }

            return GestureType.Throw;
        }

        private void DrawHitBox(List<Attempt> attempts, string fileName) {

            //61 pixel sized squares, makes it better to look at
            int cellSize = 61;
            int bmsize = cellSize * 3;

            Bitmap hitbox = new Bitmap(bmsize, bmsize);
            Graphics hBGraphic = Graphics.FromImage(hitbox);
            hBGraphic.FillRectangle(Brushes.White, 0, 0, bmsize, bmsize);
            hBGraphic.DrawRectangle(new Pen(Brushes.Black, 1.0f), cellSize, cellSize, cellSize, cellSize);

            foreach (var attempt in attempts) {
                Brush brush = attempt.Size == GridSize.Large ? Brushes.Red : Brushes.Green;
                float scale = attempt.Size == GridSize.Large ? 122.0f : 61.0f;
                Point p = new Point(attempt.TargetCell.X, attempt.TargetCell.Y);
                p.X = p.X * scale; p.Y = p.Y * scale;
                p.X = attempt.Pointer.X - p.X;
                p.Y = attempt.Pointer.Y - p.Y;
                if(attempt.Size == GridSize.Large) {
                    p.X /= 2;
                    p.Y /= 2;
                }

                p.X += cellSize;
                p.Y += cellSize;

                if (!((p.X < 0) && (p.X >= bmsize)) || !((p.Y < 0) && (p.Y >= bmsize))) {
                    hBGraphic.FillRectangle(brush, (float)p.X, (float)p.Y, 2, 2);
                }
            }
            

            hBGraphic.Save();
            if(!Directory.Exists(directory + ID + "/")) { Directory.CreateDirectory(directory + ID + "/"); }
            hitbox.Save(directory + ID + "/" + fileName +  ".bmp");


        //Changed grid size.Grid height: 10 Grid width: 20 Cell height: 61.4 Cell width: 60.7
        //Changed grid size.Grid height: 5 Grid width: 10 Cell height: 122.8 Cell width: 121.4

        }

        private static float[] GetHitsPerTry(List<Attempt> attempts) {
            
            int hits = 0; float[] hitsAtTries = new float[attempts.Count]; int currentAttempt = 0;
            foreach (var attempt in attempts) {
                if (attempt.Hit) {
                    hits++;
                }
                hitsAtTries[currentAttempt++] = (float)hits / ((float)currentAttempt);
            }


            return hitsAtTries;
        }

        private static float[] GetTimePerTarget(List<Attempt> attempts, TimeSpan start)
        {

           float[] timeAtTries = new float[attempts.Count]; int currentAttempt = 0;
            foreach (var attempt in attempts)
            {
                float timeAtTarget = (float) (attempt.Time.TotalSeconds - start.TotalSeconds);
                
                timeAtTries[currentAttempt++] = timeAtTarget;
                start = attempt.Time;
            }

            return timeAtTries;
        }

        private static string GetJSPercentageArray(float[] percentages, GestureType type)
        {

            //var data = [ [[0, 0], [1, 1], [1,0]] ];

            string array = " [ ";
            for (int i = 0; i < percentages.Length; i++)
            {
                float percentage = (float)percentages[i] * 100.0f;
                string sPercentage = percentage.ToString().Replace(',', '.');
                array += "[" + (i + 1) + ", " + sPercentage + "], ";
            }

            array = array.Remove(array.Length - 2);
            array += " ];\n";

            return "var " + type + "Data = " + array;
        }

        private static string GetJSTimeArray(float[] times, GestureType type)
        {

            //var data = [ [[0, 0], [1, 1], [1,0]] ];

            string array = " [ ";
            for (int i = 0; i < times.Length; i++)
            {
                float time = (float)times[i];
                string sTime = time.ToString().Replace(',', '.');
                array += "[" + (i + 1) + ", " + sTime + "], ";
            }

            array = array.Remove(array.Length - 2);
            array += " ];\n";

            return "var Time" + type + "Data = " + array;
        }
    }
}
