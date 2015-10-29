using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SW9_Project {
    class KinectGesture {

        public string Shape;
        public GestureType Type;
        public GestureDirection Direction;
        public Point Pointer;
        public DateTime Timestamp;

        public KinectGesture(string shape, GestureType type, GestureDirection direction, Point p) {
            Type = type;
            Direction = direction;
            Pointer = p;
            Timestamp = DateTime.Now;
        }
    }
}
