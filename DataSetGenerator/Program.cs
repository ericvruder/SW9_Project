using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    class Program {
        static void Main(string[] args) {
            DataGenerator.GetAllTargetData();
            DataGenerator.GetAllUserData();
            DataGenerator.GetUserTwoWayData();
            DataGenerator.GetTargetTwoWayData();
            //DataGenerator.GetStartPercentages();
            //DataGenerator.GetWrongTargetTests();
            //DataGenerator.VerifyTests();
        }
    }
}
