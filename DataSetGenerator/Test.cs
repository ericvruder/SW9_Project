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
        
        public Test(String path, bool old = false) : this() {
            StreamReader sr = new StreamReader(path);
            ID = path.Split('/').Last().Split('.')[0];

            var TotalTime = new Dictionary<GestureType, TimeSpan>();
            var PracticeTime = new Dictionary<GestureType, TimeSpan>();
            TimeSpan attemptTime = default(TimeSpan);

            using (sr) {
                string line = "";
                GridSize size = GridSize.Large;
                GestureType type = GestureType.Pinch;
                GestureDirection direction = GestureDirection.Push;
                TimeSpan currentTime = TimeSpan.Zero;
                while ((line = sr.ReadLine()) != null) {
                    if(line == "") { continue; }
                    string[] time = line.Trim().Split('[', ']')[1].Split(':');
                    TimeSpan entryTime = new TimeSpan(Int32.Parse(time[0]), Int32.Parse(time[1]), Int32.Parse(time[2]));
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
                        PracticeTime.Add(type, entryTime);
                        currentTime = new TimeSpan(Int32.Parse(para[0]), Int32.Parse(para[1]), Int32.Parse(para[2]));
                    }
                    else if (line.Contains("Grid height: 10")) {
                        size = GridSize.Small;
                    }
                    else if (line.Contains("Grid height: 5")) {
                        size = GridSize.Large;
                    }
                    else if (line.Contains("Target")) {
                        if (!old)
                        {
                            string[] para = line.Trim().Split('[', ']')[1].Split(':');
                            var cTime = new TimeSpan(Int32.Parse(para[0]), Int32.Parse(para[1]), Int32.Parse(para[2]));
                            attemptTime = cTime - currentTime;
                            currentTime = cTime;
                        }
                        else
                        {
                            attemptTime = TimeSpan.Zero;
                        }

                        Attempt attempt = new Attempt(ID, line, attemptTime, size, direction, type);
                        Attempts[type].Add(attempt);
                    }
                }
            }

            if (old)
            {
                foreach (var g in Attempts) {
                    TotalTime.Add(g.Key, g.Value[0].Time);
                    g.Value.RemoveAt(0);
                    PracticeTime[g.Key] = TotalTime[g.Key] - PracticeTime[g.Key];


                }
                foreach (var gesture in Attempts) {
                    TimeSpan currTime = TotalTime[gesture.Key];
                    TimeSpan totTime = TimeSpan.Zero;
                    foreach (var attempt in Attempts[gesture.Key]) {
                        TimeSpan t = new TimeSpan(attempt.Time.Ticks);
                        attempt.Time = t - currTime;
                        currTime = t;
                        totTime += attempt.Time;
                    }
                    TotalTime[gesture.Key] = totTime;
                }
            }
        }

        public Test(List<Attempt> attempts) : this() {
            ID = attempts[0].ID;
            foreach(var technique in DataGenerator.AllTechniques) {
                var techniqueQuery = from attempt in attempts
                                     where attempt.Type == technique
                                     select attempt;
                Attempts[technique] = techniqueQuery.ToList();
            }
        }
    }
}
