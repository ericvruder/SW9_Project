using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    class Program {
        static void Main(string[] args) {
            //DataGenerator.GetStartPercentages();
            DataGenerator.GetAllTechniqueAttempts();
            DataGenerator.GetAllGridSizeAttempts();
            DataGenerator.GetUserTechniqueData();
            DataGenerator.GetUserGridSizeData();
        }
    }
}
