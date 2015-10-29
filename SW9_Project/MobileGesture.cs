using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project {
    class MobileGesture {
        public string Shape;
        public GestureType Type;
        public GestureDirection Direction;
        public DateTime Timestamp;

        public MobileGesture(dynamic jsonObject) {
            Shape = jsonObject["Shape"];
            switch ((string)jsonObject["Type"]) {
                case "Throw": Type = GestureType.Throw; break;
                case "Pinch": Type = GestureType.Pinch; break;
                case "Tilt": Type = GestureType.Tilt; break;
                case "Swipe": Type = GestureType.Swipe; break;
                default: break;
            }
            switch ((string)jsonObject["Direction"]) {
                case "Pull": Direction = GestureDirection.Pull; break;
                case "Push": Direction = GestureDirection.Push; break;
                default: break;
            }
            Timestamp = DateTime.Now;
        }

    }
}
