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

        public Test(int id, DataSource source) : this(DataGenerator.TestFileDirectory(source) + id + ".test", source) { }
        
        public Test(String path, DataSource source) : this() {
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
                        
                        currentTime = entryTime;
                    }
                    else if (line.Contains("Grid height: 10")) {
                        size = GridSize.Small;
                    }
                    else if (line.Contains("Grid height: 5")) {
                        size = GridSize.Large;
                    }
                    else if (line.Contains("Target")) {
                        if (line.Contains("JL: NA")) {
                            currentTime = entryTime;
                            continue;
                        }
                        attemptTime = entryTime - currentTime;
                        currentTime = entryTime;
                        

                        Attempt attempt = new Attempt(ID, line, attemptTime, size, direction, type);
                        Attempts[type].Add(attempt);
                    }
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
