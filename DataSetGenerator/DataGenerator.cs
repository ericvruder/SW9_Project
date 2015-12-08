using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;


namespace DataSetGenerator {

    static class DataGenerator {

        static string Directory { get { return ".\\..\\..\\..\\Testlog/"; } }

        public static void GetUserInfoGridSize() {
            StreamReader sr = new StreamReader(Directory + "1.test");
        }
    }
}
