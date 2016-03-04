using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using WebDataParser;
using SW9_Project;
using Newtonsoft.Json;

using Spss;

namespace DataSetGenerator {

    public static class DataGenerator {

        public static List<GestureType> AllTechniques = new List<GestureType> { GestureType.Pinch, GestureType.Swipe, GestureType.Throw, GestureType.Tilt };
        public static List<GestureDirection> AllDirections = new List<GestureDirection> { GestureDirection.Push, GestureDirection.Pull };

        public static string TestFileDirectory { get { return ".\\..\\..\\..\\Testlog/"; } }
        public static string DataDirectory
        {
            get
            {
                if (!Directory.Exists(".\\..\\..\\..\\Data/")) {
                    Directory.CreateDirectory(".\\..\\..\\..\\Data/");
                }
                return ".\\..\\..\\..\\Data/";
            }
        }


        public static List<Test> GetTests() {
            List<Test> tests = new List<Test>();
            string[] files = Directory.GetFiles(TestFileDirectory, "*.test");
            foreach (var file in files) {
                string id = file.Split('/').Last().Split('.')[0];
                tests.Add(new Test(file));

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

        

        public static void CreateCSVDocument() {

            List<int> testing = new List<int>();

            using (StreamWriter datawriter = new StreamWriter(DataDirectory + "target_data.csv")) {
                datawriter.WriteLine("ID PinchLargeTime PinchSmallTime PinchLargeHit PinchSmallHit PinchLargeDist PinchSmallDist PinchLargeXDist PinchSmallXDist PinchLargeYDist PinchSmallYDist" +
                                       " SwipeLargeTime SwipeSmallTime SwipeLargeHit SwipeSmallHit SwipeLargeDist SwipeSmallDist SwipeLargeXDist SwipeSmallXDist SwipeLargeYDist SwipeSmallYDist" +
                                       " ThrowLargeTime ThrowSmallTime ThrowLargeHit ThrowSmallHit ThrowLargeDist ThrowSmallDist ThrowLargeXDist ThrowSmallXDist ThrowLargeYDist ThrowSmallYDist" +
                                       " TiltLargeTime TiltSmallTime TiltLargeHit TiltSmallHit TiltLargeDist TiltSmallDist TiltLargeXDist TiltSmallXDist TiltLargeYDist TiltSmallYDist");
                List<Test> tests = GetTests();

                foreach (var test in tests) {

                    Dictionary<GestureType, List<int>> sTimes = new Dictionary<GestureType, List<int>>();
                    Dictionary<GestureType, List<string>> sHits = new Dictionary<GestureType, List<string>>();

                    Dictionary<GestureType, List<int>> lTimes = new Dictionary<GestureType, List<int>>();
                    Dictionary<GestureType, List<string>> lHits = new Dictionary<GestureType, List<string>>();

                    Dictionary<GestureType, List<double>> lDist = new Dictionary<GestureType, List<double>>();
                    Dictionary<GestureType, List<double>> lxDist = new Dictionary<GestureType, List<double>>();
                    Dictionary<GestureType, List<double>> lyDist = new Dictionary<GestureType, List<double>>();

                    Dictionary<GestureType, List<double>> sDist = new Dictionary<GestureType, List<double>>();
                    Dictionary<GestureType, List<double>> sxDist = new Dictionary<GestureType, List<double>>();
                    Dictionary<GestureType, List<double>> syDist = new Dictionary<GestureType, List<double>>();

                    foreach (var gesture in AllTechniques) {
                        if (!sTimes.ContainsKey(gesture)) {
                            sTimes.Add(gesture, new List<int>());
                            sHits.Add(gesture, new List<string>());
                            sDist.Add(gesture, new List<double>());
                            sxDist.Add(gesture, new List<double>());
                            syDist.Add(gesture, new List<double>());
                            lTimes.Add(gesture, new List<int>());
                            lHits.Add(gesture, new List<string>());
                            lDist.Add(gesture, new List<double>());
                            lxDist.Add(gesture, new List<double>());
                            lyDist.Add(gesture, new List<double>());
                        }

                        var stList = from attempt in test.Attempts[gesture]
                                     where attempt.Size == GridSize.Small
                                     select attempt;

                        var ltList = from attempt in test.Attempts[gesture]
                                     where attempt.Size == GridSize.Large
                                     select attempt;

                        foreach (var attempt in stList) {
                            var distances = GetDistances(attempt);
                            sTimes[gesture].Add((int)attempt.Time.TotalSeconds);
                            sHits[gesture].Add(attempt.Hit ? "1" : "0");
                            sDist[gesture].Add(distances.Item1);
                            sxDist[gesture].Add(distances.Item2);
                            syDist[gesture].Add(distances.Item3);
                        }
                        foreach (var attempt in ltList) {
                            var distances = GetDistances(attempt);
                            lTimes[gesture].Add((int)attempt.Time.TotalSeconds);
                            lHits[gesture].Add(attempt.Hit ? "1" : "0");
                            lDist[gesture].Add(distances.Item1);
                            lxDist[gesture].Add(distances.Item2);
                            lyDist[gesture].Add(distances.Item3);
                        }
                    }
                    for(int tryN = 0; tryN < sTimes[GestureType.Pinch].Count(); tryN++) {
                        string line = test.ID;

                        foreach(var gesture in AllTechniques) {
                            line += $" {lTimes[gesture][tryN]} {sTimes[gesture][tryN]} {lHits[gesture][tryN]} {sHits[gesture][tryN]} {lDist[gesture][tryN]} {sDist[gesture][tryN]} {lxDist[gesture][tryN]} {sxDist[gesture][tryN]} {lyDist[gesture][tryN]} {syDist[gesture][tryN]}";
                        }
                        
                        datawriter.WriteLine(line);
                    }

                }
            }
        }

        private static Tuple<double, double, double> GetDistances(Attempt attempt) {
            Tuple<double,double,double> result = new Tuple<double, double, double>(0,0,0);
            if (!attempt.Hit) {
                var distances = GetXYDistance(attempt);
                var distance = DistanceToTargetCell(attempt);
                if (distances.Item2 == 0 && distances.Item1 == 0) {
                    distance = 1;
                    distances = new Tuple<double, double>(1, 1);
                }
                result = new Tuple<double, double, double>(distance, distances.Item1, distances.Item2);
            } 
            return result;
        }

        public static void GetWrongTargetTests()
        {
            List<Test> tests = GetTests();
            foreach (var test in tests)
            {
                foreach (var gesture in AllTechniques)
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
                foreach (var gesture in AllTechniques)
                {
                    foreach(var attempt in test.Attempts[gesture]) {
                        if(attempt.Pointer.X > 1920 || attempt.Pointer.X < 0 || attempt.Pointer.Y > 1080 || attempt.Pointer.Y < 0) {
                            Console.WriteLine(attempt.Pointer);
                        }
                    }
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

        
        private static double DistanceSquare(Point v, Point w) {
            return Math.Pow(v.X - w.X, 2) + Math.Pow(v.Y - w.Y,2);
        }

        private static double DistanceToSegmentSquared(Point p, Point v, Point w) {
            double l2 = DistanceSquare(v, w);
            if(l2 == 0) { return DistanceSquare(p, v); }
            double t = ((p.X - v.X) * (w.X - v.X) + (p.Y - v.Y) * (w.Y - v.Y)) / l2;
            if(t < 0) { return DistanceSquare(p, v); }
            if(t > 1) { return DistanceSquare(p, w); }
            Point n = new Point(v.X + t * (w.X - v.X), v.Y + t * (w.Y - v.Y));
            return DistanceSquare(p, n);
        }

        private static double DistanceToSegment(Point p, Point ls, Point le) {
            return Math.Sqrt(DistanceToSegmentSquared(p, ls, le));
        }

        public static double DistanceToTargetCell(Attempt attempt) {
            double scale = attempt.Size == GridSize.Large ? 122.0f : 61.0f;
            List<Tuple<Point,Point>> lineSegments = new List<Tuple<Point, Point>>();
            Point t = new Point(attempt.TargetCell.X * scale, attempt.TargetCell.Y * scale);
            Point u = new Point(t.X, t.Y + scale);
            Point v = new Point(t.X + scale, t.Y + scale);
            Point w = new Point(t.X + scale, t.Y);
            lineSegments.Add(new Tuple<Point, Point>(t, u));
            lineSegments.Add(new Tuple<Point, Point>(t, w));
            lineSegments.Add(new Tuple<Point, Point>(u, v));
            lineSegments.Add(new Tuple<Point, Point>(v, w));
            List<double> distances = new List<double>();
            foreach(var line in lineSegments) {
                distances.Add(DistanceToSegment(attempt.Pointer, line.Item1, line.Item2));
            }

            return distances.Min();
        }
        
        public static Tuple<double, double> GetXYDistance(Attempt attempt) {
            double scale = attempt.Size == GridSize.Large ? 122.0f : 61.0f;
            double x = attempt.TargetCell.X * scale, y = attempt.TargetCell.Y * scale;
            double xDistance = 0, yDistance = 0;

            if(attempt.Pointer.X < x) {
                xDistance = x - attempt.Pointer.X;
            }
            else if(attempt.Pointer.X > x + scale) {
                xDistance = attempt.Pointer.X - (x + scale);
            }

            if (attempt.Pointer.Y < y) {
                yDistance = y - attempt.Pointer.Y;
            } else if (attempt.Pointer.Y > y + scale) {
                yDistance = attempt.Pointer.Y - (y + scale);
            }

            return new Tuple<double, double>(xDistance, yDistance);
        }


        public static void CreateSPSSDocument() {

            if (File.Exists(DataDirectory + "data.sav")) {
                File.Delete(DataDirectory + "data.sav");
            }


            List<Test> tests = DataGenerator.GetTests();

            using (SpssDataDocument doc = SpssDataDocument.Create(DataDirectory + "data.sav")) {
                CreateMetaData(doc);
                foreach (var test in tests) {
                    ParseTest(doc, test);
                }
            }

        }

        private static void AddVariableForTechnique(SpssDataDocument doc, GestureType type) {


            SpssNumericVariable time = new SpssNumericVariable();
            time.Name = $"{type}Time";
            time.Label = $"Time taken in seconds for the attempt using {type}";
            doc.Variables.Add(time);

            SpssNumericVariable hit = new SpssNumericVariable();
            hit.Name = $"{type}Hit";
            hit.Label = $"Whether the user hit the target or not using {type}";
            doc.Variables.Add(hit);

            SpssNumericVariable accuracy = new SpssNumericVariable();
            accuracy.Name = $"{type}Accuracy";
            accuracy.Label = $"Distance in pixels from target using {type}";
            doc.Variables.Add(accuracy);

            SpssNumericVariable gridSize = new SpssNumericVariable();
            gridSize.Name = $"{type}Size";
            gridSize.Label = $"Grid size for attempt using {type}";
            gridSize.ValueLabels.Add(0, "Small");
            gridSize.ValueLabels.Add(1, "Large");
            doc.Variables.Add(gridSize);

            SpssNumericVariable direction = new SpssNumericVariable();
            direction.Name = $"{type}Direction";
            direction.Label = $"Direction for attempt using {type}";
            direction.ValueLabels.Add(0, "Push");
            direction.ValueLabels.Add(1, "Pull");
            doc.Variables.Add(direction);

        }

        public static void CreateMetaData(SpssDataDocument doc) {

            SpssNumericVariable vID = new SpssNumericVariable();
            vID.Name = "ID";
            vID.Label = "User ID";
            doc.Variables.Add(vID);

            foreach (var technique in AllTechniques) {
                AddVariableForTechnique(doc, technique);
            }

            doc.CommitDictionary();
        }

        public static void ParseTest(SpssDataDocument doc, Test test) {
            int id = int.Parse(test.ID);
            int nAttempts = test.Attempts[GestureType.Pinch].Count;

            // GestureType.Pinch, GestureType.Swipe, GestureType.Throw, GestureType.Tilt 
            for (int i = 0; i < nAttempts; i++) {
                SpssCase gestureAttempts = doc.Cases.New();
                gestureAttempts["ID"] = id;
                foreach (var type in AllTechniques) {
                    gestureAttempts = AddTechniqueData(gestureAttempts, type, test.Attempts[type][i]);
                }
                gestureAttempts.Commit();

            }
        }

        public static SpssCase AddTechniqueData(SpssCase gestureAttempt, GestureType type, Attempt attempt) {

            gestureAttempt[$"{type}Time"] = attempt.Time.TotalSeconds;
            gestureAttempt[$"{type}Hit"] = attempt.Hit;
            gestureAttempt[$"{type}Accuracy"] = DistanceToTargetCell(attempt);
            gestureAttempt[$"{type}Size"] = attempt.Size;
            gestureAttempt[$"{type}Direction"] = attempt.Direction;

            return gestureAttempt;
        }

        public static void GenerateJSONDocument() {
            var tests = GetTests();
            List<string> jsonInfo = new List<string>();

            using (StreamWriter jsonFile = new StreamWriter(DataDirectory + "techniqueinfo.js")) {
                string total = "";
                foreach (var technique in AllTechniques) {

                    var attemptsPush = tests.SelectMany(x => x.Attempts[technique].ToList()).Where(x => x.Direction == GestureDirection.Push).ToList();
                    var attemptsPull = tests.SelectMany(x => x.Attempts[technique].ToList()).Where(x => x.Direction == GestureDirection.Pull).ToList();

                    var aPushS = new TechniqueInfo(attemptsPush).ToJson();
                    var aPullS = new TechniqueInfo(attemptsPull).ToJson();

                    total += $"\"{technique}\": {{ \n \"Push\": {aPushS},  \n \"Pull\": {aPullS} }},\n";

                }


                jsonFile.WriteLine("var data = {\n" + total.Remove(total.Length - 2) + "\n}");
            }
        }
    }
}
