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

        Dictionary<GestureDirection, Dictionary<GestureType, List<Attempt>>> tests = new Dictionary<GestureDirection, Dictionary<GestureType, List<Attempt>>>();
        public Test(string path) {

            ID = path.Split('/').Last().Split('.').First();

            List<Attempt> attempts = new List<Attempt>();
            using (StreamReader sr = new StreamReader(path)) {
                string line = "";
                while ((line = sr.ReadLine()) != null) {
                    if (line.Contains("Started new gesture test.")) {
                        GestureType type = GestureType.Pinch; GestureDirection direction = GestureDirection.Pull;
                        string tobesearched = "Type: ";
                        string toBefound = line.Substring(line.IndexOf(tobesearched) + tobesearched.Length).Split(' ')[0];
                        switch (toBefound) {
                            case "Throw": type = GestureType.Throw; break;
                            case "Tilt": type = GestureType.Tilt; break;
                            case "Swipe": type = GestureType.Swipe; break;
                            case "Pinch": type = GestureType.Pinch; break;
                        }
                        tobesearched = "Direction:";
                        toBefound = line.Substring(line.IndexOf(tobesearched) + tobesearched.Length).Split(' ')[1];
                        switch (toBefound) {
                            case "Push": direction = GestureDirection.Push; break;
                            case "Pull": direction = GestureDirection.Pull; break;
                        }
                        if (!tests.ContainsKey(direction)) {
                            tests.Add(direction, new Dictionary<GestureType, List<Attempt>>());
                        }
                        if (!tests[direction].ContainsKey(type)) {
                            tests[direction].Add(type, new List<Attempt>());
                        }
                        attempts = tests[direction][type];
                    } else if (line.Contains("Target")) {
                        attempts.Add(new Attempt(line));
                    }
                }
            }
        }

        public void GenerateHTML() {
            using(StreamReader sr = new StreamReader(directory + "template.html"))
            using(StreamWriter sw = new StreamWriter(directory + ID + ".html")) {
                string line = "";
                while((line = sr.ReadLine()) != null) {
                    if(line.Contains("var pullTiltData =")) { line += GetHitsPerTry(GestureDirection.Pull, GestureType.Tilt); }
                    else if(line.Contains("var pullPinchData =")) { line += GetHitsPerTry(GestureDirection.Pull, GestureType.Pinch); } 
                    else if(line.Contains("var pullSwipeData =")) { line += GetHitsPerTry(GestureDirection.Pull, GestureType.Swipe); } 
                    else if(line.Contains("var pullThrowData =")) { line += GetHitsPerTry(GestureDirection.Pull, GestureType.Throw); } 

                    else if(line.Contains("var pushTiltData =")) { line += GetHitsPerTry(GestureDirection.Push, GestureType.Tilt); } 
                    else if(line.Contains("var pushPinchData =")) { line += GetHitsPerTry(GestureDirection.Push, GestureType.Pinch); } 
                    else if(line.Contains("var pushSwipeData =")) { line += GetHitsPerTry(GestureDirection.Push, GestureType.Swipe); } 
                    else if(line.Contains("var pushThrowData =")) { line += GetHitsPerTry(GestureDirection.Push, GestureType.Throw); }

                    sw.WriteLine(line);
                }
            }
        }

        private string GetHitsPerTry(GestureDirection direction, GestureType type) {

            List<Attempt> attempts = tests[direction][type];
            //var data = [ [[0, 0], [1, 1], [1,0]] ];
            
            int hits = 0; int[] hitsAtTries = new int[25]; int currentAttempt = 0;
            foreach (var attempt in attempts) {
                if (attempt.Hit) {
                    hits++;
                }
                hitsAtTries[currentAttempt++] = hits;
            }

            string array = " [ [";
            for (int i = 0; i < 25; i++) {
                double percentage = (double)hitsAtTries[i] / ((double)i + 1.0) * 100.0;
                array += "[" + (i + 1) + ", " + percentage + "], ";
            }

            array = array.Remove(array.Length - 2);
            array += "] ];";

            return array;
        }

        public void HitsPerTry() {

            foreach (var direction in tests) {
                foreach (var type in direction.Value) {
                    int hits = 0; int[] hitsAtTries = new int[25]; int currentAttempt = 0;
                    foreach (var attempt in type.Value) {
                        if (attempt.Hit) {
                            hits++;
                        }
                        hitsAtTries[currentAttempt++] = hits;
                    }
                    Console.WriteLine(type.Key + " " + direction.Key);
                    for (int i = 0; i < 25; i++) {
                        double percentage = (double)hitsAtTries[i] / ((double)i + 1.0) * 100.0;
                        Console.WriteLine("% on " + (i + 1).ToString("D2") + " try: " + percentage);
                    }
                }
            }
        }
    }
}
