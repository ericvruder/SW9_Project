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

            //List<Validity> validities = new List<Validity>();

            //string[] files = System.IO.Directory.GetFiles(DataGenerator.DataDirectory, "*.invalid");
            //foreach (var file in files)
            //{
            //    var filename = Path.GetFileNameWithoutExtension(file);
            //    StreamReader sr = new StreamReader(file);
            //    using (sr)
            //    {
            //        string line = "";
            //        while ((line = sr.ReadLine()) != null)
            //        {
            //            Validity invalid = new Validity(filename, line);
            //            validities.Add(invalid);
            //        }
            //    }
            //}

            //    var attempts = AttemptRepository.GetAttempts(DataSource.Target);
            //    var lAttempts = attempts.Where(x => x.Size == GridSize.Small);
            //    DataVisualizer.DrawHeatMap(lAttempts.ToList(), GridSize.Small, "heatmap.png");
            //
        }
    }
}
