using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SW9_Project {
    class MobileGesture {
        public string Type { get; set; }

        public MobileGesture(string jsonString) {
            dynamic jsonObject = JsonConvert.DeserializeObject(jsonString);
            Type = jsonObject.Type;
        }
    }
}
