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

        private Socket userSocket = null;
        private bool userAlive = true;

        public User() {
            AllUsers.Add(this);
        }

        public void AddMobileConnection(Socket socketConnection) {
            userSocket = socketConnection;
            Task.Factory.StartNew(() => { ManageMobileConnection(); });
        }

        private void ManageMobileConnection(){
            try {
                Console.WriteLine("User connected! Address: " + userSocket.RemoteEndPoint);
                NetworkStream stream = new NetworkStream(userSocket);
                StreamReader sr = new StreamReader(stream);
                StreamWriter sw = new StreamWriter(stream);
                sw.AutoFlush = true;
                sw.WriteLine("Received your connection!");
                while (true) {
                    string readLine = sr.ReadLine();
                    if (readLine == "quit") { break; }
                    Console.WriteLine(readLine);
                }
                sw.Close();
                sr.Close();
                stream.Close();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            } finally {
                userSocket.Close();
            }
        }
    }
}
