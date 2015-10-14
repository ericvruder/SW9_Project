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


        private AccelerometerData() {
            Type = "AccelerometerData";
        }

        public AccelerometerData(float x, float y, float z, long time) : this() {
            X = x; Y = y; Z = z;
            Time = time;
        }

        public AccelerometerData(dynamic jsonObject) : this() {
            X = Convert.ToSingle(jsonObject.X);
            Y = Convert.ToSingle(jsonObject.Y);
            Z = Convert.ToSingle(jsonObject.Z);
            Time = Convert.ToInt64(jsonObject.Time);

        }

    }
}
