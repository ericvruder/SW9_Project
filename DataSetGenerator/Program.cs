using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    class Program {
        static void Main(string[] args) {

            
            var attempts = AttemptRepository.GetAttempts(DataSource.Target).Where(x => x.Type == GestureType.Swipe).ToList();
            var lAttempts = attempts.Where(x => x.Size == GridSize.Large);
             DataVisualizer.DrawHeatMap(lAttempts.ToList(), GridSize.Large, "heatmap.png");
        }
    }
}
