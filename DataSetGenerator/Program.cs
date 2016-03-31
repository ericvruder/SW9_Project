using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    class Program {
        static void Main(string[] args) {

            DataGenerator.TargetPracticeComputer();

            //DataGenerator.FixLargeJump();

            var oldTest = DataGenerator.GetTests(DataSource.Old);
            var newTest = DataGenerator.GetTests(DataSource.Target);

            
            //AttemptRepository.SaveOldTestToDatabase();
        }
    }
}
