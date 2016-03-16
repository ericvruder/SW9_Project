﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataSetGenerator;

using Point = System.Windows.Point;

namespace SW9_Project {
    public class KinectGesture {

        public String Shape;
        public GestureType Type;
        public GestureDirection Direction;
        public Point Pointer;
        public DateTime Timestamp;

        public KinectGesture(GestureType type, GestureDirection direction): this(null) {
            Type = type;
            Direction = direction;
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
