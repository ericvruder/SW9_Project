using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project {
    public class Target {
        public int X { get; set; }
        public int Y { get; set; }
        public GridSize Size { get; set; }
        public Target(int x, int y, GridSize size) {
            X = x;
            Y = y;
            Size = size;
        }

        static List<Queue<Target>> TargetSequences;

        public static Queue<Target> GetTargetSequence(int sequenceNumber) {
            if(TargetSequences == null) {
                TargetSequences = new List<Queue<Target>>();
                TargetSequences.Add(LoadSequence(1));
                
            }
            return TargetSequences.ElementAt(0);
        }

        static Queue<Target> LoadSequence(int sequenceNumber) {
            Queue<Target> targets = new Queue<Target>();
            using (StreamReader sr = new StreamReader("sequences/" + sequenceNumber + "_sequence.txt")) {
                string line = "";
                while((line = sr.ReadLine()) != null) { 
                    if(line == "") { break; }
                    string[] targetInfo = line.Split(',');
                    int x = Int32.Parse(targetInfo[0].Trim());
                    int y = Int32.Parse(targetInfo[1].Trim());
                    GridSize size = targetInfo[2].Trim() == "L" ? GridSize.Large : GridSize.Small;
                    targets.Enqueue(new Target(x, y, size));
                }
            }
            return targets;
        }
    }
}
