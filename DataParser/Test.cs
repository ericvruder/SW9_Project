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

        public string ID { get; }

        public Test(List<Test> tests) {
            if(gestures.Count == 0) {
                this.gestures = tests[0].gestures;
                tests.RemoveAt(0);
            }
            foreach(var test in tests) {
                foreach(var gesture in test.gestures) {
                    foreach(var size in gestures.Values) {
                        foreach(var length in size.Values) {
                            foreach(var attempt in length.Values) {
                                attempt
                            }
                        }
                    }
                }
            }
        }

        //Dictionary<GestureDirection, Dictionary<GestureType, List<Attempt>>> tests = new Dictionary<GestureDirection, Dictionary<GestureType, List<Attempt>>>();
        protected Dictionary<GestureType, Dictionary<GridSize, Dictionary<JumpLength, List<Attempt>>>> gestures = new Dictionary<GestureType, Dictionary<GridSize, Dictionary<JumpLength, List<Attempt>>>>();

        public Test(string path) {

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
                        if (!gestures.ContainsKey(type)) {
                            gestures.Add(type, new Dictionary<GridSize, Dictionary<JumpLength, List<Attempt>>>());

                            gestures[type].Add(GridSize.Large, new Dictionary<JumpLength, List<Attempt>>());
                            gestures[type][GridSize.Large].Add(JumpLength.Short, new List<Attempt>());
                            gestures[type][GridSize.Large].Add(JumpLength.Medium, new List<Attempt>());
                            gestures[type][GridSize.Large].Add(JumpLength.Long, new List<Attempt>());
                            gestures[type][GridSize.Large].Add(JumpLength.NA, new List<Attempt>());


                            gestures[type].Add(GridSize.Small, new Dictionary<JumpLength, List<Attempt>>());
                            gestures[type][GridSize.Small].Add(JumpLength.Short, new List<Attempt>());
                            gestures[type][GridSize.Small].Add(JumpLength.Medium, new List<Attempt>());
                            gestures[type][GridSize.Small].Add(JumpLength.Long, new List<Attempt>());
                            gestures[type][GridSize.Small].Add(JumpLength.NA, new List<Attempt>());
                        }
                    } else if(line.Contains("Grid height: 10")) {
                        size = GridSize.Small;
                    }
                    else if(line.Contains("Grid height: 5")) {
                        size = GridSize.Large;
                    }
                    else if (line.Contains("Target")) {
                        Attempt attempt = new Attempt(line, size);
                        gestures[type][attempt.Size][attempt.Length].Add(attempt);
                    }
                }
            }
        }

        public void GenerateHTML() {
            using(StreamReader sr = new StreamReader(directory + "template.html"))
            using(StreamWriter sw = new StreamWriter(directory + ID + ".html")) {
                string line = "";
                while((line = sr.ReadLine()) != null) {

                    if (line.Contains("%Tilt%")) { line = GetHitsPerTry(GestureType.Tilt); } 
                    else if(line.Contains("%Swipe%")) { line = GetHitsPerTry(GestureType.Pinch); } 
                    else if(line.Contains("%Throw%")) { line = GetHitsPerTry(GestureType.Swipe); } 
                    else if(line.Contains("%Pinch%")) { line = GetHitsPerTry(GestureType.Throw); }

                    sw.WriteLine(line);
                }
            }
        }

        public void DrawHitBoxs() {

            //List<Attempt> attempts = tests[GestureType.Swipe][GridSize.Large][JumpLength.Long];
            List<Attempt> attempts = new List<Attempt>();

           

            foreach (var gesture in gestures) {
                
                foreach(var gridSize in gesture.Value) {

                    float scale = gridSize.Key == GridSize.Large ? 122.0f : 61.0f;
                    int bmsize = (int)scale * 3;

                    Bitmap hitbox = new Bitmap(bmsize, bmsize);
                    Graphics hBGraphic = Graphics.FromImage(hitbox);
                    hBGraphic.FillRectangle(Brushes.White, 0, 0, bmsize, bmsize);
                    hBGraphic.DrawRectangle(new Pen(Brushes.Black, 1.0f), scale, scale, scale, scale);

                    foreach (var length in gridSize.Value) {
                        foreach(var attempt in length.Value) {
                            Point p = new Point(attempt.TargetCell.X, attempt.TargetCell.Y);
                            p.X = p.X * scale; p.Y = p.Y * scale;
                            p.X = attempt.Pointer.X - p.X + scale;
                            p.Y = attempt.Pointer.Y - p.Y + scale;

                            if (!((p.X < 0) && (p.X >= bmsize)) || !((p.Y < 0) && (p.X >= bmsize))) {
                                hBGraphic.FillRectangle(Brushes.Red, (float)p.X, (float)p.Y, 2, 2);
                            }
                        }
                    }

                    hBGraphic.Save();
                    if(!Directory.Exists(directory + ID + "/")) { Directory.CreateDirectory(directory + ID + "/"); }
                    hitbox.Save(directory + ID + "/" + gesture.Key + "_" + gridSize.Key +  ".bmp");
                }
            }

        //Changed grid size.Grid height: 10 Grid width: 20 Cell height: 61.4 Cell width: 60.7
        //Changed grid size.Grid height: 5 Grid width: 10 Cell height: 122.8 Cell width: 121.4

        }

        private string GetHitsPerTry(GestureType type) {

            List<Attempt> attempts = new List<Attempt>();

            foreach(var size in gestures[type]) {
                foreach(var length in size.Value) {
                    foreach(var attempt in length.Value) {
                        attempts.Add(attempt);
                    }
                }
            }
            //var data = [ [[0, 0], [1, 1], [1,0]] ];
            
            int hits = 0; int[] hitsAtTries = new int[attempts.Count]; int currentAttempt = 0;
            foreach (var attempt in attempts) {
                if (attempt.Hit) {
                    hits++;
                }
                hitsAtTries[currentAttempt++] = hits;
            }

            string array = " [ [";
            for (int i = 0; i < attempts.Count; i++) {
                double percentage = (double)hitsAtTries[i] / ((double)i + 1.0) * 100.0;
                array += "[" + (i + 1) + ", " + percentage + "], ";
            }

            array = array.Remove(array.Length - 2);
            array += "] ];";

            return "var " + type + "Data = " + array;
        }
    }
}
