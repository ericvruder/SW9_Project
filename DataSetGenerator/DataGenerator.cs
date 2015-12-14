﻿using System;
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

        public static void GetAllUserData() {
            using (StreamWriter datawriter = new StreamWriter("user_data.csv")) {
                datawriter.WriteLine("ID TimePinch TotalHitPinch TotalErrorPinch" + 
                                       " TimeSwipe TotalHitSwipe TotalErrorSwipe" +
                                       " TimeThrow TotalHitThrow TotalErrorThrow" +
                                       " TimeTilt TotalHitTilt TotalErrorTilt" +
                                       " LargeTotalTime LargeTotalHitPercent LargeTotalMissPercent" +
                                       " SmallTotalTime SmallTotalHitPercent SmallTotalMissPercent");
                List<Test> tests = GetFixedTests();
                foreach (var t in tests) {
                    string line = t.ID;
                    float largeTotalTime = 0, largeTotalHit = 0, largeTotalMiss = 0;
                    float smallTotalTime = 0, smallTotalHit = 0, smallTotalMiss = 0;
                    foreach (var gesture in AllTypes) {
                        TimeSpan curr = t.TestStart[gesture];
                        foreach (var attempt in t.Attempts[gesture]) {
                            if (attempt.Size == GridSize.Large) {
                                largeTotalTime += (int)(attempt.Time.TotalSeconds - curr.TotalSeconds);
                                if (attempt.Hit) {
                                    largeTotalHit++;
                                } else {
                                    largeTotalMiss++;
                                }
                            } else {
                                smallTotalTime += (int)(attempt.Time.TotalSeconds - curr.TotalSeconds);
                                if (attempt.Hit) {
                                    smallTotalHit++;
                                } else {
                                    smallTotalMiss++;
                                }
                            }
                            curr = attempt.Time;
                        }
                        string time = (t.Attempts[gesture].Last().Time - t.TestStart[gesture]).TotalSeconds.ToString();
                        float hitPercentage = Test.GetHitsPerTry(t.Attempts[gesture]).Last() * 100f;
                        string totalHit = hitPercentage.ToString();
                        string totalError = (100f - hitPercentage).ToString();
                        line += " " + time + " " + totalHit + " " + totalError;

                        
                    }
                    string lTHP = ((largeTotalHit / (largeTotalHit + largeTotalMiss)) * 100f).ToString();
                    string lTMP = ((largeTotalMiss / (largeTotalHit + largeTotalMiss)) * 100f).ToString();

                    string sTHP = ((smallTotalHit / (smallTotalHit + smallTotalMiss)) * 100f).ToString();
                    string sTMP = ((smallTotalMiss / (smallTotalHit + smallTotalMiss)) * 100f).ToString();
                    line += " " + largeTotalTime + " " + lTHP + " " + lTMP + " " + smallTotalTime + " " + sTHP + " " + sTMP;
                    datawriter.WriteLine(line);
                }
            }
        }

        public static void GetAllTargetData()
        {
            using (StreamWriter datawriter = new StreamWriter("target_data.csv"))
            {
                List<Test> tests = GetFixedTests();
                datawriter.WriteLine("GridSize Technique HitOrMiss Time");
                foreach (var t in tests)
                {
                    foreach (var attempt in t.Attempts)
                    {
                        TimeSpan currTime = t.TestStart[attempt.Key];
                        foreach (var a in t.Attempts[attempt.Key])
                        {
                            string time = (a.Time.TotalSeconds - currTime.TotalSeconds).ToString();
                            currTime = a.Time;
                            string hit = a.Hit ? "1" : "0";
                            datawriter.WriteLine(GetGridsizeNumber(a.Size) + " " + GetTechniqueNumber(attempt.Key) + " " + hit + " " + time);
                        }
                    }
                }
            }
        }

        public static List<Test> GetFixedTests() {
            List<Test> tests = new List<Test>();
            string[] files = Directory.GetFiles(TestFileDirectory, "*.test");
            foreach (var file in files) {
                string id = file.Split('/').Last().Split('.')[0];
                tests.Add(new Test(new StreamReader(file), id));
            }
            return tests;
        }

        public static void GetUserTwoWayData() {
            using (StreamWriter datawriter = new StreamWriter("user_twoway_data.csv")) {
                datawriter.WriteLine("ID PinchLargeTime PinchSmallTime PinchLargeHit PinchSmallHit" +
                                       " SwipeLargeTime SwipeSmallTime SwipeLargeHit SwipeSmallHit" +
                                       " ThrowLargeTime ThrowSmallTime ThrowLargeHit ThrowSmallHit" +
                                       " TiltLargeTime TiltSmallTime TiltLargeHit TiltSmallHit");

                List<Test> tests = GetFixedTests();
                foreach (var t in tests) {
                    int[,] time = new int[4, 2];
                    float[,] hit = new float[4, 2];
                    foreach(var gesture in AllTypes) {
                        int gIndex = GetTechniqueNumber(gesture) - 1;
                        TimeSpan curr = t.TestStart[gesture];
                        foreach(var attempt in t.Attempts[gesture]) {
                            int sIndex = attempt.Size == GridSize.Large ? 0 : 1;
                            time[gIndex, sIndex] += (int)(attempt.Time - curr).TotalSeconds;
                            hit[gIndex, sIndex] += attempt.Hit ? 1 : 0;
                            curr = attempt.Time;
                        }
                    }

                    for(int i = 0; i < 4; i++) {
                        for(int j = 0; j < 2; j++) {
                            hit[i, j] = (hit[i, j] / 9f) * 100f;
                        }
                    }

                    string line = t.ID + " " + time[0, 0] + " " + time[0, 1] + " " + hit[0, 0] + " " + hit[0, 1];
                    line +=              " " + time[1, 0] + " " + time[1, 1] + " " + hit[1, 0] + " " + hit[1, 1];
                    line +=              " " + time[2, 0] + " " + time[2, 1] + " " + hit[2, 0] + " " + hit[2, 1];
                    line +=              " " + time[3, 0] + " " + time[3, 1] + " " + hit[3, 0] + " " + hit[3, 1];

                }
            }
        }

        public static void GetTargetTwoWayData() {
           
        }

        public static int GetTechniqueNumber(GestureType gesturetype) {

            switch (gesturetype) {
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

        public static int GetGridsizeNumber(GridSize gridsize) {
            switch (gridsize) {
                case GridSize.Large:
                    return 0;
                case GridSize.Small:
                    return 1;
                default:
                    return -1;
            }
        }
    }
}
