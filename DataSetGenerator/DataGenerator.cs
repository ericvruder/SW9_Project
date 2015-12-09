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

        static string TestFileDirectory { get { return ".\\..\\..\\..\\Testlog/"; } }

        public static int GetTechniqueNumber(GestureType gesturetype) {

            switch (gesturetype)
            {
                case GestureType.Throw:
                    return 1;
                    break;
                case GestureType.Pinch:
                    return 2;
                    break;
                case GestureType.Tilt:
                    return 3;
                    break;
                case GestureType.Swipe:
                    return 4;
                    break;
                default:
                    return 0;
                    break;
            }
        }

        public static int GetGridsizeNumber(GridSize gridsize)
        {
            switch (gridsize)
            {
                case GridSize.Small:
                    return 0;
                case GridSize.Large:
                    return 1;
                default:
                    return -1;
            }
        }

        public static void GetUserInfoTechnique() {
            using (StreamWriter datawriter = new StreamWriter("user_technique_data.csv")) {
                string[] files = Directory.GetFiles(TestFileDirectory);
                datawriter.WriteLine("time1 time2 time3 time4 success1 success2 success3 success4 failure1 failure2 failure3 failure4");
                foreach (var file in files) {
                    string id = file.Split('/').Last().Split('.')[0];
                    Test t = new Test(new StreamReader(file), id);
                    string timeOut = "";
                    string successOut = "";
                    string failureOut = "";
                    foreach (var gesture in t.Attempts) {
                        string time = (t.Attempts[gesture.Key].Last().Time - t.TestStart[gesture.Key]).TotalSeconds.ToString();
                        float hitPercentage = Test.GetHitsPerTry(t.Attempts[gesture.Key]).Last() * 100f;
                        string totalHit = hitPercentage.ToString();
                        string totalError = (100f - hitPercentage).ToString();
                        timeOut += time + " ";
                        successOut += totalHit + " ";
                        failureOut += totalError + " ";
                    }
                    if(!string.IsNullOrEmpty(timeOut))
                        datawriter.WriteLine(timeOut + successOut + failureOut);
                }
            }
        }

        public static void GetAllTechniqueAttempts()
        {
            using (StreamWriter datawriter = new StreamWriter("all_technique_data.csv"))
            {
                string[] files = Directory.GetFiles(TestFileDirectory, "*.test");
                datawriter.WriteLine("technique hit");
                foreach (var file in files)
                {
                    string id = file.Split('/').Last().Split('.')[0];
                    Test t = new Test(new StreamReader(file), id);
                    foreach(var attempt in t.Attempts)
                    {
                        foreach(var a in t.Attempts[attempt.Key])
                        {
                            string hit = a.Hit ? "1" : "0";
                            datawriter.WriteLine(GetTechniqueNumber(attempt.Key) + " " + hit);
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
                datawriter.WriteLine("gridsize hit");
                foreach (var file in files)
                {
                    string id = file.Split('/').Last().Split('.')[0];
                    Test t = new Test(new StreamReader(file), id);
                    foreach (var attempt in t.Attempts)
                    {
                        foreach (var a in t.Attempts[attempt.Key])
                        {
                            string hit = a.Hit ? "1" : "0";
                            datawriter.WriteLine(GetGridsizeNumber(a.Size) + " " + hit);
                        }
                    }
                }
            }
        }
    }
}
