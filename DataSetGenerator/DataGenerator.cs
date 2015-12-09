using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using WebDataParser;
using System.Globalization;
using SW9_Project;

namespace DataSetGenerator {

    static class DataGenerator {

        private static List<GestureType> AllTypes = new List<GestureType> { GestureType.Pinch, GestureType.Swipe, GestureType.Throw, GestureType.Tilt };

        static string TestFileDirectory { get { return ".\\..\\..\\..\\Testlog/"; } }

        public static void GetUserTechniqueData() {
            using (StreamWriter datawriter = new StreamWriter("user_technique_data.csv")) {
                string[] files = Directory.GetFiles(TestFileDirectory, "*.test");
                foreach (var file in files) {
                    string id = file.Split('/').Last().Split('.')[0];
                    Test t = new Test(new StreamReader(file), id);
                    string line = t.ID;
                    foreach(var gesture in AllTypes) {
                        string time = (t.Attempts[gesture].Last().Time - t.TestStart[gesture]).TotalSeconds.ToString();
                        float hitPercentage = Test.GetHitsPerTry(t.Attempts[gesture]).Last() * 100f;
                        string totalHit = hitPercentage.ToString();
                        string totalError = (100f - hitPercentage).ToString();
                        line += " " + time + " " + totalHit + " " + totalError;
                    }
                    datawriter.WriteLine(line);
                }
            }
        }

        public static void GetAllTechniqueAttempts()
        {
            using (StreamWriter datawriter = new StreamWriter("all_technique_data.csv"))
            {
                string[] files = Directory.GetFiles(TestFileDirectory, "*.test");
                foreach (var file in files)
                {
                    string id = file.Split('/').Last().Split('.')[0];
                    Test t = new Test(new StreamReader(file), id);
                    foreach(var attempt in t.Attempts)
                    {
                        foreach(var a in t.Attempts[attempt.Key])
                        {
                            string type = attempt.Key.ToString();
                            string hit = a.Hit ? "1" : "0";
                            datawriter.WriteLine(type + " " + hit);
                        }
                    }
                }
            }
        }

        public static void GetAllGridSizeAttempts()
        {
            using (StreamWriter datawriter = new StreamWriter("all_gridsize_data.csv"))
            {
                string[] files = Directory.GetFiles(TestFileDirectory, "*.test");
                foreach (var file in files)
                {
                    string id = file.Split('/').Last().Split('.')[0];
                    Test t = new Test(new StreamReader(file), id);
                    foreach (var attempt in t.Attempts)
                    {
                        foreach (var a in t.Attempts[attempt.Key])
                        {
                            string gridsize = a.Size.ToString();
                            string hit = a.Hit ? "1" : "0";
                            datawriter.WriteLine(gridsize + " " + hit);
                        }
                    }
                }
            }
        }

        public static void GetUserGridSizeData() {
            using (StreamWriter datawriter = new StreamWriter("user_gridsize_data.csv")) {
                string[] files = Directory.GetFiles(TestFileDirectory);
                foreach (var file in files) {
                    string id = file.Split('/').Last().Split('.')[0];
                    Test t = new Test(new StreamReader(file), id);
                    TimeSpan largeTotalTime = TimeSpan.Zero, smallTotalTime = TimeSpan.Zero;
                    foreach (var gesture in t.Attempts) {
                        TimeSpan curr = t.TestStart[gesture.Key];
                        foreach(var attempt in gesture.Value) {
                            if(attempt.Size == SW9_Project.GridSize.Large) {

                            }
                        }
                        string time = (t.Attempts[gesture.Key].Last().Time - t.TestStart[gesture.Key]).TotalSeconds.ToString();
                        float hitPercentage = Test.GetHitsPerTry(t.Attempts[gesture.Key]).Last() * 100f;
                        string totalHit = hitPercentage.ToString();
                        string totalError = (100f - hitPercentage).ToString();
                        datawriter.WriteLine(t.ID + ", " + gesture.Key + ", " + time + ", " + totalHit + ", " + totalError);
                    }
                    
                }
            }

        }
    }
}
