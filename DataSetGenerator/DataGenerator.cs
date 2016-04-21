using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net.NetworkInformation;

using Spss;
using System.Threading.Tasks;
using CsvHelper;

namespace DataSetGenerator {

    public static class DataGenerator {

        public static List<GestureType> AllTechniques = new List<GestureType> { GestureType.Pinch, GestureType.Swipe, GestureType.Throw, GestureType.Tilt };
        public static List<GestureDirection> AllDirections = new List<GestureDirection> { GestureDirection.Push, GestureDirection.Pull };
        public static List<GridSize> AllSizes = new List<GridSize> { GridSize.Large, GridSize.Small };

        static private List<string> TargetPracticeComputers = new List<string>() { "5CF9DD74A984", "A44E31B190E4" , "485D60CA70DA" };

        public static string TestFileDirectory(DataSource source) {
            string directory = "";
            switch (source) {
                case DataSource.Old: directory = ".\\..\\..\\..\\Testlog.SW9/"; break;
                case DataSource.Field: directory = ".\\..\\..\\..\\FieldTestlog/"; break;
                case DataSource.Target: directory = ".\\..\\..\\..\\TargetTestlog/"; break;
                case DataSource.Accuracy: directory = ".\\..\\..\\..\\AccuracyTestlog/"; break;
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

        public static void GenerateTestFileFromShorthand(string path) {
            string largeGrid = "Changed grid size. Grid height: 5 Grid width: 10 Cell height: 122.8 Cell width: 121.4";
            string smallGrid = "Changed grid size. Grid height: 10 Grid width: 20 Cell height: 61.4 Cell width: 60.7";
            GridSize size = GridSize.Large;
            using (StreamReader sr = new StreamReader(path))
            using (StreamWriter sw = new StreamWriter(path + ".test")) {
                string line = "", writeLine = "";
                DateTime time = DateTime.Now;
                while ((line = sr.ReadLine()) != null) {
                    if (line == "") continue;
                    string[] words = line.Split(' ');
                    int seconds = (Int32.Parse(words[0].Split(':')[0]) * 60) + (Int32.Parse(words[0].Split(':')[1]));
                    writeLine = $"[{(time + TimeSpan.FromSeconds(seconds)).ToString("HH:mm:ss")}]: ";
                    string extra = "";
                    switch (words[1]) {
                        case "practice": extra = $"Started new gesture practice. Type: {words[3]} Direction: {words[2]}"; break;
                        case "start": extra = $"Started new gesture test. Type: {words[3]} Direction: {words[2]}"; break;
                        case "cg": extra = words[2] == "s" ? smallGrid : largeGrid; size = words[2] == "s" ? GridSize.Small : GridSize.Large; break;
                        case "Hit": case "Miss": extra = GetLine(words, size);  break;
                    }
                    if(extra == "") {
                        Console.WriteLine(line);
                    }
                    sw.WriteLine(writeLine + extra);

                }
            }
        }

        private static string GetLine(string[] words, GridSize size) {

            //29:51 hit 2,4
            //29:56 miss 9,0 8,0
            string tc = $"({ words[2].Split(',')[0]},{ words[2].Split(',')[1]})";
            string cc = "", pointer = "";
            int cSize = size == GridSize.Large ? 121 : 61;
            int index = 2;
            if (words[1] == "Hit") {
                cc = tc;
            }
            else {
                cc = $"({words[3]})";
                index = 3;
            }

            int x = Int32.Parse(words[index].Split(',')[0]);
            int y = Int32.Parse(words[index].Split(',')[1]);
            int px = x * cSize + (cSize / 2);
            int py = y * cSize + (cSize / 2);
            pointer = $"({px},{py})";
            //Target: Hit  Shape: Correct TC: (01,02) CC: (01, 02) JL: Long Pointer position: (136.0,310.5).
            return $"Target: {words[1]} Shape: Correct TC: {tc} CC: {cc} JL: Long Pointer position: {pointer}";

        }

        
        public static void GenerateCSVDocument(DataSource source)
        {
            var attempts = AttemptRepository.GetAttempts(source);
            using (StreamWriter sr = new StreamWriter(DataDirectory + source + ".csv"))
            using (CsvWriter wr = new CsvWriter(sr))
            {
                wr.WriteHeader(typeof(Attempt));
                
                wr.WriteRecords(attempts);
            }
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
                        Console.WriteLine("Test ID: " + test.ID + " FAILED on" + gesture + " " +listSmall.Count() + " "+  listLarge.Count());
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
               
                foreach (var type in AllTechniques) {
                    for (int i = 0; i < test.Attempts[type].Count; i++) {
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

        public static void ReadSpssFile()
        {


            using (var doc = SpssDataDocument.Open("t.sav", SpssFileAccess.Append))
            {
                SpssNumericVariable accuracy = new SpssNumericVariable
                {
                    Name = $"Accuracy",
                    Label = $"Distance in pixels from target",
                    MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT
                };
                doc.Variables.Add(accuracy);

                SpssStringVariable var = (SpssStringVariable)doc.Variables["someLabel"];

                SpssCase row = doc.Cases.New();
                row[$"Accuracy"] = new string('a', var.Length + 1);
            }
        }

        public static void InvalidateOldTests() {
            var removed = GetAttemptsRemoved();
            Dictionary<string, List<int>> something = new Dictionary<string, List<int>>();

            something.Add("1", new List<int>() { 69, 5, 42, 48, 50 });
            something.Add("2", new List<int>() { 56, 68 });
            something.Add("4", new List<int>() { 36, 67, 69 });
            something.Add("5", new List<int>() { 15 });
            something.Add("8", new List<int>() { 41 });

            foreach (var s in something) {
                if (removed.ContainsKey(s.Key)) {
                    removed[s.Key] = removed[s.Key].Union(s.Value).ToList();
                }
            }
            AttemptRepository.InvalidateAttempts(removed, DataSource.Old);
        }

        public static Dictionary<string,List<int>> GetAttemptsRemoved()
        {
            Dictionary<string,List<int>> dictionary = new Dictionary<string, List<int>>();
            List<string> list = new List<string>();
            var reader = new StreamReader(File.OpenRead(DataDirectory + "outliersRemoved.csv"));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                list.Add(values[0] + "," + values[1]);
                
            }

            for (int i = 1; i <= 53; i++)
            {
                List<int> removed = new List<int>();
                for (int j = 1; j < 72; j++)
                {
                    if (!list.Contains(i + "," + j))
                    {
                        removed.Add(j);
                    }
                }
                dictionary.Add(i.ToString(), removed);
            }

            return dictionary;
        }


    }
}
