using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    class Program {
        static void Main(string[] args) {
            DataGenerator.VerifyTests();
            DataGenerator.GenerateJSONDocument();
            DataGenerator.CreateSPSSDocument();
            DataGenerator.CreateCSVDocument();
            HitboxDrawer.CreateHitboxes();
        }

    }
}
