using System;
using System.Collections.Generic;
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
                TargetSequences.Add(FirstSequence());
                TargetSequences.Add(SecondSequence());
                TargetSequences.Add(ThirdSequence());
                TargetSequences.Add(FourthSequence());
                TargetSequences.Add(FifthSequence());
                TargetSequences.Add(SixthSequence());
                TargetSequences.Add(SeventhSequence());
                TargetSequences.Add(EigthSequence());
                
            }
            return TargetSequences.ElementAt(0);
        }

        static Queue<Target> FirstSequence() {
            Queue<Target> list = new Queue<Target>();
            list.Enqueue(new Target(6, 4, GridSize.Large));
            list.Enqueue(new Target(4, 2, GridSize.Large));
            list.Enqueue(new Target(16, 7, GridSize.Small));
            list.Enqueue(new Target(2, 1, GridSize.Large));
            list.Enqueue(new Target(9, 3, GridSize.Small));
            list.Enqueue(new Target(1, 2, GridSize.Small));
            list.Enqueue(new Target(5, 8, GridSize.Small));
            list.Enqueue(new Target(8, 3, GridSize.Large));
            list.Enqueue(new Target(4, 9, GridSize.Small));
            list.Enqueue(new Target(6, 4, GridSize.Large));
            list.Enqueue(new Target(2, 1, GridSize.Large));
            list.Enqueue(new Target(17, 5, GridSize.Small));
            return list;
        }
        static Queue<Target> SecondSequence() {
            Queue<Target> list = new Queue<Target>();
            list.Enqueue(new Target(1, 2, GridSize.Large));
            return list;
        }
        static Queue<Target> ThirdSequence() {
            Queue<Target> list = new Queue<Target>();
            list.Enqueue(new Target(1, 2, GridSize.Large));
            return list;
        }
        static Queue<Target> FourthSequence() {
            Queue<Target> list = new Queue<Target>();
            list.Enqueue(new Target(1, 2, GridSize.Large));
            return list;
        }
        static Queue<Target> FifthSequence() {
            Queue<Target> list = new Queue<Target>();
            list.Enqueue(new Target(1, 2, GridSize.Large));
            return list;
        }
        static Queue<Target> SixthSequence() {
            Queue<Target> list = new Queue<Target>();
            list.Enqueue(new Target(1, 2, GridSize.Large));
            return list;
        }
        static Queue<Target> SeventhSequence() {
            Queue<Target> list = new Queue<Target>();
            list.Enqueue(new Target(1, 2, GridSize.Large));
            return list;
        }
        static Queue<Target> EigthSequence() {
            Queue<Target> list = new Queue<Target>();
            list.Enqueue(new Target(1, 2, GridSize.Large));
            return list;
        }
    }
}
