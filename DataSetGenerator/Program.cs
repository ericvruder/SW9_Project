﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    class Program {
        static void Main(string[] args) {

            var t = AttemptRepository.GetAttempt("24", 59, DataSource.Old);
            //var mylist = AttemptRepository.GetMissedAttempts();
            //Validity.ValidateAttempts();

            //Console.ReadLine();
            //    var attempts = AttemptRepository.GetAttempts(DataSource.Target);
            //    var lAttempts = attempts.Where(x => x.Size == GridSize.Small);
            //    DataVisualizer.DrawHeatMap(lAttempts.ToList(), GridSize.Small, "heatmap.png");
            //
        }
    }
}
