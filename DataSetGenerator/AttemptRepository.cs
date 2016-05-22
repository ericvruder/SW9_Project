using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.OData.Query;
using Microsoft.Data.OData.Query.SemanticAst;

namespace DataSetGenerator {
    public class AttemptRepository : DbContext {
        public  static DatabaseSaveStatus SaveStatus { get; set; }

        public AttemptRepository() : base("SW9_Project") { }
        public DbSet<Attempt> Attempts { get; set; }

        public static void UpdateAttempt(Attempt attempt)
        {
            UpdateAttempt(new List<Attempt>() {attempt});
        }

        public static void UpdateAttempt(IEnumerable<Attempt> attempts)
        {
            using (var Repository = new AttemptRepository()) {
                foreach (var attempt in attempts) {
                    Repository.Entry(attempt).State = EntityState.Modified;
                }
                Repository.SaveChanges();
            }
        }
        public static void RemoveTests(DataSource source)
        {
            using (var Repository = new AttemptRepository())
            {
                lock (Repository)
                {
                    var attempts = Repository.Attempts
                        .Where(x => x.Source == source);

                    Repository.Attempts.RemoveRange(attempts);
                    Repository.SaveChanges();
                }
            }
        }

        public static void RemoveTests(DataSource source, int from , int to)
        {
            using (var Repository = new AttemptRepository())
            {
                lock (Repository)
                {
                    for (int i = from; i <= to; i++)
                    {
                        var attempts = Repository.Attempts
                            .Where(x => x.Source == source && x.ID == i.ToString());

                        Repository.Attempts.RemoveRange(attempts);
                        Repository.SaveChanges();
                    }
                }
            }
        }

        public static Attempt GetAttempt(string id, int attemptN, DataSource source) {
            using(var repo = new AttemptRepository()) {
                var attempt = repo.Attempts.Where(att => att.ID == id && att.AttemptNumber == attemptN && att.Source == source);
                return attempt.Single();
            }
        }

        public static void InvalidateAttempts(Dictionary<string, List<int>> outliers, DataSource source) {
            using(var Repo = new AttemptRepository()) {
                foreach (var entry in outliers) {
                    var attempts = Repo.Attempts.Where(attempt => attempt.ID == entry.Key && attempt.Source == source);
                    foreach(var aNum in entry.Value) {
                        var attempt = attempts.Where(att => att.AttemptNumber == aNum).Single();
                        attempt.Valid = false;
                        Repo.Entry(attempt).State = EntityState.Modified;
                    }
                }
                Repo.SaveChanges();
            }
        }

        public static void InvalidateAttempts(List<Validity> invalidities) {
            foreach(var validity in invalidities) {
                
            }
        }

        public static void SaveTestsToDatabase(List<Test> tests) {
            

            foreach (var test in tests) {
                SaveTest(test); 
            }
        }

        public static List<Attempt> GetAttempts(DataSource source, bool valid = true) {
            List<Attempt> attempts = null;
            try
            {
                using (var Repository = new AttemptRepository())
                {
                    attempts = Repository.Attempts
                    .Where(x => x.Source == source && (x.Valid == valid || x.Valid == true)).ToList();
                }
            }
            catch (Exception)
            {

                Console.WriteLine("SQL ERROR");
                return null;
            }
            return attempts;
        }

        public static int GetTestCount(DataSource source) {

            int count = 0;
            using (var Repository = new AttemptRepository()) {
                count = (from attempt in Repository.Attempts
                         where attempt.Source == source
                         group attempt by attempt.ID into testsFound
                         select testsFound).Count();
            }
            return count;

        }

        public static Test GetTest(string id, DataSource source, bool valid = true) {
            List<Attempt> attempts = null;
            using (var Repository = new AttemptRepository()) {
                attempts = Repository.Attempts
                    .Where(x => x.Source == source && x.ID == id && (x.Valid == valid || x.Valid == true))
                    .ToList();
            }

            return new Test(attempts);
        }

        public static List<Attempt> GetMissedAttempts(DataSource source, bool valid = true)
        {
            List<Attempt> attempts = null;

            using (var Repository = new AttemptRepository())
            {
                attempts = Repository.Attempts
                    .Where(x => !x.Hit && x.Source == source && (x.Valid == valid || x.Valid == true))
                    .ToList()
                    .OrderByDescending(MathHelper.GetDistance)
                    .ToList();
            }
            return attempts;
        }

        public static List<Test> GetTests(DataSource source, bool valid = true) {

            List<Test> tests = new List<Test>();

            using (var Repository = new AttemptRepository()) {
                var allTests = Repository.Attempts
                    .Where(attempt => attempt.Source == source && (attempt.Valid == valid || attempt.Valid == true))
                    .GroupBy(attempt => attempt.ID, attempt => attempt);

                foreach (var testgrouping in allTests) {
                    tests.Add(new Test(testgrouping.ToList()));
                }
            }
            
            return tests;
        }

        private static void SaveTest(Test test) {
            using (var Repository = new AttemptRepository()) {
                SaveStatus = DatabaseSaveStatus.Saving;
                DataSource source = test.Attempts.First().Value.First().Source;
                bool success = false;
                try {
                    Console.WriteLine($"Searching for {test.ID} in database...");
                    var testFound = Repository.Attempts.Where(z => z.ID == test.ID && z.Source == source).Count() > 0;
                    if (testFound) {
                        Console.WriteLine($"Test ID {test.ID} from {source} data source already exists in database");
                        SaveStatus = DatabaseSaveStatus.Failed;
                        return;
                    }
                    Console.WriteLine($"Test ID {test.ID} not found, saving...");
                    foreach (var technique in DataGenerator.AllTechniques) {
                        Repository.Attempts.AddRange(test.Attempts[technique]);
                    }

                    Repository.SaveChanges();
                    success = true;
                    Console.WriteLine($"Successfully saved test number {test.ID} to database");
                }
                catch (Exception e) {
                    Console.WriteLine("Failed saving to database");
                    Console.WriteLine("Message: " + e.Message);
                }
                SaveStatus = success ? DatabaseSaveStatus.Success : DatabaseSaveStatus.Failed;
            }
        }

        public static void SaveTestToDatabase(Test test) {
            Task.Factory.StartNew(() => {
                SaveTest(test);
            });
        }
    }
}
