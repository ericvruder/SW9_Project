using System;
using System.Linq;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataSetGenerator {

    public class Attempt {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        public Guid AttemptID { get; set; }
        public string ID { get; set; }
        public TimeSpan Time { get; set; }
        public bool Hit { get; set; }
        public bool Shape { get; set; }
        public Point TargetCell { get; set; }
        public Point CurrentCell { get; set; }
        public Point Pointer { get; set; }
        public GridSize Size { get; set; }
        public GestureDirection Direction { get; set; }
        public GestureType Type { get; set; }
        public DataSource Source { get; set; }
        public int AttemptNumber { get; set; }
        public bool Valid { get; set; }

        public Attempt() { }

        public Attempt(bool hit, GridSize size, Point targetCell, Point pointer) {
            Hit = hit;
            Size = size;
            TargetCell = targetCell;
            Pointer = pointer;
        }

        public Attempt(string id, int attemptNumber, string attemptLine, TimeSpan time, GridSize size, GestureDirection direction, GestureType type, DataSource source) {
            Valid = true;
            Size = size;
            Direction = direction;
            Source = source;
            AttemptNumber = attemptNumber;

            // 0   1  2     3        4          5           6            7                  8                 9
            //[15:59:47]: Target: Hit Shape: Correct TC: (07,02) CC: (07, 02) JL: Short Pointer position: (1054,1,384,9).
            ID = id;
            string[] para = attemptLine.Trim().Split('[', ']')[1].Split(':');
            Time = time;
            string[] info = attemptLine.Split(':');
            Hit = info[4].Split(' ')[1] == "Hit";
            Shape = info[5].Split(' ')[1] == "Correct";
            TargetCell = GetPoint(info[6]);
            CurrentCell = GetPoint(info[7]);
            Pointer = GetPoint(info[9]);
            Type = type;
            
        }

        private Point GetPoint(string segment) {
            string parameter = segment.Trim().Split('(', ')')[1];
            double x = 0;
            double y = 0;
            if (parameter.Count(t => t == ',') == 3) {
                string temp = parameter.Split(',')[0] + "." + parameter.Split(',')[1];
                x = Double.Parse(temp);
                temp = parameter.Split(',')[2] + "." + parameter.Split(',')[3];
                y = Double.Parse(temp);
            }
            else {
                x = Double.Parse(parameter.Split(',')[0].Replace(',', '.'), new CultureInfo("en-US"));
                y = Double.Parse(parameter.Split(',')[1].Replace(',', '.'), new CultureInfo("en-US"));

            }
            return new Point(x, y);
        }
    }
}
