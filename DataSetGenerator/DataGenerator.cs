using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net.NetworkInformation;

using Spss;
using System.Threading.Tasks;

namespace DataSetGenerator {

    public static class DataGenerator {

        public static List<GestureType> AllTechniques = new List<GestureType> { GestureType.Pinch, GestureType.Swipe, GestureType.Throw, GestureType.Tilt };
        public static List<GestureDirection> AllDirections = new List<GestureDirection> { GestureDirection.Push, GestureDirection.Pull };

        static private List<string> TargetPracticeComputers = new List<string>() { "5CF9DD74A984", "A44E31B190E4" , "485D60CA70DA" };

        public static string TestFileDirectory {
            get {
                if (OldData) { return ".\\..\\..\\..\\Testlog.SW9/"; }
                else {
                    if (TargetPractice) {
                        if (!Directory.Exists(".\\..\\..\\..\\TargetTestlog/")) {
                            Directory.CreateDirectory(".\\..\\..\\..\\TargetTestlog/");
                        }
                        return ".\\..\\..\\..\\TargetTestlog/";
                    }
                    else {
                        if (!Directory.Exists(".\\..\\..\\..\\FieldTestlog/")) {
                            Directory.CreateDirectory(".\\..\\..\\..\\FieldTestlog/");
                        }
                        return ".\\..\\..\\..\\FieldTestlog/";
                    }
                }
            }
        }

        public static bool TargetPracticeComputer() {
            var mac = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();
            TargetPractice = TargetPracticeComputers.Contains(mac);
            return TargetPracticeComputers.Contains(mac);
        }


        private static AttemptContext database;
        public static AttemptContext Database { get {
                if(database == null) {
                    database = new AttemptContext();
                }
                return database;
            }
        }

        private static OldAttemptContext oldDatabase;
        public static OldAttemptContext OldDatabase
        {
            get
            {
                if (oldDatabase == null) {
                    oldDatabase = new OldAttemptContext();
                }
                return oldDatabase;
            }
        }

        public static bool OldData { get; set; }
        public static bool TargetPractice { get; set; }

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

        public static DatabaseSaveStatus SaveStatus { get; set; }

        public static void SaveOldTestToDatabase() {
            bool t = OldData;
            OldData = true;
            List<Test> tests = GetTests();

            Task.Factory.StartNew(() => {
                foreach(var test in tests) {
                    try {
                        Console.WriteLine($"Searching for {test.ID} in database...");
                        var testFound = Database.Attempts.Where(z => z.ID == test.ID).Count() > 0;

                        if (testFound) {
                            Console.WriteLine($"Test ID {test.ID} already exists in database");
                            continue;
                        }
                        else {
                            Console.WriteLine($"Not found, saving {test.ID} to database...");
                        }
                        foreach (var technique in AllTechniques) {
                            OldDatabase.Attempts.AddRange(test.Attempts[technique]);
                        }

                        Database.SaveChanges();
                        Console.WriteLine($"Successfully saved test number {test.ID} to database");
                    } catch (Exception e) {
                        Console.WriteLine("Message: " + e.Message);
                    }
                } 
            });

            OldData = t;
        }

        public static void SaveTestToDatabase(Test test) {
            Task.Factory.StartNew(() => {
                if (TargetPractice) {
                    SaveStatus = DatabaseSaveStatus.Saving;
                    int count = 0;
                    bool success = false;
                    try {
                        var testFound = Database.Attempts.Where(z => z.ID == test.ID).Count() > 0;

                        if (testFound) {
                            Console.WriteLine($"Test ID {test.ID} already exists in database");
                            SaveStatus = DatabaseSaveStatus.Failed;
                            return;
                        }
                        foreach (var technique in AllTechniques) {
                            Database.Attempts.AddRange(test.Attempts[technique]);
                        }

                        Database.SaveChanges();
                        success = true;
                        Console.WriteLine($"Successfully saved test number {test.ID} to database");
                    }
                    catch (Exception e) {
                        Console.WriteLine("Failed saving to database, trying again. Try number: " + ++count);
                        Console.WriteLine("Message: " + e.Message);
                    }
                    SaveStatus = success ? DatabaseSaveStatus.Success : DatabaseSaveStatus.Failed;
                }
                else {
                    Console.WriteLine("Database saves only available to Target Practice");
                }
            });
        }

