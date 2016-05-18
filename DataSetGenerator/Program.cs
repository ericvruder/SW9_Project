using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace DataSetGenerator {
    class Program {
        static void Main(string[] args)
        {
            DataSource ds = DataSource.Field;
            //AttemptRepository.RemoveTests(ds);

            //List<Test> tests = new List<Test>();

            //for (int i = 1; i < 25; i++)
            //{
            //    tests.Add(new Test(i,ds));
            //}
            //AttemptRepository.SaveTestsToDatabase(tests);
            Test test = new Test(26, ds);
            AttemptRepository.SaveTestToDatabase(test);

            while (AttemptRepository.SaveStatus == DatabaseSaveStatus.Saving)
            {

            }

            Console.WriteLine("save status = " + AttemptRepository.SaveStatus);
            Console.Read();
        }
    }
}
 