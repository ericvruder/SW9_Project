using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    public class AttemptRepository : DbContext {

        static AttemptRepository attemptRepository;
        public  static DatabaseSaveStatus SaveStatus { get; set; }

        static AttemptRepository Repository {
            get {
                if(attemptRepository == null) {
                    attemptRepository = new AttemptRepository();
                }
                return attemptRepository;
            }
        }

        public AttemptRepository() : base("SW9_Project") { }
        public DbSet<Attempt> Attempts { get; set; }

        public static void SaveOldTestToDatabase() {

            List<Test> tests = DataGenerator.GetTests(DataSource.Old);

            foreach (var test in tests) {
                try {
                    Console.WriteLine($"Searching for {test.ID} in database...");
                    var testFound = Repository.Attempts.Where(z => z.ID == test.ID).Count() > 0;

                    if (testFound) {
                        Console.WriteLine($"Test ID {test.ID} already exists in database");
                        continue;
                    }
                    else {
                        Console.WriteLine($"Not found, saving {test.ID} to database...");
                    }
                    foreach (var technique in DataGenerator.AllTechniques) {
                        Repository.Attempts.AddRange(test.Attempts[technique]);
                    }

                    Repository.SaveChanges();
                    Console.WriteLine($"Successfully saved test number {test.ID} to database");
                }
                catch (Exception e) {
                    Console.WriteLine("Message: " + e.Message);
                }
            }
        }

        public static List<Attempt> GetAttempts(DataSource source) {
            return attemptRepository.Attempts.ToList();
        }

        public static List<Test> GetTests(DataSource source) {
            return new List<Test>();
        }

        public static void SaveTestToDatabase(Test test) {
            Task.Factory.StartNew(() => {
                SaveStatus = DatabaseSaveStatus.Saving;
                int count = 0;
                bool success = false;
                try {
                    var testFound = Repository.Attempts.Where(z => z.ID == test.ID).Count() > 0;

                    if (testFound) {
                        Console.WriteLine($"Test ID {test.ID} already exists in database");
                        SaveStatus = DatabaseSaveStatus.Failed;
                        return;
                    }
                    foreach (var technique in DataGenerator.AllTechniques) {
                        Repository.Attempts.AddRange(test.Attempts[technique]);
                    }

                    Repository.SaveChanges();
                    success = true;
                    Console.WriteLine($"Successfully saved test number {test.ID} to database");
                }
                catch (Exception e) {
                    Console.WriteLine("Failed saving to database, trying again. Try number: " + ++count);
                    Console.WriteLine("Message: " + e.Message);
                }
                SaveStatus = success ? DatabaseSaveStatus.Success : DatabaseSaveStatus.Failed;
            });
        }
    }
}