        public static List<Test> GetTests() {
            List<Test> tests = new List<Test>();
            string[] files = Directory.GetFiles(TestFileDirectory, "*.test");
            foreach (var file in files) {
                string id = file.Split('/').Last().Split('.')[0];
                Test test = null;
                if (OldData) {
                    test = new Test(file, OldData);
                    FixTest(test);
                }
                else {
                    test = new Test(file);
                }
                tests.Add(test);
            }
            return tests;
        }

        static void FixTest(Test test) {
            switch (test.ID) {
                case "1":
                    test.Attempts[GestureType.Tilt][14].Time = TimeSpan.FromSeconds(6);
                    test.Attempts[GestureType.Throw][4].Time = TimeSpan.FromSeconds(8);
                    test.Attempts[GestureType.Throw][4].Size = GridSize.Small;
                    test.Attempts[GestureType.Swipe][5].Time = TimeSpan.FromSeconds(6);
                    test.Attempts[GestureType.Swipe][5].Size = GridSize.Large;
                    test.Attempts[GestureType.Swipe][11].Time = TimeSpan.FromSeconds(6);
                    test.Attempts[GestureType.Swipe][11].Size = GridSize.Large;
                    test.Attempts[GestureType.Swipe][13].Time = TimeSpan.FromSeconds(6);
                    test.Attempts[GestureType.Swipe][13].Size = GridSize.Large;
                    break;
                case "2":
                    test.Attempts[GestureType.Swipe][1].Time = TimeSpan.FromSeconds(6);
                    test.Attempts[GestureType.Swipe][13].Time = TimeSpan.FromSeconds(6);
                    break;
                case "4":
                    test.Attempts[GestureType.Throw][17].Time = TimeSpan.FromSeconds(7);
                    test.Attempts[GestureType.Throw][17].Size = GridSize.Large;
                    test.Attempts[GestureType.Tilt][12].Time = TimeSpan.FromSeconds(5);
                    test.Attempts[GestureType.Tilt][14].Time = TimeSpan.FromSeconds(6);
                    test.Attempts[GestureType.Tilt][14].Size = GridSize.Small;
                    break;
                case "5":
                    test.Attempts[GestureType.Swipe][14].Time = TimeSpan.FromSeconds(4);
                    break;
                case "8":
                    test.Attempts[GestureType.Throw][4].Time = TimeSpan.FromSeconds(8);
                    test.Attempts[GestureType.Throw][4].Size = GridSize.Small;
                    break;
                default:
                    break;
            }
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
                var distances = MathHelper.GetXYDistance(attempt);
                var distance = MathHelper.DistanceToTargetCell(attempt);
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
            gestureAttempt[$"{type}Accuracy"] = MathHelper.DistanceToTargetCell(attempt);
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

                    if (attemptsPull.Count != 0) {
                        var aPushS = new TechniqueInfo(attemptsPush).ToJson();
                        var aPullS = new TechniqueInfo(attemptsPull).ToJson();

                        total += $"\"{technique}\": {{ \n \"Push\": {aPushS},  \n \"Pull\": {aPullS} }},\n";

                    } else { 
                        var aPushS = new TechniqueInfo(attemptsPush).ToJson();

                        total += $"\"{technique}\": {{ \n \"Push\": {aPushS} }},\n";
                    }

                }


                jsonFile.WriteLine("var data = {\n" + total.Remove(total.Length - 2) + "\n}");
            }
        }
    }
}
