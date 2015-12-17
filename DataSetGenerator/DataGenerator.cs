﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WebDataParser;
using System.Globalization;
using SW9_Project;
using System.Drawing;
using System.Drawing.Imaging;

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
                List<Test> tests = GetTests();
                foreach (var t in tests) {
                    string line = t.ID;
                    float largeTotalTime = 0, largeTotalHit = 0, largeTotalMiss = 0;
                    float smallTotalTime = 0, smallTotalHit = 0, smallTotalMiss = 0;
                    foreach (var gesture in AllTypes) {
                        foreach (var attempt in t.Attempts[gesture]) {
                            if (attempt.Size == GridSize.Large) {
                                largeTotalTime += (int)attempt.Time.TotalSeconds;
                                if (attempt.Hit) {
                                    largeTotalHit++;
                                } else {
                                    largeTotalMiss++;
                                }
                            } else {
                                smallTotalTime += (int)attempt.Time.TotalSeconds;
                                if (attempt.Hit) {
                                    smallTotalHit++;
                                } else {
                                    smallTotalMiss++;
                                }
                            }
                        }
                        string time = t.TotalTime[gesture].TotalSeconds.ToString();
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
            using (StreamWriter largeWriter = new StreamWriter("large_target_data.csv"))
            using (StreamWriter smallWriter = new StreamWriter("small_target_data.csv"))
            using (StreamWriter datawriter = new StreamWriter("target_data.csv")) {
                List<Test> tests = GetTests();
                datawriter.WriteLine("ID GridSize Technique HitOrMiss Time");
                smallWriter.WriteLine("ID GridSize Technique HitOrMiss Time");
                largeWriter.WriteLine("ID GridSize Technique HitOrMiss Time");
                foreach (var t in tests) {
                    foreach (var attempt in t.Attempts) {
                        foreach (var a in t.Attempts[attempt.Key]) {
                            string time = a.Time.TotalSeconds.ToString();
                            string hit = a.Hit ? "1" : "0";
                            StreamWriter sizeWriter = a.Size == GridSize.Large ? largeWriter : smallWriter;
                            string line = t.ID + " " + GetGridsizeNumber(a.Size) + " " + GetTechniqueNumber(attempt.Key) + " " + hit + " " + time;
                            datawriter.WriteLine(line);
                            sizeWriter.WriteLine(line);
                        }
                    }
                }
            }
        }

        public static List<Test> GetTests() {
            List<Test> tests = new List<Test>();
            string[] files = Directory.GetFiles(TestFileDirectory, "*.test");
            foreach (var file in files) {
                string id = file.Split('/').Last().Split('.')[0];
                tests.Add(new Test(new StreamReader(file), id));

                switch (tests.Last().ID)
                {
                    case "1":
                        tests.Last().Attempts[GestureType.Tilt][14].Time = TimeSpan.FromSeconds(6);
                        tests.Last().Attempts[GestureType.Throw][4].Time = TimeSpan.FromSeconds(8);
                        tests.Last().Attempts[GestureType.Throw][4].Size = GridSize.Small;
                        tests.Last().Attempts[GestureType.Swipe][5].Time = TimeSpan.FromSeconds(6);
                        tests.Last().Attempts[GestureType.Swipe][5].Size = GridSize.Large;
                        tests.Last().Attempts[GestureType.Swipe][11].Time = TimeSpan.FromSeconds(6);
                        tests.Last().Attempts[GestureType.Swipe][11].Size = GridSize.Large;
                        tests.Last().Attempts[GestureType.Swipe][13].Time = TimeSpan.FromSeconds(6);
                        tests.Last().Attempts[GestureType.Swipe][13].Size = GridSize.Large;
                        break;
                    case "2":
                        tests.Last().Attempts[GestureType.Swipe][1].Time = TimeSpan.FromSeconds(6);
                        tests.Last().Attempts[GestureType.Swipe][13].Time = TimeSpan.FromSeconds(6);
                        break;
                    case "4":
                        tests.Last().Attempts[GestureType.Throw][17].Time = TimeSpan.FromSeconds(7);
                        tests.Last().Attempts[GestureType.Throw][17].Size = GridSize.Large;
                        tests.Last().Attempts[GestureType.Tilt][12].Time = TimeSpan.FromSeconds(5);
                        tests.Last().Attempts[GestureType.Tilt][14].Time = TimeSpan.FromSeconds(6);
                        tests.Last().Attempts[GestureType.Tilt][14].Size = GridSize.Small;
                        break;
                    case "5":
                        tests.Last().Attempts[GestureType.Swipe][14].Time = TimeSpan.FromSeconds(4);
                        break;
                    case "8":
                        tests.Last().Attempts[GestureType.Throw][4].Time = TimeSpan.FromSeconds(8);
                        tests.Last().Attempts[GestureType.Throw][4].Size = GridSize.Small;
                        break;
                    default:
                        break;
                }
            }
            return tests;
        }

        public static void GetUserTwoWayData() {
            using (StreamWriter datawriter = new StreamWriter("user_twoway_data.csv")) {
                datawriter.WriteLine("ID PinchLargeTime PinchSmallTime PinchLargeHit PinchSmallHit" +
                                       " SwipeLargeTime SwipeSmallTime SwipeLargeHit SwipeSmallHit" +
                                       " ThrowLargeTime ThrowSmallTime ThrowLargeHit ThrowSmallHit" +
                                       " TiltLargeTime TiltSmallTime TiltLargeHit TiltSmallHit");

                List<Test> tests = GetTests();
                foreach (var t in tests) {
                    int[,] time = new int[4, 2];
                    float[,] hit = new float[4, 2];
                    foreach(var gesture in AllTypes) {
                        int gIndex = GetTechniqueNumber(gesture) - 1;
                        foreach(var attempt in t.Attempts[gesture]) {
                            int sIndex = attempt.Size == GridSize.Large ? 0 : 1;
                            time[gIndex, sIndex] += (int)attempt.Time.TotalSeconds;
                            hit[gIndex, sIndex] += attempt.Hit ? 1 : 0;
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
                    datawriter.WriteLine(line);
                }
            }
        }
        public static void GetTargetTechniqueANOVAData() {

            using (StreamWriter datawriter = new StreamWriter("target_oneway_technique_data.csv")) {
                datawriter.WriteLine("ID PinchTime PinchHit" +
                                       " SwipeTime SwipeHit" +
                                       " ThrowTime ThrowHit" +
                                       " TiltTime TiltHit");
                List<Test> tests = GetTests();

                foreach (var test in tests) {

                    Dictionary<GestureType, List<int>> times = new Dictionary<GestureType, List<int>>();
                    Dictionary<GestureType, List<string>> hits = new Dictionary<GestureType, List<string>>();

                    foreach (var gesture in AllTypes) {
                        if (!times.ContainsKey(gesture)) {
                            times.Add(gesture, new List<int>());
                            hits.Add(gesture, new List<string>());
                        }

                        foreach (var attempt in test.Attempts[gesture]) {
                            times[gesture].Add((int)attempt.Time.TotalSeconds);
                            hits[gesture].Add(attempt.Hit ? "1" : "0");
                        }
                    }
                    for (int tryN = 0; tryN < times[GestureType.Pinch].Count(); tryN++) {
                        string line = test.ID;


                        /* "ID PinchLargeTime PinchSmallTime PinchLargeHit PinchSmallHit" +
                           " SwipeLargeTime SwipeSmallTime SwipeLargeHit SwipeSmallHit" +
                           " ThrowLargeTime ThrowSmallTime ThrowLargeHit ThrowSmallHit" +
                           " TiltLargeTime TiltSmallTime TiltLargeHit TiltSmallHit"); */

                        line += " " + times[GestureType.Pinch][tryN] + " " + hits[GestureType.Pinch][tryN];
                        line += " " + times[GestureType.Swipe][tryN] + " " + hits[GestureType.Swipe][tryN];
                        line += " " + times[GestureType.Throw][tryN] + " " + hits[GestureType.Throw][tryN];
                        line += " " + times[GestureType.Tilt][tryN] +  " " + hits[GestureType.Tilt][tryN];
                        datawriter.WriteLine(line);
                    }

                }
            }
        }

        public static void GetTargetGridANOVAData() {

            using (StreamWriter datawriter = new StreamWriter("target_oneway_grid_data.csv")) {
                datawriter.WriteLine("ID LargeTime LargeHit" +
                                       " SmallTime SmallHit");
                List<Test> tests = GetTests();

                foreach (var test in tests) {
                    List<Attempt> sAttempts = new List<Attempt>(), lAttempts = new List<Attempt>();

                    foreach(var s in test.Attempts) {
                        sAttempts.AddRange(from attempt in s.Value
                                           where attempt.Size == GridSize.Small
                                           select attempt);
                        lAttempts.AddRange(from attempt in s.Value
                                           where attempt.Size == GridSize.Large
                                           select attempt);
                    }

                    for(int i = 0; i < sAttempts.Count; i++) {
                        string line = test.ID + " " + lAttempts[i].Time.TotalSeconds + " " + (lAttempts[i].Hit ? "1" : "0") + " " + sAttempts[i].Time.TotalSeconds + " " + (sAttempts[i].Hit ? "1" : "0");
                        datawriter.WriteLine(line);
                    }

                }
            }

        }

        public static void GetTargetTwoWayData() {

            using (StreamWriter datawriter = new StreamWriter("target_twoway_data.csv")) {
                datawriter.WriteLine("ID PinchLargeTime PinchSmallTime PinchLargeHit PinchSmallHit" +
                                       " SwipeLargeTime SwipeSmallTime SwipeLargeHit SwipeSmallHit" +
                                       " ThrowLargeTime ThrowSmallTime ThrowLargeHit ThrowSmallHit" +
                                       " TiltLargeTime TiltSmallTime TiltLargeHit TiltSmallHit");
                List<Test> tests = GetTests();

                foreach (var test in tests) {

                    Dictionary<GestureType, List<int>> sTimes = new Dictionary<GestureType, List<int>>();
                    Dictionary<GestureType, List<string>> sHits = new Dictionary<GestureType, List<string>>();

                    Dictionary<GestureType, List<int>> lTimes = new Dictionary<GestureType, List<int>>();
                    Dictionary<GestureType, List<string>> lHits = new Dictionary<GestureType, List<string>>();

                    foreach (var gesture in AllTypes) {
                        if (!sTimes.ContainsKey(gesture)) {
                            sTimes.Add(gesture, new List<int>());
                            sHits.Add(gesture, new List<string>());
                            lTimes.Add(gesture, new List<int>());
                            lHits.Add(gesture, new List<string>());
                        }

                        var stList = from attempt in test.Attempts[gesture]
                                     where attempt.Size == GridSize.Small
                                     select attempt;

                        var ltList = from attempt in test.Attempts[gesture]
                                     where attempt.Size == GridSize.Large
                                     select attempt;

                        foreach (var attempt in stList) {
                            sTimes[gesture].Add((int)attempt.Time.TotalSeconds);
                            sHits[gesture].Add(attempt.Hit ? "1" : "0");
                        }
                        foreach (var attempt in ltList) {
                            lTimes[gesture].Add((int)attempt.Time.TotalSeconds);
                            lHits[gesture].Add(attempt.Hit ? "1" : "0");
                        }
                    }
                    for(int tryN = 0; tryN < sTimes[GestureType.Pinch].Count(); tryN++) {
                        string line = test.ID;


                        /* "ID PinchLargeTime PinchSmallTime PinchLargeHit PinchSmallHit" +
                           " SwipeLargeTime SwipeSmallTime SwipeLargeHit SwipeSmallHit" +
                           " ThrowLargeTime ThrowSmallTime ThrowLargeHit ThrowSmallHit" +
                           " TiltLargeTime TiltSmallTime TiltLargeHit TiltSmallHit"); */

                        line += " " + lTimes[GestureType.Pinch][tryN] + " " + sTimes[GestureType.Pinch][tryN] + " " + lHits[GestureType.Pinch][tryN] + " " + sHits[GestureType.Pinch][tryN];
                        line += " " + lTimes[GestureType.Swipe][tryN] + " " + sTimes[GestureType.Swipe][tryN] + " " + lHits[GestureType.Swipe][tryN] + " " + sHits[GestureType.Swipe][tryN];
                        line += " " + lTimes[GestureType.Throw][tryN] + " " + sTimes[GestureType.Throw][tryN] + " " + lHits[GestureType.Throw][tryN] + " " + sHits[GestureType.Throw][tryN];
                        line += " " + lTimes[GestureType.Tilt][tryN] + " " + sTimes[GestureType.Tilt][tryN] + " " + lHits[GestureType.Tilt][tryN] + " " + sHits[GestureType.Tilt][tryN];
                        datawriter.WriteLine(line);
                    }

                }
            }
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

        public static void GetWrongTargetTests()
        {
            List<Test> tests = GetTests();
            foreach (var test in tests)
            {
                foreach (var gesture in AllTypes)
                {
                    bool show = false;
                    Attempt t = test.Attempts[gesture].First();
                    foreach (var attepmt in test.Attempts[gesture])
                    {
                        if (attepmt == t)
                        {
                            continue;
                        }
                        if (attepmt.TargetCell.X == t.TargetCell.X && attepmt.TargetCell.Y == t.TargetCell.Y)
                        {
                            Console.WriteLine(test.ID + " " + gesture + " " + test.Attempts[gesture].IndexOf(attepmt));
                            show = true;
                        }
                        t = attepmt;
                    }
                    var listLarge = from attempt in test.Attempts[gesture]
                                    where attempt.Size == GridSize.Large
                                    select attempt;
                    var listSmall = from attempt in test.Attempts[gesture]
                                    where attempt.Size == GridSize.Small
                                    select attempt;
                    if (show)
                        Console.WriteLine("largeGrid = " + listLarge.Count() + " smallGrid = " + listSmall.Count());
                }

            }
            Console.ReadLine();
        }

        public static void VerifyTests()
        {
            List<Test> tests = GetTests();
            foreach (var test in tests)
            {
                foreach (var gesture in AllTypes)
                {
                    var listLarge = from attempt in test.Attempts[gesture]
                                    where attempt.Size == GridSize.Large
                                    select attempt;
                    var listSmall = from attempt in test.Attempts[gesture]
                                    where attempt.Size == GridSize.Small
                                    select attempt;
                    if(listSmall.Count() != listLarge.Count())
                        Console.WriteLine("Test ID: " + test.ID + " FAILED");
                }
            }
        }

        private static void DrawHitBox(List<Attempt> attempts, string fileName) {

            //61 pixel sized squares, makes it better to look at
            int cellSize = 61;
            int bmsize = cellSize * 3;

            Bitmap hitbox = new Bitmap(bmsize, bmsize);
            Graphics hBGraphic = Graphics.FromImage(hitbox);
            hBGraphic.FillRectangle(Brushes.White, 0, 0, bmsize, bmsize);
            hBGraphic.DrawRectangle(new Pen(Brushes.Black, 1.0f), cellSize, cellSize, cellSize, cellSize);

            foreach (var attempt in attempts) {
                Brush brush = Brushes.Red;
                float scale = attempt.Size == GridSize.Large ? 122.0f : 61.0f;
                WebDataParser.Point p = new WebDataParser.Point(attempt.TargetCell.X, attempt.TargetCell.Y);
                p.X = p.X * scale; p.Y = p.Y * scale;
                p.X = attempt.Pointer.X - p.X;
                p.Y = attempt.Pointer.Y - p.Y;
                if (attempt.Size == GridSize.Large) {
                    p.X /= 2;
                    p.Y /= 2;
                }

                p.X += cellSize;
                p.Y += cellSize;

                if((p.X > cellSize && p.X < cellSize*2) && (p.Y > cellSize && p.Y < cellSize * 2)) {
                    brush = Brushes.Green;
                }

                if (!((p.X < 0) && (p.X >= bmsize)) || !((p.Y < 0) && (p.Y >= bmsize))) {
                    hBGraphic.FillRectangle(brush, (float)p.X, (float)p.Y, 2, 2);
                }
            }

            hBGraphic.Save();

            hitbox.Save(fileName);

            hBGraphic.Dispose();
            hitbox.Dispose();

        }

        public static void CreateHitboxes() {
            var tests = GetTests();
            Dictionary<GestureType, List<Attempt>> techAttempts = new Dictionary<GestureType, List<Attempt>>();
            Dictionary<GridSize, List<Attempt>> sizeAttempts = new Dictionary<GridSize, List<Attempt>>();
            sizeAttempts.Add(GridSize.Large, new List<Attempt>());
            sizeAttempts.Add(GridSize.Small, new List<Attempt>());
            foreach (var test in tests) {
                foreach(var gesture in AllTypes) {
                    if (!techAttempts.ContainsKey(gesture)) {
                        techAttempts.Add(gesture, new List<Attempt>());
                    }
                    techAttempts[gesture].AddRange(test.Attempts[gesture]);
                    sizeAttempts[GridSize.Small].AddRange(from attempt in test.Attempts[gesture]
                                where attempt.Size == GridSize.Small
                                select attempt);
                    sizeAttempts[GridSize.Large].AddRange(from attempt in test.Attempts[gesture]
                                where attempt.Size == GridSize.Large
                                select attempt);
                }
            }

            DrawHitBox(techAttempts[GestureType.Pinch], "pinch.png");
            DrawHitBox(techAttempts[GestureType.Swipe], "swipe.png");
            DrawHitBox(techAttempts[GestureType.Throw], "throw.png");
            DrawHitBox(techAttempts[GestureType.Tilt], "tilt.png");

            DrawHitBox(sizeAttempts[GridSize.Large], "large.png");
            DrawHitBox(sizeAttempts[GridSize.Small], "small.png");

            var lp = from attempt in techAttempts[GestureType.Pinch]
                     where attempt.Size == GridSize.Large
                     select attempt;
            DrawHitBox(lp.ToList(), "pinchlarge.png");
            var sp = from attempt in techAttempts[GestureType.Pinch]
                     where attempt.Size == GridSize.Small
                     select attempt;
            DrawHitBox(sp.ToList(), "pinchsmall.png");

            var ls = from attempt in techAttempts[GestureType.Swipe]
                     where attempt.Size == GridSize.Large
                     select attempt;
            DrawHitBox(ls.ToList(), "swipelarge.png");
            var ss = from attempt in techAttempts[GestureType.Swipe]
                     where attempt.Size == GridSize.Small
                     select attempt;
            DrawHitBox(ss.ToList(), "swipesmall.png");

            var lti = from attempt in techAttempts[GestureType.Tilt]
                     where attempt.Size == GridSize.Large
                     select attempt;
            DrawHitBox(lti.ToList(), "tiltlarge.png");
            var sti = from attempt in techAttempts[GestureType.Tilt]
                     where attempt.Size == GridSize.Small
                     select attempt;
            DrawHitBox(sti.ToList(), "tiltsmall.png");

            var lth = from attempt in techAttempts[GestureType.Throw]
                      where attempt.Size == GridSize.Large
                      select attempt;
            DrawHitBox(lth.ToList(), "throwlarge.png");
            var sth = from attempt in techAttempts[GestureType.Throw]
                      where attempt.Size == GridSize.Small
                      select attempt;
            DrawHitBox(sth.ToList(), "throwsmall.png");

        }

    }
}
