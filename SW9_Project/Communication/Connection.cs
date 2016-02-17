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
using System.Diagnostics;

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

            UdpClient dispatcher = new UdpClient(49255);
            Gyro gyro = new Gyro();
            Task.Factory.StartNew(() =>
            {
                byte[] response;
                int packets = 0;
                Stopwatch tStart =  new Stopwatch();
                long tDelta;
                double pps;
                response = Encoding.ASCII.GetBytes("DISCOVER_IS903SERVER_RESPONSE");
                while (alive)
                {
                    var remoteEP = new IPEndPoint(IPAddress.Any, 49255);
                    var data = dispatcher.Receive(ref remoteEP); // listen on port 49255
                    string returnData = Encoding.ASCII.GetString(data);
                    Console.WriteLine("UDP from " + remoteEP.ToString());
                    if (returnData.StartsWith("gyrodata"))
                    {
                        if (packets == 0)
                            tStart.Start();
                        gyro.updateUI(returnData.Split(':')[2], returnData.Split(':')[4], returnData.Split(':')[6], returnData.Split(':')[8]);
                        packets++;
                        tDelta = tStart.ElapsedMilliseconds;
                        pps = packets / (tDelta / 1000.0);
                        //Console.WriteLine(packets + "/" + (tDelta / 1000.0) + " = " +pps);
                    }
                    dispatcher.Send(response, response.Length, remoteEP); //reply back
                }
            });

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
        StreamReader sr;
        private void ManageMobileConnection() {
            try {
                Console.WriteLine("User connected! Address: " + socket.RemoteEndPoint);
                CanvasWindow.SetConnection(this);
                Connected = true;

                using (NetworkStream stream = new NetworkStream(socket))
                using (sr = new StreamReader(stream))
                using (sw = new StreamWriter(stream)) {
                    sw.AutoFlush = true;
                    sw.WriteLine("startpush");
                    while (true) {
                        String line = sr.ReadLine();
                        if (line.Contains("nextshape:")) {
                            nextShape = line.Split(':')[1];
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
                Connected = false;
            }
        }

        public bool Connected { get; set; }

        public void StartTest(GestureDirection direction) {
            String test = direction == GestureDirection.Pull ? "startpull" : "startpush";
            sw.WriteLine(test);
            sw.Flush();
        }

        public void SwitchShapes() {
            sw.WriteLine("switch");
            Thread.Sleep(50);
        }

        public void SendPinch() {
            sw.WriteLine("pinch:pull");
        }

        String nextShape = "";
        public void SetNextShape(string shape) {
            sw.WriteLine("nextshape:" + shape);
        }
    }
}
