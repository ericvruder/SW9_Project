﻿using System;
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

        private Test() {
            Attempts = new Dictionary<GestureType, List<Attempt>>();
            TestStart = new Dictionary<GestureType, TimeSpan>();
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
                        TestStart.Add(type, new TimeSpan(Int32.Parse(para[0]), Int32.Parse(para[1]), Int32.Parse(para[2])));

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
        }

        public void GenerateHTML() {
            using(StreamReader sr = new StreamReader(directory + "template.html"))
            using(StreamWriter sw = new StreamWriter(directory + ID + ".html")) {
                string line = "";
                while((line = sr.ReadLine()) != null) {

                    if (line.Contains("%Tilt%")) {
                        line = GetJSAvgPercentageArray(GetHitsPerTry(Attempts[GestureType.Tilt]), GestureType.Tilt);
                    } 
                    else if(line.Contains("%Swipe%")) {
                        line = GetJSAvgPercentageArray(GetHitsPerTry(Attempts[GestureType.Swipe]), GestureType.Swipe); ;
                    } 
                    else if(line.Contains("%Throw%")) {
                        line = GetJSAvgPercentageArray(GetHitsPerTry(Attempts[GestureType.Throw]), GestureType.Throw); ;
                    } 
                    else if(line.Contains("%Pinch%")) {
                        line = GetJSAvgPercentageArray(GetHitsPerTry(Attempts[GestureType.Pinch]), GestureType.Pinch); ;
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

        public static void CreateAverageLearningGraph(List<Test> tests) {

            List<GestureType> gestures = new List<GestureType> { GestureType.Pinch, GestureType.Swipe, GestureType.Throw, GestureType.Tilt };

            Dictionary<GestureType, float[]> averageHitPercentagePerGesture = new Dictionary<GestureType, float[]>();

            foreach(var gesture in gestures) {

                float[] avgPercentage = new float[tests[0].Attempts[0].Count];
                List<float[]> percentages = new List<float[]>();

                foreach(var test in tests) {
                    percentages.Add(GetHitsPerTry(test.Attempts[gesture]));
                }

                for(int i = 0; i < avgPercentage.Length; i++) {
                    foreach(var percentage in percentages) {
                        avgPercentage[i] += percentage[i];
                    }
                    avgPercentage[i] /= (float)percentages.Count;
                }
                averageHitPercentagePerGesture.Add(gesture, avgPercentage);
            }


            using (StreamReader sr = new StreamReader(directory + "template.html"))
            using (StreamWriter sw = new StreamWriter(directory + "Average" + ".html")) {
                string line = "";
                while ((line = sr.ReadLine()) != null) {

                    if (line.Contains("%Tilt%")) {
                        line = GetJSAvgPercentageArray(averageHitPercentagePerGesture[GestureType.Tilt], GestureType.Tilt);
                    } 
                    else if (line.Contains("%Swipe%")) {
                        line = GetJSAvgPercentageArray(averageHitPercentagePerGesture[GestureType.Swipe], GestureType.Swipe);
                    } 
                    else if (line.Contains("%Throw%")) {
                        line = GetJSAvgPercentageArray(averageHitPercentagePerGesture[GestureType.Throw], GestureType.Throw);
                    } 
                    else if (line.Contains("%Pinch%")) {
                        line = GetJSAvgPercentageArray(averageHitPercentagePerGesture[GestureType.Pinch], GestureType.Pinch);
                    }

                    sw.WriteLine(line);
                }
            }

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

        private static string GetJSAvgPercentageArray(float[] percentages, GestureType type)
        {

            //var data = [ [[0, 0], [1, 1], [1,0]] ];

            string array = " [ ";
            for (int i = 0; i < percentages.Length; i++)
            {
                float percentage = (float)percentages[i] * 100.0f;
                array += "[" + (i + 1) + ", " + percentage + "], ";
            }

            array = array.Remove(array.Length - 2);
            array += " ];";

            return "var " + type + "Data = " + array;
        }
    }
}
