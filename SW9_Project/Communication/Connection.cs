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
    public class Connection {

        Socket socket;

        private static List<Connection> allConnections;
        private static bool alive = true;

        public static List<Connection> AllConnections {
            get {
                if (allConnections == null) {
                    allConnections = new List<Connection>();
                }
                return allConnections;
            }
        }

        public Connection(Socket socket) {
            this.socket = socket;

            Task.Factory.StartNew(() => {
                ManageMobileConnection();
            });

        }

        static byte[] GetBytes(string str) {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static void StartService() {
            UdpClient dispatcher = new UdpClient(49255);
            Task.Factory.StartNew(() => {
                byte[] response;
                //response = GetBytes( "DISCOVER_IS903SERVER_RESPONSE");
                response = Encoding.ASCII.GetBytes("DISCOVER_IS903SERVER_RESPONSE");
                while (alive) {
                    var remoteEP = new IPEndPoint(IPAddress.Any, 49255);
                    var data = dispatcher.Receive(ref remoteEP); // listen on port 49255
                    Console.Write("receive data from " + remoteEP.ToString());
                    dispatcher.Send(response, response.Length, remoteEP); //reply back
                }
            });
        }

        public static void StartService(int port = 8000) {

            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine("Now listening on port " + port);
            Task.Factory.StartNew(() => {
                while (alive) {
                    Socket socket = listener.AcceptSocket();
                    AllConnections.Add(new Connection(socket));
                }
                listener.Stop();
            });
        }
        StreamWriter sw;
        bool ready = false;
        private void ManageMobileConnection() {
            try {
                Console.WriteLine("User connected! Address: " + socket.RemoteEndPoint);
                CanvasWindow.SetConnection(this);

                using (NetworkStream stream = new NetworkStream(socket))
                using (StreamReader sr = new StreamReader(stream))
                using (sw = new StreamWriter(stream)) {
                    sw.AutoFlush = true;
                    sw.WriteLine("startpush");
                    sw.WriteLine("startpull");
                    string startdirection = GestureParser.GetDirectionContext() == GestureDirection.Pull ? "startpull" : "startpush";
                    sw.WriteLine(startdirection);
                    while (true) {
                        String line = sr.ReadLine();
                        if (line.Contains("nextshape:")) {
                            nextShape = line.Split(':')[1];
                        } else if (line.Contains("ready:")) {

                        } else {
                            dynamic jO = JsonConvert.DeserializeObject(line);
                            if (jO.GetType().GetProperty("Type") != null) {
                                GestureParser.AddMobileGesture(new MobileGesture(jO));
                            }
                        }
                    }
                }

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            } finally {
                socket.Close();
            }
        }

        public void StartTest(GestureDirection direction) {
            String test = direction == GestureDirection.Pull ? "startpull" : "startpush";
            sw.WriteLine(test);
            sw.Flush();
        }

        public void SwitchShapes() {
            sw.WriteLine("switch");
            Thread.Sleep(50);
        }

        String nextShape = "";
        public string GetNextShape() {
            sw.WriteLine("nextshape");
            String response = "";
            while(nextShape == "") {
                Thread.Sleep(50);
            }
            response = nextShape;
            nextShape = "";
            return response;
        }
    }
}
