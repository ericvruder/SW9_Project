using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SW9_Project;
using System.IO;

namespace DataParser {
    class Test {

        private static string directory = "..\\..\\..\\InfoPages/";

        public string ID { get; }

        //Dictionary<GestureDirection, Dictionary<GestureType, List<Attempt>>> tests = new Dictionary<GestureDirection, Dictionary<GestureType, List<Attempt>>>();
        Dictionary<GestureType, Dictionary<GridSize, Dictionary<JumpLength, List<Attempt>>>> tests = new Dictionary<GestureType, Dictionary<GridSize, Dictionary<JumpLength, List<Attempt>>>>();

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
                        if (!tests.ContainsKey(type)) {
                            tests.Add(type, new Dictionary<GridSize, Dictionary<JumpLength, List<Attempt>>>());

                            tests[type].Add(GridSize.Large, new Dictionary<JumpLength, List<Attempt>>());
                            tests[type][GridSize.Large].Add(JumpLength.Short, new List<Attempt>());
                            tests[type][GridSize.Large].Add(JumpLength.Medium, new List<Attempt>());
                            tests[type][GridSize.Large].Add(JumpLength.Long, new List<Attempt>());
                            tests[type][GridSize.Large].Add(JumpLength.NA, new List<Attempt>());


                            tests[type].Add(GridSize.Small, new Dictionary<JumpLength, List<Attempt>>());
                            tests[type][GridSize.Small].Add(JumpLength.Short, new List<Attempt>());
                            tests[type][GridSize.Small].Add(JumpLength.Medium, new List<Attempt>());
                            tests[type][GridSize.Small].Add(JumpLength.Long, new List<Attempt>());
                            tests[type][GridSize.Small].Add(JumpLength.NA, new List<Attempt>());
                        }
                    } else if(line.Contains("Grid height: 10")) {
                        size = GridSize.Small;
                    }
                    else if(line.Contains("Grid height: 5")) {
                        size = GridSize.Large;
                    }
                    else if (line.Contains("Target")) {
                        Attempt attempt = new Attempt(line, size);
                        tests[type][attempt.Size][attempt.Length].Add(attempt);
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

        private string GetHitsPerTry(GestureType type) {

            List<Attempt> attempts = new List<Attempt>();

            foreach(var size in tests[type]) {
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
