using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project {
    class Connection {
        
        Socket socket;
        User user;

        private static List<Connection> allConnections;
        private static bool alive = true;

        public static List<Connection> AllConnections { get {
                if(allConnections == null) {
                    allConnections = new List<Connection>();
                }
                return allConnections;
            }
        }

        public Connection(User user, Socket socket) {
            this.user = user;
            this.socket = socket;

            Task.Factory.StartNew(() => {
                ManageMobileConnection(user);
            });

        }

        public static void StartService(int port = 8000) {
            
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Task.Factory.StartNew(() => {
                while (alive) {
                    Socket socket = listener.AcceptSocket();
                    AllConnections.Add(new Connection(User.FindUser("something"), socket));
                }
                listener.Stop();
            });
        }

        private void ManageMobileConnection(User user) {
            this.user = user;
            try {
                Console.WriteLine("User connected! Address: " + socket.RemoteEndPoint);

                using (NetworkStream stream = new NetworkStream(socket))
                using (StreamReader sr = new StreamReader(stream))
                using (StreamWriter sw = new StreamWriter(stream)) {
                    sw.AutoFlush = true;
                    sw.WriteLine("Received your connection!");
                    while (true) {
                        string readLine = sr.ReadLine();
                        if (readLine == "quit") { break; }
                        Console.WriteLine(readLine);
                        user.SendData(new MobileGesture(readLine));
                    }
                }

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            } finally {
                socket.Close();
            }
        }
    }
}
