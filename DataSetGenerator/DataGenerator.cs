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
            using (StreamWriter anovadatawriter = new StreamWriter("user_technique_data_anova.csv"))
            using (StreamWriter datawriter = new StreamWriter("user_technique_data.csv")) {
                datawriter.WriteLine("ID TimePinch TotalHitPinch TotalErrorPinch" + 
                                       " TimeSwipe TotalHitSwipe TotalErrorSwipe" +
                                       " TimeThrow TotalHitThrow TotalErrorThrow" +
                                       " TimeTilt TotalHitTilt TotalErrorTilt");

                anovadatawriter.WriteLine("ID Technique Time Hit Error");
                string[] files = Directory.GetFiles(TestFileDirectory, "*.test");
                foreach (var file in files) {
                    string id = file.Split('/').Last().Split('.')[0];
                    Test t = new Test(new StreamReader(file), id);
                    string line = t.ID;
                    foreach (var gesture in AllTypes) {
                        string time = (t.Attempts[gesture].Last().Time - t.TestStart[gesture]).TotalSeconds.ToString();
                        float hitPercentage = Test.GetHitsPerTry(t.Attempts[gesture]).Last() * 100f;
                        string totalHit = hitPercentage.ToString();
                        string totalError = (100f - hitPercentage).ToString();
                        line += " " + time + " " + totalHit + " " + totalError;

                        anovadatawriter.WriteLine(id + " " + GetTechniqueNumber(gesture) + " " + time + " " + hitPercentage + " " + totalError);
                    }
                    datawriter.WriteLine(line);
                }
            }
        }

        public static int GetTechniqueNumber(GestureType gesturetype) {

            switch (gesturetype)
            {
                case GestureType.Pinch:
                    return 1;
                case GestureType.Swipe:
                    return 2;
                case GestureType.Throw:
                    return 3;
                case GestureType.Tilt:
                    return 4;
                default:
                    return 0;
            }
        }

        public static int GetGridsizeNumber(GridSize gridsize)
        {
            switch (gridsize)
            {
                case GridSize.Large:
                    return 0;
                case GridSize.Small:
                    return 1;
                default:
                    return -1;
            }
        }

        public static void GetAllTechniqueAttempts()
        {
            using (StreamWriter datawriter = new StreamWriter("all_technique_data.csv"))
            {
                datawriter.WriteLine("Technique HitOrMiss Time");
                string[] files = Directory.GetFiles(TestFileDirectory, "*.test");
                foreach (var file in files)
                {
                    string id = file.Split('/').Last().Split('.')[0];
                    Test t = new Test(new StreamReader(file), id);
                    foreach(var attempt in t.Attempts) {
                        TimeSpan currTime = t.TestStart[attempt.Key];
                        foreach (var a in t.Attempts[attempt.Key]) {
                            string time = (a.Time.TotalSeconds - currTime.TotalSeconds).ToString();
                            currTime = a.Time;
                            string hit = a.Hit ? "1" : "0";
                            datawriter.WriteLine(GetTechniqueNumber(attempt.Key) + " " + hit + " " + time);
                        }
                    }
                }
            }
        }

        public static void GetAllGridSizeAttempts()
        {
            using (StreamWriter datawriter = new StreamWriter("all_gridsize_data.csv"))
            {
                datawriter.WriteLine("GridSize HitOrMiss Time");
                string[] files = Directory.GetFiles(TestFileDirectory, "*.test");
                foreach (var file in files)
                {
                    string id = file.Split('/').Last().Split('.')[0];
                    Test t = new Test(new StreamReader(file), id);
                    foreach (var attempt in t.Attempts)
                    {
                        TimeSpan currTime = t.TestStart[attempt.Key];
                        foreach (var a in t.Attempts[attempt.Key])
                        {
                            string time = (a.Time.TotalSeconds - currTime.TotalSeconds).ToString();
                            currTime = a.Time;
                            string hit = a.Hit ? "1" : "0";
                            datawriter.WriteLine(GetGridsizeNumber(a.Size) + " " + hit + " " + time);
                        }
                    }
                }
            }
        }

        public static void GetUserGridSizeData() {
            using (StreamWriter anovadatawriter = new StreamWriter("user_gridsize_data_anova.csv"))
            using (StreamWriter datawriter = new StreamWriter("user_gridsize_data.csv")) {
                datawriter.WriteLine("ID LargeTotalTime LargeTotalHitPercent LargeTotalMissPercent" +
                                       " SmallTotalTime SmallTotalHitPercent SmallTotalMissPercent");
                anovadatawriter.WriteLine("ID GridSize Time Hit Miss");
                string[] files = Directory.GetFiles(TestFileDirectory, "*test");
                foreach (var file in files) {
                    string id = file.Split('/').Last().Split('.')[0];
                    Test t = new Test(new StreamReader(file), id);
                    float largeTotalTime = 0, largeTotalHit = 0, largeTotalMiss = 0;
                    float smallTotalTime = 0, smallTotalHit = 0, smallTotalMiss = 0;
                    foreach(var gesture in AllTypes) {
                        TimeSpan curr = t.TestStart[gesture];
                        foreach (var attempt in t.Attempts[gesture]) {
                            if(attempt.Size == GridSize.Large) {
                                largeTotalTime += (int)(attempt.Time.TotalSeconds - curr.TotalSeconds);
                                if(attempt.Hit) {
                                    largeTotalHit++;
                                } else {
                                    largeTotalMiss++;
                                }
                            }
                            else {
                                smallTotalTime += (int)(attempt.Time.TotalSeconds - curr.TotalSeconds);
                                if (attempt.Hit) {
                                    smallTotalHit++;
                                } else {
                                    smallTotalMiss++;
                                }
                            }
                            curr = attempt.Time;
                        }
                    }
                    string lTHP = ((largeTotalHit / (largeTotalHit + largeTotalMiss)) * 100f).ToString();
                    string lTMP = ((largeTotalMiss / (largeTotalHit + largeTotalMiss)) * 100f).ToString();

                    string sTHP = ((smallTotalHit / (smallTotalHit + smallTotalMiss)) * 100f).ToString();
                    string sTMP = ((smallTotalMiss / (smallTotalHit + smallTotalMiss)) * 100f).ToString();

                    string line = t.ID + " " + largeTotalTime + " " + lTHP + " " + lTMP + " " + smallTotalTime + " " + sTHP + " " + sTMP;
                    datawriter.WriteLine(line);
                    anovadatawriter.WriteLine(t.ID + " " + GetGridsizeNumber(GridSize.Large) + " " + largeTotalTime + " " + lTHP + " " + lTMP);
                    anovadatawriter.WriteLine(t.ID + " " + GetGridsizeNumber(GridSize.Small) + " " + smallTotalTime + " " + sTHP + " " + sTMP);

                }
            }

        }
    }
}
