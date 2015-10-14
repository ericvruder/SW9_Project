using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project {
    class RotationData : MobileData {

        private RotationData() {
            Type = "RotationData";
        }

        public RotationData(dynamic jsonObject) : this() {

        }
    }
}
