using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SW9_Project;

namespace DataParser {
    class Program {
        private static string directory = "..\\..\\..\\Testlog/";
        static void Main(string[] args) {

            string[] files = Directory.GetFiles(directory, "*.test");
            var tests = new Dictionary<GestureDirection, Dictionary<GestureType, List<Attempt>>>();

            foreach(var s in files) {
                List<Attempt> attempts = new List<Attempt>();
                using (StreamReader sr = new StreamReader(s)) {
                    string line = "";
                    while((line = sr.ReadLine()) != null) {
                        if(line.Contains("Started new gesture test.")) {
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
                            string[] tadasdas = line.Substring(line.IndexOf(tobesearched) + tobesearched.Length).Split(' ');
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
                        }
                        else if (line.Contains("Target")) {
                            attempts.Add(new Attempt(line));
                        }
                    }
                }
                foreach(var direction in tests) {
                    foreach(var type in direction.Value) {
                        int hits = 0; int[] hitsAtTries = new int[25]; int currentAttempt = 0;
                        foreach(var attempt in type.Value) {
                            if (attempt.Hit) {
                                hits++;
                            }
                            if(currentAttempt == 0) {
                                hitsAtTries[0] = hits;
                            }
                            else {
                                for(int i = 0; i < currentAttempt; )
                            }
                        }
                    }
                }
                Console.ReadLine();
            }
        }
    }
}
