using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SW9_Project {
    class MobileData {

        public string Name { get; set; }
        public string TeamName { get; set; }
        public string Email { get; set; }

        public MobileData(string jsonString) {
            JObject t = JObject.Parse(jsonString);
            JToken token = t["user"];
            Name = (string)t["name"];
            TeamName = (string)t["teamname"];
            Email = (string)t["email"];
        }
    }
}
