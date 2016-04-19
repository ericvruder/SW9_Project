using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    class Program {
        static void Main(string[] args)
        {

            
            var attempts = AttemptRepository.GetAttempts(DataSource.Target).Where(x => x.Type == GestureType.Swipe).ToList();
            var lAttempts = attempts.Where(x => x.Size == GridSize.Large);
             DataVisualizer.DrawHeatMap(lAttempts.ToList(), GridSize.Large, "heatmap.png");

            Validity.ValidateAttempts();

            //    var attempts = AttemptRepository.GetAttempts(DataSource.Target);
            //    var lAttempts = attempts.Where(x => x.Size == GridSize.Small);
            //    DataVisualizer.DrawHeatMap(lAttempts.ToList(), GridSize.Small, "heatmap.png");
            //
        }
    }
}
