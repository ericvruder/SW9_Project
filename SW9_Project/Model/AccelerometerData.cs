using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project {
    class AccelerometerData : MobileData {

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public long Time { get; set; }

        public AccelerometerData(float x, float y, float z, long time) {
            X = x; Y = y; Z = z;
            Time = time;
        }

    }
}
