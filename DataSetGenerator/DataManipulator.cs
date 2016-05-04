using Spss;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    class DataManipulator {


        public static void ReadSpssFile() {


            using (var doc = SpssDataDocument.Open("t.sav", SpssFileAccess.Append)) {
                SpssNumericVariable accuracy = new SpssNumericVariable {
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
            Dictionary<string, List<int>> tests = new Dictionary<string, List<int>>();

            tests.Add("1", new List<int>() { 69, 5, 42, 48, 50 });
            tests.Add("2", new List<int>() { 56, 68 });
            tests.Add("4", new List<int>() { 36, 67, 69 });
            tests.Add("5", new List<int>() { 15 });
            tests.Add("8", new List<int>() { 41 });

            foreach (var s in tests) {
                if (removed.ContainsKey(s.Key)) {
                    removed[s.Key] = removed[s.Key].Union(s.Value).ToList();
                }
            }
            AttemptRepository.InvalidateAttempts(removed, DataSource.Old);
        }

        public static void FixAccuracyDocument(string path) {
            using (StreamReader sr = new StreamReader(path))
            using (StreamWriter sw = new StreamWriter(path + "-with-accuracy.csv")) {
                string line = sr.ReadLine();
                if (line == "") line = sr.ReadLine();
                sw.WriteLine(line + "; Accuracy");
                while ((line = sr.ReadLine()) != null) {
                    string[] lines = line.Split(';');
                    GridSize size = lines[2] == "1" ? GridSize.Large : GridSize.Small;
                    bool hit = lines[4] == "1";
                    float x = float.Parse(lines[6]), y = float.Parse(lines[7]);
                    Point t = new Point(x, y);
                    x = float.Parse(lines[10]); y = float.Parse(lines[11]);
                    Point p = new Point(x, y);
                    var accuracy = MathHelper.GetDistance(new Attempt(hit, size, t, p));
                    line += ";" + accuracy;
                    sw.WriteLine(line);
                }
            }
        }

        public static Dictionary<string, List<int>> GetAttemptsRemoved() {
            Dictionary<string, List<int>> dictionary = new Dictionary<string, List<int>>();
            List<string> list = new List<string>();
            var reader = new StreamReader(File.OpenRead(DataGenerator.DataDirectory + "outliersRemoved.csv"));
            while (!reader.EndOfStream) {
                var line = reader.ReadLine();
                var values = line.Split(',');

                list.Add(values[0] + "," + values[1]);

            }

            for (int i = 1; i <= 53; i++) {
                List<int> removed = new List<int>();
                for (int j = 1; j < 72; j++) {
                    if (!list.Contains(i + "," + j)) {
                        removed.Add(j);
                    }
                }
                dictionary.Add(i.ToString(), removed);
            }

            return dictionary;
        }
        
        public static void FixExtensionsInvalidity(DataSource source) {
            string[] files = Directory.GetFiles(DataGenerator.TestFileDirectory(source) + "/invalidity", "*txt");
            foreach (var file in files) {
                string n = DataGenerator.TestFileDirectory(source) + "/invalidity/" + file.Split('\\').Last().Split('.')[0] + ".invalidity";
                File.Move(file, n);
            }
        }

        /// <summary>
        /// Parses the .invalid files to a List
        /// </summary>
        /// <returns>A list of validities with id, type, direction, number of invalid attempts, and time errors</returns>
        private static List<Validity> ValidityFilesList(DataSource source) {
            List<Validity> results = new List<Validity>();

            string[] files = Directory.GetFiles(DataGenerator.TestFileDirectory(source), "invalidity/*.invalidity");
            foreach (var file in files) {
                var filename = Path.GetFileNameWithoutExtension(file);
                StreamReader sr = new StreamReader(file);
                using (sr) {
                    string line = "";
                    while ((line = sr.ReadLine()) != null) {
                        Validity invalid = new Validity(filename, line);
                        results.Add(invalid);
                    }
                }
            }
            
            return results;

        }

        /// <summary>
        /// Sets the correct validity value for each attempt
        /// </summary>
        /// <param name="source">where the data comes from</param>
        public static void ValidateAttempts(DataSource source) {
            //get number of invalid attempts from validity file
            var validities = ValidityFilesList(source);


            //Get all miss attempts
            var missedAttempts = AttemptRepository.GetMissedAttempts(source);

            var changedAttempts = new List<Attempt>();
            foreach (var validity in validities) {
                var tattempt = missedAttempts.Where(x => x.ID == validity.ParticipantID.ToString() && x.Type == validity.Type && x.Direction == validity.Direction).ToList();
                tattempt.OrderByDescending(x => MathHelper.GetDistance(x));
                var nAttempts = tattempt.Select(x => MathHelper.GetDistance(x));
                for (int i = 0; i < validity.InvalidAttempts; i++) {
                    tattempt[i].Valid = false;
                    changedAttempts.Add(tattempt[i]);
                }
            }

            AttemptRepository.UpdateAttempt(changedAttempts);

            //Apply false to valid property in attempt class on database for for furthest attempts 
        }

        public static void InvalidateTechniqueAttempts(DataSource source, int userId, GestureType type, GestureDirection direction) {
            var attempts = AttemptRepository.GetAttempts(source, false);
            var modified = from attempt in attempts
                           where    attempt.ID == userId.ToString() &&
                                    attempt.Type == type &&
                                    attempt.Direction == direction &&
                                    attempt.Source == source
                           select attempt;


            foreach(var attempt in modified) {
                attempt.Valid = false;
            }
            AttemptRepository.UpdateAttempt(modified);

        }

        public static void InvalidateAttempts(DataSource source) {

            if (source == DataSource.Target) {
                var attempts = AttemptRepository.GetAttempts(source, false);
                var modified = attempts.Where(x => x.Time >= TimeSpan.FromSeconds(18) && x.Source == source).ToList();
                var invalids = GetInvalidAttempts(source);

                foreach (var inv in invalids) {
                    var attempt = attempts.Where(x => x.ID == inv.Item1.ToString() && x.AttemptNumber == inv.Item2 && x.Source == source).Single();
                    if (!modified.Contains(attempt)) modified.Add(attempt);
                }

                foreach (var attempt in modified) {
                    attempt.Valid = false;
                }

                AttemptRepository.UpdateAttempt(modified);
            }

            else {

                var attempts = AttemptRepository.GetAttempts(source, false);
                var modified = attempts.Where(x => x.Time >= TimeSpan.FromSeconds(23) && x.Source == source).ToList();

                foreach (var attempt in modified) {
                    attempt.Valid = false;
                }

                AttemptRepository.UpdateAttempt(modified);
            }

        }


        public static List<Tuple<int, int>> GetInvalidAttempts(DataSource source) {
            List<Tuple<int, int>> invalids = null;
            if (source == DataSource.Target) {
                invalids = new List<Tuple<int, int>> {
                    new Tuple<int, int>(41,61),
                    new Tuple<int, int>(28, 133),
                    new Tuple<int, int>(22,123),
                    new Tuple<int, int>(19,66),
                    new Tuple<int, int>(19,62),
                    new Tuple<int, int>(16,108)
                };
            }

            return invalids;
        }

    }
}
