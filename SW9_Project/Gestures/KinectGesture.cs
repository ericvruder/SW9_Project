using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SW9_Project {
    public class KinectGesture {

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
        public KinectGesture(GestureType type, GestureDirection direction, Point p) : this(null, type, direction, p) { }
        public KinectGesture(string shape, GestureDirection direction, Point p) {
            Shape = shape;
            Direction = direction;
            Pointer = p;
            Timestamp = DateTime.Now;
        }

        public KinectGesture(string shape) {
            Shape = shape;
            Type = GestureParser.GetTypeContext();
            Direction = GestureParser.GetDirectionContext();
            Pointer = CanvasWindow.GetCurrentPoint();
            Timestamp = DateTime.Now;
        }
    }
}
