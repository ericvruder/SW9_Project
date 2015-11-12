using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project {
    class Target {
        public int X { get; set; }
        public int Y { get; set; }
        public GridSize Size { get; set; }
        public Target(int x, int y, GridSize size) {
            X = x;
            Y = y;
            Size = size;
        }
    }
}
