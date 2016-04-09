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

        public static string TestFileDirectory(DataSource source) {
            string directory = "";
            switch (source) {
                case DataSource.Old: directory = ".\\..\\..\\..\\Testlog.SW9/"; break;
                case DataSource.Field: directory = ".\\..\\..\\..\\FieldTestlog/"; break;
                case DataSource.Target: directory = ".\\..\\..\\..\\TargetTestlog/"; break;
            }
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        public static bool TargetPracticeComputer() {
            var mac = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();
            return TargetPracticeComputers.Contains(mac);
        }

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

        public static List<Test> GetTests(DataSource source) {
            
            List<Test> tests = new List<Test>();
            int count = Directory.GetFiles(TestFileDirectory(source), "*.test").Count();
            for(int i = 1; i <= count; i++) {
                tests.Add(new Test(i, source));
            }
            return tests;
        }

        public static Test GetTest(int test, DataSource source) {
            return new Test(test, source);
        }

        
        
        public static string GenerateCSVDocument(DataSource source, string path) {

            List<int> testing = new List<int>();
            string fPath = $"{path}{source}data.csv";
            using (StreamWriter datawriter = new StreamWriter(fPath)) {
                datawriter.WriteLine("ID PinchLargeTime PinchSmallTime PinchLargeHit PinchSmallHit PinchLargeDist PinchSmallDist PinchLargeXDist PinchSmallXDist PinchLargeYDist PinchSmallYDist" +
                                       " SwipeLargeTime SwipeSmallTime SwipeLargeHit SwipeSmallHit SwipeLargeDist SwipeSmallDist SwipeLargeXDist SwipeSmallXDist SwipeLargeYDist SwipeSmallYDist" +
                                       " ThrowLargeTime ThrowSmallTime ThrowLargeHit ThrowSmallHit ThrowLargeDist ThrowSmallDist ThrowLargeXDist ThrowSmallXDist ThrowLargeYDist ThrowSmallYDist" +
                                       " TiltLargeTime TiltSmallTime TiltLargeHit TiltSmallHit TiltLargeDist TiltSmallDist TiltLargeXDist TiltSmallXDist TiltLargeYDist TiltSmallYDist");
                List<Test> tests = GetTests(source);

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
                            var distances = MathHelper.GetDistances(attempt);
                            sTimes[gesture].Add((int)attempt.Time.TotalSeconds);
                            sHits[gesture].Add(attempt.Hit ? "1" : "0");
                            sDist[gesture].Add(distances.Item1);
                            sxDist[gesture].Add(distances.Item2);
                            syDist[gesture].Add(distances.Item3);
                        }
                        foreach (var attempt in ltList) {
                            var distances = MathHelper.GetDistances(attempt);
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
            return fPath;
        }


        public static void VerifyTests(DataSource source)
        {
            List<Test> tests = GetTests(source);
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


        public static string GenerateSPSSDocument(DataSource source, string path) {

            List<Test> tests = AttemptRepository.GetTests(source);
            string fPath = $"{path}{source}data.sav";
            if (File.Exists(fPath)) {
                File.Delete(fPath);
            }
            using (SpssDataDocument doc = SpssDataDocument.Create(fPath)) {
                CreateMetaData(doc);
                foreach (var test in tests) {
                    ParseTest(doc, test);
                }
            }

            return fPath;

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

        public static void CreateMetaData(SpssDataDocument doc, SpssFormat format = SpssFormat.Long) {

            SpssNumericVariable vID = new SpssNumericVariable();
            vID.Name = "ID";
            vID.Label = "User ID";
            vID.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
            doc.Variables.Add(vID);

            if (format == SpssFormat.Short)
            {
                foreach (var technique in AllTechniques)
                {
                    AddVariableForTechnique(doc, technique);
                }
            }
            else if(format == SpssFormat.Long)
            {
                //SpssNumericVariable overallAttemptNo = new SpssNumericVariable();
                //overallAttemptNo.Name = $"OverallAttemptNo";
                //overallAttemptNo.Label = $"The overall attempt number";
                //overallAttemptNo.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
                //doc.Variables.Add(overallAttemptNo);

                //SpssNumericVariable techniqueAttemptNo = new SpssNumericVariable();
                //techniqueAttemptNo.Name = $"TechniqueAttemptNo";
                //techniqueAttemptNo.Label = $"The attempt number for the technique";
                //techniqueAttemptNo.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
                //doc.Variables.Add(techniqueAttemptNo);

                SpssNumericVariable time = new SpssNumericVariable();
                time.Name = $"Time";
                time.Label = $"Time taken in seconds for the attempt";
                time.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
                doc.Variables.Add(time);

                SpssNumericVariable hit = new SpssNumericVariable();
                hit.Name = $"Hit";
                hit.Label = $"Whether the user hit the target or not";
                hit.ValueLabels.Add(0, "Miss");
                hit.ValueLabels.Add(1, "Hit");
                hit.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_NOM;
                doc.Variables.Add(hit);

                SpssNumericVariable accuracy = new SpssNumericVariable();
                accuracy.Name = $"Accuracy";
                accuracy.Label = $"Distance in pixels from target";
                accuracy.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
                doc.Variables.Add(accuracy);

                SpssNumericVariable gridSize = new SpssNumericVariable();
                gridSize.Name = $"Size";
                gridSize.Label = $"Target (grid) size for attempt";
                gridSize.ValueLabels.Add(0, "Small");
                gridSize.ValueLabels.Add(1, "Large");
                gridSize.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_NOM;
                doc.Variables.Add(gridSize);

                SpssNumericVariable direction = new SpssNumericVariable();
                direction.Name = $"Direction";
                direction.Label = $"Direction for attempt";
                direction.ValueLabels.Add(0, "Push");
                direction.ValueLabels.Add(1, "Pull");
                direction.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_NOM;
                doc.Variables.Add(direction);

                SpssNumericVariable technique = new SpssNumericVariable();
                technique.Name = $"Technique";
                technique.Label = $"The technique used for the attempt";
                technique.ValueLabels.Add(0, "Pinch");
                technique.ValueLabels.Add(1, "Swipe");
                technique.ValueLabels.Add(2, "Throw");
                technique.ValueLabels.Add(3, "Tilt");
                technique.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_NOM;
                doc.Variables.Add(technique);

                SpssNumericVariable attemptNumber = new SpssNumericVariable();
                attemptNumber.Name = $"AttemptNumber";
                attemptNumber.Label = $"The continuous number of this attempt";
                attemptNumber.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_NOM;
                doc.Variables.Add(attemptNumber);
            }

            doc.CommitDictionary();
        }

        public static void ParseTest(SpssDataDocument doc, Test test, SpssFormat format = SpssFormat.Long) {
            int id = int.Parse(test.ID);
            int nAttempts = test.Attempts[GestureType.Pinch].Count;
            //int overallAttempt = 0;

            if (format == SpssFormat.Short)
            {
                // GestureType.Pinch, GestureType.Swipe, GestureType.Throw, GestureType.Tilt 
                for (int i = 0; i < nAttempts; i++) {
                    SpssCase gestureAttempts = doc.Cases.New();
                    gestureAttempts["ID"] = id;

                    foreach (var type in AllTechniques)
                    {
                        gestureAttempts = AddTechniqueData(gestureAttempts, type, test.Attempts[type][i]);
                    }
                    gestureAttempts.Commit();
                }
            }
            else if (format == SpssFormat.Long)
            {
                for (int i = 0; i < test.Attempts[GestureType.Pinch].Count; i++)
                {
                    foreach (var type in AllTechniques)
                    {
                        SpssCase gestureAttempts = doc.Cases.New();
                        gestureAttempts[$"ID"] = id;
                        //gestureAttempts[$"OverallAttemptNo"] = ++overallAttempt;
                        //gestureAttempts[$"TargetAttemptNo"] = targetAttempt;
                        gestureAttempts = AddTechniqueDataLong(gestureAttempts, type, test.Attempts[type][i]);
                        gestureAttempts[$"Technique"] = type;
                        gestureAttempts.Commit();
                    }
                }
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

        public static SpssCase AddTechniqueDataLong(SpssCase gestureAttempt, GestureType type, Attempt attempt)
        {
            gestureAttempt[$"Time"] = attempt.Time.TotalSeconds;
            gestureAttempt[$"Hit"] = attempt.Hit;
            gestureAttempt[$"Accuracy"] = MathHelper.DistanceToTargetCell(attempt);
            gestureAttempt[$"Size"] = attempt.Size;
            gestureAttempt[$"Direction"] = attempt.Direction;
            gestureAttempt[$"AttemptNumber"] = attempt.AttemptNumber;

            return gestureAttempt;
        }
    }
}
