using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser {

    // 0   1  2     3        4          5           6                      7                  8 
    //[15:59:47]: Target: Hit Shape: Correct TC: (07,02) CC: (07, 02) Pointer position: (1054,1,384,9).
    class Attempt {

        public TimeSpan Time { get; }
        public bool Hit { get; }
        public bool Shape { get; }
        public Point TargetCell { get; }
        public Point CurrentCell { get; }
        public Point Pointer { get; }

        public Attempt(string attemptLine) {

            string[] para = attemptLine.Trim().Split('[', ']')[1].Split(':');
            Time = new TimeSpan(Int32.Parse(para[0]), Int32.Parse(para[1]), Int32.Parse(para[2]));
            string[] info = attemptLine.Split(':');
            string[] temp = info[4].Split(' ');
            Hit = info[4].Split(' ')[1] == "Hit";
            Shape = info[5].Split(' ')[1] == "Correct";
            TargetCell = GetPoint(info[6]);
            CurrentCell = GetPoint(info[7]);
            Pointer = GetPoint(info[8]);
            
        }

        private Point GetPoint(string segment) {
            string para = segment.Trim().Split('(', ')')[1];
            double x = 0;
            double y = 0;
            if (para.Count(t => t == ',') == 3) {
                string temp = para.Split(',')[0] + "." + para.Split(',')[1];
                x = Double.Parse(temp);
                temp = para.Split(',')[2] + "." + para.Split(',')[3];
                y = Double.Parse(temp);
            }
            else {
                x = Double.Parse(para.Split(',')[0].Replace(',', '.'));
                y = Double.Parse(para.Split(',')[1].Replace(',', '.'));

            }
            return new Point(x, y);
        }
    }
}
