using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project {
    class User {
        static List<User> allUsers;
        public static List<User> AllUsers {
            get {
                if(allUsers == null) {
                    allUsers = new List<User>();
                }
                return allUsers;
            }
        }
        

        public User() {
            AllUsers.Add(this);
        }
        
        public static User FindUser(string something) {
            //TODO: FIX THIS! :D
            return new User();
        }

        public void SendData(MobileGesture data) {

        }
        
    }
}
