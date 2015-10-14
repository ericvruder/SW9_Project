using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

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

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static void StartService(int port = 8000) {

            UdpClient dispatcher = new UdpClient(49255);
            Task.Factory.StartNew(() =>
            {
                byte[] response;
                //response = GetBytes( "DISCOVER_IS903SERVER_RESPONSE");
                response = Encoding.ASCII.GetBytes("DISCOVER_IS903SERVER_RESPONSE");
                while (alive)
                {
                    var remoteEP = new IPEndPoint(IPAddress.Any, 49255);
                    var data = dispatcher.Receive(ref remoteEP); // listen on port 49255
                    Console.Write("receive data from " + remoteEP.ToString());
                    dispatcher.Send(response, response.Length, remoteEP); //reply back
                }
            });

            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Now listening on port " + port);
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
                        dynamic jO = JsonConvert.DeserializeObject(readLine);
                        if (jO.Type == "AccelerometerData") {
                            user.ParseMobileData(new AccelerometerData(jO));
                        }
                        if(jO.Type == "RotationData") {
                            user.ParseMobileData(new RotationData(jO));
                        }
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
