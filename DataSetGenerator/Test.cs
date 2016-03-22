using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DataSetGenerator {
    public class Test {
        
        public String ID { get; set; }

        public Dictionary<GestureType, List<Attempt>> Attempts { get; set; }

        private Test() {
            Attempts = new Dictionary<GestureType, List<Attempt>>();
        }

        public Test(int id) : this(DataGenerator.TestFileDirectory + id + ".test") { }
        
        public Test(String path) : this() {
            
            StreamReader sr = new StreamReader(path);
            ID = path.Split('/').Last().Split('.')[0];
            
            using (sr) {
                string line = "";
                GridSize size = GridSize.Large;
                GestureType type = GestureType.Pinch;
                GestureDirection direction = GestureDirection.Push;
                TimeSpan currentTime = TimeSpan.Zero;
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

                        tobesearched = "Direction: ";
                        toBefound = line.Substring(line.IndexOf(tobesearched) + tobesearched.Length).Split(' ')[0];
                        direction = toBefound == "Push" ? GestureDirection.Push : GestureDirection.Pull;
                        if (!Attempts.ContainsKey(type)) {
                            Attempts.Add(type, new List<Attempt>());
                        }

                        string[] para = line.Trim().Split('[', ']')[1].Split(':');
                        currentTime = new TimeSpan(Int32.Parse(para[0]), Int32.Parse(para[1]), Int32.Parse(para[2]));

                    } else if(line.Contains("Grid height: 10")) {
                        size = GridSize.Small;
                    }
                    else if(line.Contains("Grid height: 5")) {
                        size = GridSize.Large;
                    }
                    else if (line.Contains("Target")) {

                        string[] para = line.Trim().Split('[', ']')[1].Split(':');
                        var cTime = new TimeSpan(Int32.Parse(para[0]), Int32.Parse(para[1]), Int32.Parse(para[2]));
                        var attemptTime = cTime - currentTime;
                        currentTime = cTime;
                        Attempt attempt = new Attempt(ID, line, attemptTime, size, direction, type);
                        Attempts[type].Add(attempt);
                    }
                }
            }
        }

        public static float[] GetHitsPerTry(List<Attempt> attempts) {

            int hits = 0; float[] hitsAtTries = new float[attempts.Count]; int currentAttempt = 0;
            foreach (var attempt in attempts) {
                if (attempt.Hit) {
                    hits++;
                }
                hitsAtTries[currentAttempt++] = (float)hits / ((float)currentAttempt);
            }


            return hitsAtTries;
        }

        public static float[] GetTimePerTarget(List<Attempt> attempts) {

            float[] timeAtTries = new float[attempts.Count]; int currentAttempt = 0;
            foreach (var attempt in attempts) {

                timeAtTries[currentAttempt++] = (float)attempt.Time.TotalSeconds;
            }

            return timeAtTries;
        }
    }
}
