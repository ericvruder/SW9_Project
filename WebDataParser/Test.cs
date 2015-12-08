using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using SW9_Project;
using System.IO;

namespace WebDataParser {
    public class Test {
        
        public string ID { get; set; }

        public Test(List<Test> tests) {
        }
        public Dictionary<GestureType, List<Attempt>> Attempts { get; set; }
        public Dictionary<GestureType, TimeSpan> TestStart { get; set; }
        public Dictionary<GestureType, TimeSpan> PracticeTime { get; set; }
        public Dictionary<GestureType, MemoryStream> ImageFile { get; set; }

        private Test() {
            Attempts = new Dictionary<GestureType, List<Attempt>>();
            TestStart = new Dictionary<GestureType, TimeSpan>();
            PracticeTime = new Dictionary<GestureType, TimeSpan>();
        }
        public Test(StreamReader sr, string id) : this() {

            ID = id;
            
            using (sr) {
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

        public static float[] GetTimePerTarget(List<Attempt> attempts, TimeSpan start) {

            float[] timeAtTries = new float[attempts.Count]; int currentAttempt = 0;
            foreach (var attempt in attempts) {
                float timeAtTarget = (float)(attempt.Time.TotalSeconds - start.TotalSeconds);

                timeAtTries[currentAttempt++] = timeAtTarget;
                start = attempt.Time;
            }

            return timeAtTries;
        }
    }
}
