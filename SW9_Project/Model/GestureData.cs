using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace SW9_Project {
    class GestureData {
        public string Name { get; set; }

        public GestureData(string jsonString) {
            JObject gestureData = new JObject(jsonString);
            Name = (string)gestureData["Name"];
        }
    }
}
