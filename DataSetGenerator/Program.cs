﻿using System;
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
            DataGenerator.GetInvalidCounts(DataSource.Target);
            DataGenerator.GetInvalidCounts(DataSource.Accuracy);
            Console.Read();

        }
    }
}
 