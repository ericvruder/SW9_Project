using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    class Program {
        static void Main(string[] args) {

            DataGenerator.TargetPracticeComputer();

            DataGenerator.OldData = true;
            DataGenerator.SaveOldTestToDatabase();

            DataGenerator.VerifyTests();
            DataGenerator.GenerateJSONDocument();
            DataGenerator.CreateSPSSDocument();
            DataGenerator.CreateCSVDocument();
            HitboxDrawer.CreateHitboxes();
        }

    }
}
