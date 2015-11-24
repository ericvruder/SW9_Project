using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SW9_Project;

namespace DataParser {
    class Program {
        private static string directory = "..\\..\\..\\Testlog/";
        static void Main(string[] args) {

            string[] files = Directory.GetFiles(directory, "*.test");
            List<Test> tests = new List<Test>();
            foreach(var s in files) {
                tests.Add(new Test(s));
                tests.Last().GenerateHTML();
            }
        }
    }
}
