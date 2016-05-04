using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Net.NetworkInformation;

using Spss;
using System.Threading.Tasks;

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
        

        public static void GenerateSPSSDocument(DataSource source) {

            var tests = AttemptRepository.GetTests(source); 
            string fPath = $"{source}data.sav";
            GenerateSPSSDocument(tests, fPath);
        }

        public static void GenerateSPSSDocument(List<Test> tests, string fileName = "ALLdata.sav") {
            
            fileName = DataGenerator.DataDirectory + fileName;

            if (File.Exists(fileName)) {
                File.Delete(fileName);
            }

            using (SpssDataDocument doc = SpssDataDocument.Create(fileName)) {
                CreateMetaData(doc);
                foreach (var test in tests) {
                    ParseTest(doc, test);
                }
            }

        }

        public static void CreateMetaData(SpssDataDocument doc, SpssFormat format = SpssFormat.Long) {

            SpssNumericVariable vID = new SpssNumericVariable();
            vID.Name = "UserID";
            vID.Label = "The user's ID";
            vID.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
            doc.Variables.Add(vID);


            if(format == SpssFormat.Long)
            {

                SpssNumericVariable attemptNumber = new SpssNumericVariable();
                attemptNumber.Name = $"AttemptNumber";
                attemptNumber.Label = $"The continuous number of this attempt";
                attemptNumber.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
                doc.Variables.Add(attemptNumber);

                SpssNumericVariable time = new SpssNumericVariable();
                time.Name = $"Efficiency";
                time.Label = $"Time taken in seconds for the attempt";
                time.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
                doc.Variables.Add(time);

                SpssStringVariable hit = new SpssStringVariable();
                hit.Name = $"Effectiveness";
                hit.Label = $"Whether the user hit the target or not";
                hit.ValueLabels.Add("Miss", "Miss");
                hit.ValueLabels.Add("Hit", "Hit");
                hit.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_NOM;
                doc.Variables.Add(hit);

                SpssNumericVariable accuracy = new SpssNumericVariable();
                accuracy.Name = $"Accuracy";
                accuracy.Label = $"Distance in pixels from target";
                accuracy.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_RAT;
                doc.Variables.Add(accuracy);

                SpssStringVariable gridSize = new SpssStringVariable();
                gridSize.Name = $"TargetSize";
                gridSize.Label = $"Target (grid) size for attempt";
                gridSize.ValueLabels.Add("Small", "Small");
                gridSize.ValueLabels.Add("Large", "Large");
                gridSize.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_NOM;
                doc.Variables.Add(gridSize);

                SpssStringVariable direction = new SpssStringVariable();
                direction.Name = $"Direction";
                direction.Label = $"Direction for attempt";
                direction.ValueLabels.Add("Push", "Push");
                direction.ValueLabels.Add("Pull", "Pull");
                direction.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_NOM;
                doc.Variables.Add(direction);


                SpssStringVariable technique = new SpssStringVariable();
                technique.Name = $"Technique";
                technique.Label = $"The technique used for the attempt";
                technique.ValueLabels.Add("Pinch", "Pinch");
                technique.ValueLabels.Add("Swipe", "Swipe");
                technique.ValueLabels.Add("Throw", "Throw");
                technique.ValueLabels.Add("Tilt", "Tilt");
                technique.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_NOM;
                doc.Variables.Add(technique);

                SpssStringVariable experiment = new SpssStringVariable();
                experiment.Name = $"Experiment";
                experiment.Label = $"The experiment in which the attempt was conducted in";
                // Target, Field, Old, Accuracy
                experiment.ValueLabels.Add("Target", "Target");
                experiment.ValueLabels.Add("Field", "Field");
                experiment.ValueLabels.Add("Old", "Old");
                experiment.ValueLabels.Add("Accuracy", "Accuracy");
                experiment.MeasurementLevel = MeasurementLevelCode.SPSS_MLVL_NOM;
                doc.Variables.Add(experiment);

            }

            doc.CommitDictionary();
        }

        public static void ParseTest(SpssDataDocument doc, Test test, SpssFormat format = SpssFormat.Long) {
            int id = int.Parse(test.ID);
            //int overallAttempt = 0;

            if (format == SpssFormat.Long)
            {

                //UserID, AttemptNumber, Efficiency, Effectiveness, Accuracy, TargetSize, Direction, Technique, Experiment
                foreach (var type in AllTechniques) {

                    foreach(var attempt in test.Attempts[type]) {
                        SpssCase gestureAttempts = doc.Cases.New();
                        gestureAttempts[$"UserID"] = id;
                        gestureAttempts[$"AttemptNumber"] = attempt.AttemptNumber;
                        gestureAttempts[$"Efficiency"] = attempt.Time.TotalSeconds;
                        gestureAttempts[$"Effectiveness"] = attempt.Hit ? "Hit" : "Miss";
                        gestureAttempts[$"Accuracy"] = MathHelper.GetDistance(attempt);
                        gestureAttempts[$"TargetSize"] = attempt.Size.ToString().UppercaseFirst();
                        gestureAttempts[$"Direction"] = attempt.Direction.ToString().UppercaseFirst();
                        gestureAttempts[$"Technique"] = attempt.Type.ToString().UppercaseFirst();
                        gestureAttempts[$"Experiment"] = attempt.Source.ToString().UppercaseFirst();
                        gestureAttempts.Commit();

                    }
                }
            }
        }

        public static SpssCase AddTechniqueData(SpssCase gestureAttempt, GestureType type, Attempt attempt) {

            gestureAttempt[$"{type}Efficiency"] = attempt.Time.TotalSeconds;
            gestureAttempt[$"{type}Effectiveness"] = attempt.Hit;
            gestureAttempt[$"{type}Accuracy"] = MathHelper.GetDistance(attempt);
            gestureAttempt[$"{type}TargetSize"] = attempt.Size;
            gestureAttempt[$"{type}Direction"] = attempt.Direction;

            return gestureAttempt;
        }

        public static SpssCase AddTechniqueDataLong(SpssCase gestureAttempt, GestureType type, Attempt attempt)
        {
            gestureAttempt[$"Efficiency"] = attempt.Time.TotalSeconds;
            gestureAttempt[$"Effectiveness"] = attempt.Hit;
            gestureAttempt[$"Accuracy"] = MathHelper.GetDistance(attempt);
            gestureAttempt[$"TargetSize"] = attempt.Size;
            gestureAttempt[$"Direction"] = attempt.Direction;
            gestureAttempt[$"AttemptNumber"] = attempt.AttemptNumber;

            return gestureAttempt;
        }


    }
}
